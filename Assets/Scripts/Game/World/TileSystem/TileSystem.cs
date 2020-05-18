using Game.FogOfWar;
using Lortedo.Utilities.Pattern;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void OnTileTerrainChanged(Vector2Int coords, GameObject gameObjectAtCoords);

public class TileSystem : Singleton<TileSystem>
{
    #region Fields
    public event OnTileTerrainChanged OnTileTerrainChanged;

    private Dictionary<Vector2Int, GameObject> _tiles = new Dictionary<Vector2Int, GameObject>();
    #endregion

    #region Methods
    #region MonoBehaviour Callbacks
    public void Start()
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

    public bool TrySetTile(GameObject gameObject)
    {
        return TrySetTile(gameObject.transform.position, gameObject);
    }

    private bool TrySetTile(Vector3 worldPosition, GameObject gameObject)
    {
        Vector2Int coords = WorldPositionToCoords(worldPosition);
        return TrySetTile(coords, gameObject, worldPosition);
    }

    private bool TrySetTile(Vector2Int coords, GameObject gameObject, Vector3 worldPosition)
    {
        if (_tiles.ContainsKey(coords) == false)
        {
            Debug.LogError("Can't set GameObject on Tile " + coords + ". It doesn't exist!");
            return false;
        }

        if (_tiles[coords] != null)
        {
            UIMessagesLogger.Instance.AddErrorMessage("You can't build on non-empty cell.");
            return false;
        }

        if (FOWManager.Instance != null)
        {
            if (FOWManager.Instance.TryGetTile(worldPosition, out FogState fogState))
            {
                if (fogState != FogState.Visible)
                {
                    UIMessagesLogger.Instance.AddErrorMessage("Can't build in fog of war");
                    return false;
                }
            }
            else
            {
                Debug.LogErrorFormat("Can't get fow of war tile at world position {0}", worldPosition);
                return false;
            }
        }

        _tiles[coords] = gameObject;
        OnTileTerrainChanged?.Invoke(coords, gameObject);

        return true;
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
    #endregion
}
