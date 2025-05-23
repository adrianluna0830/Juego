using System;
using System.Collections.Generic;
using UnityEngine;

public readonly struct TileCoordinate
{
    public readonly int Sector;
    public readonly int Ring;

    public TileCoordinate(int sector, int ring)
    {
        Sector = sector;
        Ring = ring;
    }
}

public readonly struct RingData
{
    public readonly float StartRadius;
    public readonly float EndRadius;
    public readonly float AverageRadius;

    public RingData(float startRadius, float endRadius)
    {
        StartRadius = startRadius;
        EndRadius = endRadius;
        AverageRadius = (startRadius + endRadius) / 2;
    }
}

public class PolarGridSystem
{
    private const int MINIMUM_RING_VALUE = 0;
    private readonly float ringStep;
    private readonly float sectorStep;
    private readonly Transform origin;


    public PolarGridSystem(float ringStep, float sectorStep, Transform origin)
    {
        this.ringStep = ringStep;
        this.sectorStep = sectorStep;
        this.origin = origin;
    }


    #region Public Methods
    

    
    public Vector3 GetTilePosition(TileCoordinate tile)
    {
        var ringData = GetRingData(tile.Ring);
        var totalSectors = CalculateTotalSectors(ringData.AverageRadius);
        var normalizedSector = NormalizeSectorIndex(tile.Sector, totalSectors);

        return CalculateSectorCenterPosition(normalizedSector, totalSectors, ringData.AverageRadius);
    }
    
    public TileCoordinate GetTileCoordinate(Vector3 position)
    {
        var relativePosition = position - origin.position;
        var distance = relativePosition.magnitude;
        var ring = GetRingFromDistance(distance);
    
        var ringData = GetRingData(ring);
        var sector = GetSector(position, ringData);

        return GetValidTileCoordinate(sector, ring);
    }


    public (Vector3 forward, Vector3 right, Vector3 up) GetTileRelativeCoordinateSystem(Vector3 position)
    {
        var center = GetTilePosition(GetTileCoordinate(position));
        var forward = (center - origin.position).normalized;
        var up = Vector3.up;
        var right = Vector3.Cross(up, forward).normalized;

        return (forward, right, up);
    }
    
    public (Vector3 forward, Vector3 right, Vector3 up) GetTileRelativeCoordinateSystem(TileCoordinate tile)
    {
        // Calcula la posición del centro del tile
        var center = GetTilePosition(tile);

        // Llama la versión existente del método para reutilizar la lógica
        return GetTileRelativeCoordinateSystem(center);
    }
    
    
    public TileCoordinate GetBackwardNeighbour(TileCoordinate tile)
    {
   

        var center = GetTilePosition(tile);
        var (forward, _, _) = GetTileRelativeCoordinateSystem(center);

        var newPosition = center + forward * ringStep;

        return GetTileCoordinate(newPosition);;
    }

    public TileCoordinate GetBackwardNeighbour(Vector3 position)
    {
        return GetForwardNeighbour(GetTileCoordinate(position));
    }


    public TileCoordinate GetForwardNeighbour(TileCoordinate tile)
    {
        
        var center = GetTilePosition(tile);
        var (forward, _, _) = GetTileRelativeCoordinateSystem(center);

        var newPosition = center - forward * ringStep;

        return GetTileCoordinate(newPosition);;
    }

    public TileCoordinate GetForwardNeighbour(Vector3 position)
    {
        return GetBackwardNeighbour(GetTileCoordinate(position));
    }


    public int GetRingFromDistance(float distance)
    {
        if (distance < 0) throw new ArgumentException("Distance cannot be negative.");
        return Mathf.FloorToInt(distance / ringStep);
    }
    
    public TileCoordinate GetValidTileCoordinate(int sector, int ring)
    {
        if (ring < 0)
        {
            throw new ArgumentException("Ring cannot be negative.");
        }
        var ringData = GetRingData(ring);
        var totalSectors = CalculateTotalSectors(ringData.AverageRadius);
        sector = NormalizeSectorIndex(sector, totalSectors);

        return new TileCoordinate(sector, ring);
    }
    
    

    #endregion

    #region Private Helper Methods




