using System.Collections.Generic;
using UnityEngine;

public class SecondDimentionPolarGridSystem
{
    private readonly float ringStep; 
    private readonly float sectorStep; 

    public SecondDimentionPolarGridSystem(float ringStep, float sectorStep)
    {
        this.ringStep = ringStep;
        this.sectorStep = sectorStep;
    }

    public Vector2 GetTileCenter(int ring, int sector, Vector2 origin)
    {
        ring = Mathf.Max(0, ring);

        float startRadius = ring * ringStep;
        float endRadius = (ring + 1) * ringStep;
        float avgRadius = (startRadius + endRadius) / 2;

        int totalSectors = CalculateTotalSectors(avgRadius);

        sector = ((sector % totalSectors) + totalSectors) % totalSectors;

        return CalculateSectorCenterPosition(sector, totalSectors, avgRadius, origin);
    }

    public List<(Vector2 center, int ring, int sector)> GetNearbyTiles(Vector2 position,Vector2 origin, float radius)
    {
        List<(Vector2 center, int ring, int sector)> tiles = new List<(Vector2 center, int ring, int sector)>();

        var tile = GetTile(position, position);
        var tileCenter = tile.center;
        
        var newOrigin = tileCenter + new Vector2(GetSectorArcLength(tile.ring), GetRingHeight());
        var newTilePos = new Vector2(GetSectorArcLength(tile.ring), GetRingHeight());
        var height = GetRingHeight();
        var length = GetSectorArcLength(tile.ring);

        var newTile = GetTile(tileCenter, origin);

        return tiles;
    }

    public (Vector2 foward, Vector2 right) GetTileRelativeCoordinateSystem(Vector2 position,Vector2 origin)
    {
        var foward = GetCenterToPositionDirection(position, origin);
        var right =new Vector2(-foward.y, foward.x);
        var coordinateSystem = (foward.normalized, right.normalized);
        return coordinateSystem;
    }

    public Vector2 GetCenterToPositionDirection(Vector2 position, Vector2 origin)
    {
        return position - origin;
    }

    public float GetSectorArcLength(int ring)
    {
        ring = Mathf.Max(0, ring);

        // Calculamos el radio del anillo (promedio)
        float startRadius = ring * ringStep;
        float endRadius = (ring + 1) * ringStep;
        float avgRadius = (startRadius + endRadius) / 2;

        int totalSectors = CalculateTotalSectors(avgRadius);

        float circumference = 2 * Mathf.PI * avgRadius;
        float arcLength = circumference / totalSectors;

        return arcLength;
    }


    public float GetRingHeight()
    {
        return ringStep;
    }


    public (Vector2 center, int ring, int sector) GetTile(Vector2 position, Vector2 origin)
    {
        Vector2 originToPosition = position - origin;
        float distance = originToPosition.magnitude;

        int ring = GetRingFromDistance(distance);

        if (ring < 0)
        {
            return (Vector2.zero, -1, -1);
        }

        (float startRadius, float endRadius, float avgRadius) = CalculateRingRadii(ring);

        int sector = GetSector(position, startRadius, endRadius, avgRadius, origin);
        if (sector < 0)
        {
            return (Vector2.zero, ring, -1);
        }

        int totalSectors = CalculateTotalSectors(avgRadius);
        Vector2 centerPosition = CalculateSectorCenterPosition(sector, totalSectors, avgRadius, origin);

        return (centerPosition, ring, sector);
    }

    public int GetRingFromDistance(float distance)
    {
        if (distance < 0)
        {
            return -1;
        }

        int ringIndex = Mathf.FloorToInt(distance / ringStep);
        return ringIndex;
    }

    private int GetSector(Vector2 position, float startRadius, float endRadius, float avgRadius, Vector2 origin)
    {
        Vector2 toPosition = position - origin;
        float distance = toPosition.magnitude;

        if (distance < startRadius || distance > endRadius || sectorStep <= 0)
        {
            return -1;
        }

        int totalSectors = CalculateTotalSectors(avgRadius);
        float angle = GetGlobalAngle(position, origin);
        float anglePerSector = 360f / totalSectors;

        return Mathf.FloorToInt(angle / anglePerSector);
    }

    private float GetGlobalAngle(Vector2 position, Vector2 origin)
    {
        Vector2 direction = position - origin;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        return (angle + 360) % 360;
    }

    private (float startRadius, float endRadius, float avgRadius) CalculateRingRadii(int ring)
    {
        float startRadius = ring * ringStep;
        float endRadius = (ring + 1) * ringStep;
        float avgRadius = (startRadius + endRadius) / 2;
        return (startRadius, endRadius, avgRadius);
    }

    private int CalculateTotalSectors(float radius)
    {
        float circumference = 2 * Mathf.PI * radius;
        return Mathf.Max(4, Mathf.RoundToInt(circumference / sectorStep));
    }

    private Vector2 CalculateSectorCenterPosition(int sector, int totalSectors, float radius, Vector2 origin)
    {
        float anglePerSector = 360f / totalSectors;
        float sectorCenterAngle = (sector * anglePerSector) + (anglePerSector / 2);
        float sectorCenterRadians = sectorCenterAngle * Mathf.Deg2Rad;

        Vector2 direction = new Vector2(
            Mathf.Cos(sectorCenterRadians),
            Mathf.Sin(sectorCenterRadians)
        );

        return origin + (direction * radius);
    }
}