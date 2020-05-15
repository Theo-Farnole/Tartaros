using Lortedo.Utilities.Pattern;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileSystem
{
    #region Fields
    private Dictionary<Vector2Int, GameObject> _tiles = new Dictionary<Vector2Int, GameObject>();
    #endregion

    #region Ctor
    public TileSystem()
    {
        ResetAllTiles();
    }
    #endregion

    #region Public Methods
    public GameObject GetTile(Vector2Int coords)
    {
        if (_tiles.ContainsKey(coords))
        {
            return _tiles[coords];
        }

        return null;
    }

    public void SetTile(Vector2Int coords, GameObject gameObject)
    {
        if (_tiles.ContainsKey(coords) == false)
        {
            Debug.LogError("Can't set GameObject on Tile " + coords + ". It doesn't exist!");
            return;
        }

        _tiles[coords] = gameObject;

        GameManager.Instance.InvokeOnTileTerrainChanged(coords, gameObject);
    }

    public GameObject GetTileFromWorldCoords(Vector3 worldPosition)
    {
        return GetTile(WorldPositionToCoords(worldPosition));
    }

    public Vector2Int WorldPositionToCoords(Vector3 worldPosition)
    {
        Vector3 gridPosition = GameManager.Instance.Grid.GetNearestPosition(worldPosition);

        var cellSize = GameManager.Instance.Grid.CellSize;
        Vector2Int result = new Vector2Int((int)(gridPosition.x / cellSize), (int)(gridPosition.z / cellSize));

        return result;
    }
    #endregion

    #region Private methods
    /// <summary>
    /// Set each tile to null.
    /// </summary>
    private void ResetAllTiles()
    {
        int midCellCount = GameManager.Instance.Grid.CellCount / 2;

        for (int x = -midCellCount; x <= midCellCount; x++)
        {
            for (int y = -midCellCount; y <= midCellCount; y++)
            {
                _tiles.Add(new Vector2Int(x, y), null);
            }
        }
    }
    #endregion
}