    /// <summary>
    ///     Calculates the sector index of a position within a specific ring in the polar grid system.
    /// </summary>
    /// <param name="position">The world position to convert into a sector index.</param>
    /// <param name="ringData">The data defining the specific ring, including its start radius, end radius, and average radius.</param>
    /// <returns>
    ///     The calculated sector index as an integer if the position is within the specified ring and valid.
    ///     Returns -1 if the position is outside the ring bounds or if the sector step is invalid.
    /// </returns>
    private int GetSector(Vector3 position, RingData ringData)
    {
        var relativePosition = position - origin.position;
        var distance = relativePosition.magnitude;

        if (distance < ringData.StartRadius)
            throw new ArgumentException(
                "Position is too close to the origin and outside the start radius of the ring.");

        if (distance > ringData.EndRadius)
            throw new ArgumentException("Position is too far from the origin and outside the end radius of the ring.");

        if (sectorStep <= 0) throw new ArgumentException("Sector step must be greater than zero.");

        var totalSectors = CalculateTotalSectors(ringData.AverageRadius);
        var angle = GetGlobalAngle(position);
        var anglePerSector = 360f / totalSectors;

        return Mathf.FloorToInt(angle / anglePerSector);
    }

    /// <summary>
    ///     Calculates the global angle of a given position relative to the origin of the polar grid system.
    /// </summary>
    /// <param name="position">The position in world space for which to calculate the angle.</param>
    /// <returns>The global angle in degrees, measured clockwise from the positive Z-axis.</returns>
    private float GetGlobalAngle(Vector3 position)
    {
        var direction = position - origin.position;
        var angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
        return (angle + 360f) % 360f;
    }

    /// <summary>
    ///     Calculates the RingData for the specified ring, which includes the start radius,
    ///     end radius, and average radius.
    /// </summary>
    /// <param name="ring">The ring index for which to calculate the radius data.</param>
    /// <returns>A RingData struct containing the start radius, end radius, and average radius for the specified ring.</returns>
    private RingData GetRingData(int ring)
    {
        ring = Mathf.Max(MINIMUM_RING_VALUE, ring);
        var startRadius = ring * ringStep;
        var endRadius = (ring + 1) * ringStep;

        return new RingData(startRadius, endRadius);
    }

    /// <summary>
    ///     Normalizes the given sector index to ensure it falls within the valid range [0, totalSectors - 1].
    /// </summary>
    /// <param name="sector">The sector index to be normalized. Can be any integer.</param>
    /// <param name="totalSectors">The total number of sectors. Must be greater than zero.</param>
    /// <returns>The normalized sector index within the range [0, totalSectors - 1].</returns>
    private int NormalizeSectorIndex(int sector, int totalSectors)
    {
        return (sector % totalSectors + totalSectors) % totalSectors;
    }

    /// Calculates the total number of sectors based on the given radius and the predefined sector step size.
    /// <param name="radius">The radius of the ring for which the total sectors need to be calculated.</param>
    /// <returns>The total number of sectors for the specified radius. Ensures a minimum of 4 sectors.</returns>
    private int CalculateTotalSectors(float radius)
    {
        var circumference = 2 * Mathf.PI * radius;
        return Mathf.Max(4, Mathf.RoundToInt(circumference / sectorStep));
    }

    /// <summary>
    ///     Calculates the center position of a sector in a polar grid based on the given sector index,
    ///     total number of sectors, and the radius of the ring.
    /// </summary>
    /// <param name="sector">The index of the sector to calculate the center for.</param>
    /// <param name="totalSectors">The total number of sectors in the ring.</param>
    /// <param name="radius">The radius of the ring where the sector is located.</param>
    /// <returns>A <see cref="Vector3" /> representing the center position of the specified sector.</returns>
    private Vector3 CalculateSectorCenterPosition(int sector, int totalSectors, float radius)
    {
        var anglePerSector = 360f / totalSectors;
        var sectorCenterAngle = sector * anglePerSector + anglePerSector / 2f;
        var sectorCenterRadians = sectorCenterAngle * Mathf.Deg2Rad;
        var direction = new Vector3(
            Mathf.Sin(sectorCenterRadians),
            0f,
            Mathf.Cos(sectorCenterRadians)
        );
        return origin.position + direction * radius;
    }

    #endregion
}


// private int CalculateTotalSectors(float radius)
// {
//     float circumference = 2 * Mathf.PI * radius;
//     return Mathf.Max(4, Mathf.RoundToInt(circumference / sectorStep));
// }

