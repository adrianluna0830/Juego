using System.Collections.Generic;

public class TileOccupationSystem
{
    private HashSet<TileCoordinate> occupiedTiles = new HashSet<TileCoordinate>();

    public HashSet<TileCoordinate> GetOccupiedTiles()
    {
        return occupiedTiles;
    }
    public bool GetIsTileOccupied(TileCoordinate tileCoordinate)
    {
        return occupiedTiles.Contains(tileCoordinate);
    }

    public bool TryOccupyTile(TileCoordinate tileCoordinate)
    {
        if (!GetIsTileOccupied(tileCoordinate))
        {
            occupiedTiles.Add(tileCoordinate);
            
            return true;
        }
        return false;
    }

    public void UnoccupyTile(TileCoordinate tileCoordinate)
    {
        occupiedTiles.Remove(tileCoordinate);
    }
}