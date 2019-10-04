using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileSystem : Singleton<TileSystem>
{
    #region Fields
    [SerializeField] private TileDatabase _data;

    private Dictionary<Vector2Int, GameObject> _tiles = new Dictionary<Vector2Int, GameObject>();
    #endregion

    #region Methods
    void Awake()
    {
        // init tiles dictionnary
        int midCellCount = _data.CellCount / 2;

        for (int x = -midCellCount; x <= midCellCount; x++)
        {
            for (int y = -midCellCount; y <= midCellCount; y++)
            {
                _tiles.Add(new Vector2Int(x, y), null);
            }
        }
    }
    
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
    }

    public GameObject GetTileFromWorldCoords(Vector3 worldPosition)
    {
        return GetTile(WorldPositionToCoords(worldPosition));
    }

    public Vector2Int WorldPositionToCoords(Vector3 worldPosition)
    {
        Vector3 gridPosition = GameManager.Instance.Grid.GetNearestPointOnGrid(worldPosition);

        Vector2Int result = new Vector2Int((int)(gridPosition.x / _data.CellSize), (int)(gridPosition.z / _data.CellSize));

        return result;
    }
    #endregion
}