// using System;
// using System.Collections.Generic;
// using UnityEngine;
//
// public class PolarGridSystem
// {
//     private readonly float ringStep; // Distancia entre cada anillo
//     private readonly float sectorStep; // Distancia mínima para los sectores
//     private readonly Transform origin; // Transform que define el origen
//
//     public PolarGridSystem(float ringStep, float sectorStep, Transform origin)
//     {
//         this.ringStep = ringStep;
//         this.sectorStep = sectorStep;
//         this.origin = origin;
//     }
//
//     public List<(int sector, int ring)> GetNearbyTiles(Vector3 position)
//     {
//         List<(int sector, int ring)> tiles = new List<(int sector, int ring)>();
//
//         var centerTile = GetTileData(position);
//         var coordinateSystem = GetTileRelativeCoordinateSystem(position);
//
//         var tileHeight = GetRingHeight();
//
//         var topTilePosition = centerTile.center + (coordinateSystem.forward * tileHeight);
//         var bottomTilePosition = centerTile.center + (-coordinateSystem.forward * tileHeight);
//
//         var topTile = GetTileData(topTilePosition);
//         var bottomTile = GetTileData(bottomTilePosition);
//
//         tiles.Add((centerTile.sector, centerTile.ring));
//         tiles.Add((centerTile.sector, centerTile.ring + 1));
//         tiles.Add((centerTile.sector, centerTile.ring - 1));
//
//         tiles.Add((topTile.sector, topTile.ring));
//         tiles.Add((topTile.sector, topTile.ring + 1));
//         tiles.Add((topTile.sector, topTile.ring - 1));
//
//         tiles.Add((bottomTile.sector, bottomTile.ring));
//         tiles.Add((bottomTile.sector, bottomTile.ring + 1));
//         tiles.Add((bottomTile.sector, bottomTile.ring - 1));
//
//         return tiles;
//     }
//
//     public List<(int sector, int ring)> GetNearbyTiles(int sector, int ring)
//     {
//         List<(int sector, int ring)> tiles = new List<(int sector, int ring)>();
//
//         var centerTilePosition = GetTilePosition(sector, ring);
//         var coordinateSystem = GetTileRelativeCoordinateSystem(centerTilePosition);
//
//         var tileHeight = GetRingHeight();
//
//         var topTilePosition = centerTilePosition + (coordinateSystem.forward * tileHeight);
//         var bottomTilePosition = centerTilePosition + (-coordinateSystem.forward * tileHeight);
//
//         var topTile = GetTileData(topTilePosition);
//         var bottomTile = GetTileData(bottomTilePosition);
//
//         foreach (var horizontalNeighbour in GetHorizontalNeighbours(sector, ring)) tiles.Add(horizontalNeighbour);
//         foreach (var horizontalNeighbour in GetHorizontalNeighbours(topTile.sector, topTile.ring))
//             tiles.Add(horizontalNeighbour);
//         foreach (var horizontalNeighbour in GetHorizontalNeighbours(bottomTile.sector, bottomTile.ring))
//             tiles.Add(horizontalNeighbour);
//
//         return tiles;
//     }
//
//     public HashSet<(int sector, int ring)> GetHorizontalNeighbours(int sector, int ring)
//     {
//         if (ring < 0)
//         {
//             throw new Exception("Ring cannot be negative");
//         }
//
//         HashSet<(int sector, int ring)> neighbours = new HashSet<(int sector, int ring)>();
//
//         neighbours.Add((sector, ring));
//         neighbours.Add(GetValidIndexData(sector + 1, ring));
//         neighbours.Add(GetValidIndexData(sector - 1, ring));
//
//         return neighbours;
//     }
//
//     public (Vector3 forward, Vector3 right, Vector3 up) GetTileRelativeCoordinateSystem(Vector3 position)
//     {
//         var (center, sector, ring) = GetTileData(position);
//
//         Vector3 forward = GetCenterToPositionDirection(center);
//         Vector3 up = Vector3.up;
//         Vector3 right = Vector3.Cross(up, forward);
//
//         return (forward.normalized, right.normalized, up);
//     }
//
//     public Vector3 GetCenterToPositionDirection(Vector3 position)
//     {
//         return position - origin.position;
//     }
//
//     public Vector3 GetTilePosition(int sector, int ring)
//     {
//         ring = Mathf.Max(0, ring);
//
//         float startRadius = ring * ringStep;
//         float endRadius = (ring + 1) * ringStep;
//         float avgRadius = (startRadius + endRadius) / 2;
//
//         int totalSectors = CalculateTotalSectors(avgRadius);
//
//         // Ajustamos el sector al rango válido
//         sector = ((sector % totalSectors) + totalSectors) % totalSectors;
//
//         return CalculateSectorCenterPosition(sector, totalSectors, avgRadius);
//     }
//
//     public List<(Vector3 center, int sector, int ring)> GetClosestAndNotOccupiedTile(Vector3 position, int radius)
//     {
//         List<(Vector3 center, int sector, int ring)> tiles = new List<(Vector3 center, int sector, int ring)>();
//
//         var centerTile = GetTileData(position);
//         var height = GetRingHeight();
//         var length = GetSectorArcLength(centerTile.ring);
//
//         // Lógica para determinar las casillas más cercanas aún no ocupadas
//         return tiles;
//     }
//
//     public float GetSectorArcLength(int ring)
//     {
//         ring = Mathf.Max(0, ring);
//         float startRadius = ring * ringStep;
//         float endRadius = (ring + 1) * ringStep;
//         float avgRadius = (startRadius + endRadius) / 2;
//         int totalSectors = CalculateTotalSectors(avgRadius);
//
//         float circumference = 2 * Mathf.PI * avgRadius;
//         float arcLength = circumference / totalSectors;
//         return arcLength;
//     }
//
//     public float GetRingHeight()
//     {
//         return ringStep;
//     }
//
//     public (int sector, int ring) GetValidIndexData(int sector, int ring)
//     {
//         ring = Mathf.Max(0, ring);
//
//         float startRadius = ring * ringStep;
//         float endRadius = (ring + 1) * ringStep;
//         float avgRadius = (startRadius + endRadius) / 2;
//
//         int totalSectors = CalculateTotalSectors(avgRadius);
//
//         // Ajustamos el sector al rango válido
//         sector = ((sector % totalSectors) + totalSectors) % totalSectors;
//
//         return (sector, ring);
//     }
//
//     public (Vector3 center, int sector, int ring) GetTileData(Vector3 position)
//     {
//         Vector3 originToPosition = position - origin.position;
//         float distance = originToPosition.magnitude;
//
//         int ring = GetRingFromDistance(distance);
//         if (ring < 0)
//         {
//             return (Vector3.zero, -1, -1);
//         }
//
//         (float startRadius, float endRadius, float avgRadius) = CalculateRingRadii(ring);
//
//         int sector = GetSector(position, startRadius, endRadius, avgRadius);
//         if (sector < 0)
//         {
//             return (Vector3.zero, -1, ring);
//         }
//
//         (sector, ring) = GetValidIndexData(sector, ring);
//         int totalSectors = CalculateTotalSectors(avgRadius);
//         Vector3 centerPosition = CalculateSectorCenterPosition(sector, totalSectors, avgRadius);
//
//         return (centerPosition, sector, ring);
//     }
//
//     private int GetSector(Vector3 position, float startRadius, float endRadius, float avgRadius)
//     {
//         Vector3 toPosition = position - origin.position;
//         float distance = toPosition.magnitude;
//
//         if (distance < startRadius || distance > endRadius || sectorStep <= 0)
//         {
//             return -1;
//         }
//
//         int totalSectors = CalculateTotalSectors(avgRadius);
//         float angle = GetGlobalAngle(position);
//         float anglePerSector = 360f / totalSectors;
//
//         return Mathf.FloorToInt(angle / anglePerSector);
//     }
//
//     private float GetGlobalAngle(Vector3 position)
//     {
//         Vector3 direction = position - origin.position;
//         float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
//         return (angle + 360f) % 360f;
//     }
//
//     private (float startRadius, float endRadius, float avgRadius) CalculateRingRadii(int ring)
//     {
//         float startRadius = ring * ringStep;
//         float endRadius = (ring + 1) * ringStep;
//         float avgRadius = (startRadius + endRadius) / 2;
//         return (startRadius, endRadius, avgRadius);
//     }
//

//
//     private Vector3 CalculateSectorCenterPosition(int sector, int totalSectors, float radius)
//     {
//         float anglePerSector = 360f / totalSectors;
//         float sectorCenterAngle = (sector * anglePerSector) + (anglePerSector / 2f);
//         float sectorCenterRadians = sectorCenterAngle * Mathf.Deg2Rad;
//
//         Vector3 direction = new Vector3(
//             Mathf.Sin(sectorCenterRadians),
//             0f,
//             Mathf.Cos(sectorCenterRadians)
//         );
//
//         return origin.position + (direction * radius);
//     }
//     
//     public int GetRingFromDistance(float distance)
//     {
//         if (distance < 0)
//         {
//             return -1;
//         }
//         return Mathf.FloorToInt(distance / ringStep);
//     }
//
// }