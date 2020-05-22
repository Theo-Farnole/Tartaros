using Game.FogOfWar;
using Lortedo.Utilities.Pattern;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public delegate void OnTileTerrainChanged(Vector2Int coords, GameObject gameObjectAtCoords);

public class TileSystem : Singleton<TileSystem>
{
    #region Fields
    public event OnTileTerrainChanged OnTileTerrainChanged;

    private const string debugLogHeader = "Tile System : ";
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
    public bool IsTileFree(Vector3 position)
        => IsTileFree(WorldPositionToCoords(position));

    public bool IsTileFree(Vector2Int coords)
    {
        return (_tiles.ContainsKey(coords) && _tiles[coords] == null);
    }

    public GameObject GetTile(Vector2Int coords)
    {
        if (_tiles.ContainsKey(coords))
        {
            return _tiles[coords];
        }

        return null;
    }

    public bool IsTileOfType(Vector3 position, EntityType type)
        => IsTileOfType(WorldPositionToCoords(position), type);

    public bool IsTileOfType(Vector2Int coords, EntityType type)
    {
        var tile = GetTile(coords);

        if (tile != null && tile.TryGetComponent(out Entity entity))
        {
            return entity.Type == type;
        }
        else
        {
            return false;
        }
    }

    public bool TrySetTile(GameObject gameObject)
    {
        return TrySetTile(gameObject.transform.position, gameObject);
    }

    public bool TrySetTile(GameObject gameObject, Vector2Int buildingSize)
    {
        Vector2Int coords = WorldPositionToCoords(gameObject.transform.position);
        bool tilesFree = DoTilesAreFree(coords, buildingSize);

        if (tilesFree)
        {
            SetTile(gameObject, buildingSize);
            return true;
        }
        else
        {
            Debug.LogFormat(debugLogHeader + "Can't set tile on non empty cells.");
            return false;
        }
    }

    private void SetTile(GameObject gameObject, Vector2Int buildingSize)
    {
        Vector3 gameObjectPosition = gameObject.transform.position;
        Vector2Int coords = WorldPositionToCoords(gameObjectPosition);
        Vector2Int uncenteredCoords = CoordsToUncenteredCoords(coords, buildingSize);

        for (int x = 0; x < buildingSize.x; x++)
        {
            for (int y = 0; y < buildingSize.y; y++)
            {
                Vector2Int coordsOffset = new Vector2Int(x, y);
                Vector2Int tileCoords = uncenteredCoords + coordsOffset;

                bool setTileSuccesful = TrySetTile(tileCoords, gameObject, gameObjectPosition);

                Debug.DrawRay(CoordsToWorldPosition(tileCoords), Vector3.up * 5, setTileSuccesful ? Color.green : Color.red, 5f);
                Assert.IsTrue(setTileSuccesful, "At this code section, 'setTileSuccessful' should always be true.");
            }
        }
    }

    private bool DoTilesAreFree(Vector2Int coords, Vector2Int buildingSize)
    {        
        Vector2Int uncenteredCoords = CoordsToUncenteredCoords(coords, buildingSize);

        for (int x = 0; x < buildingSize.x; x++)
        {
            for (int y = 0; y < buildingSize.y; y++)
            {
                Vector2Int coordsOffset = new Vector2Int(x, y);
                Vector2Int tileCoords = uncenteredCoords + coordsOffset;

                if (!IsTileFree(tileCoords))
                {
                    return false;
                }
            }
        }

        return true;
    }

    private Vector2Int CoordsToUncenteredCoords(Vector2Int centeredCoords, Vector2Int size)
    {
        if (size.x % 2 != 0) size.x--;
        if (size.y % 2 != 0) size.y--;

        Vector2Int uncenteredCoords = centeredCoords - size / 2;

        return uncenteredCoords;
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

        if (FOWManager.Instance != null && !FOWManager.Instance.IsDisabled)
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

    public Vector3 CoordsToWorldPosition(Vector2Int coords)
    {
        var cellSize = GameManager.Instance.Grid.CellSize;
        return new Vector3(coords.x * cellSize, 0, coords.y * cellSize);
    }

    public Vector2Int WorldPositionToCoords(Vector3 worldPosition)
    {
        Vector3 gridPosition = GameManager.Instance.Grid.GetNearestPosition(worldPosition);

        var cellSize = GameManager.Instance.Grid.CellSize;
        Vector2Int result = new Vector2Int((int)(gridPosition.x / cellSize), (int)(gridPosition.z / cellSize));

        return result;
    }

    public bool DoTilesAreNeightboor(Vector2Int coordsOne, Vector2Int coordsTwo)
    {
        if (coordsOne.x + 1 == coordsTwo.x) // coordsTwo at EAST of coordsOne
            return true;
        else if (coordsOne.x - 1 == coordsTwo.x) // coordsTwo at WEST of coordsOne
            return true;
        else if (coordsOne.y + 1 == coordsTwo.y) // coordsTwo at NORTH of coordsOne
            return true;
        else if (coordsOne.y - 1 == coordsTwo.y) // coordsTwo at SOUTH of coordsOne
            return true;
        else
            return false;
    }

    public Vector2Int[] GetPath(Vector3 from, Vector3 to)
        => GetPath(WorldPositionToCoords(from), WorldPositionToCoords(to));

    public static float FindDegree(int x, int y)
    {
        float value = (float)((Mathf.Atan2(x, y) / Mathf.PI) * 180f);
        if (value < 0) value += 360f;

        return value;
    }

    public Vector2Int[] GetPath(Vector2Int from, Vector2Int to)
    {
        Debug.DrawRay(to.ToXZ(), Vector3.up * 10, Color.red);

        Vector2 from2Right = Vector2.right - from;
        Vector2Int from2To = to - from;

        Debug.DrawRay(from.ToXZ(), Vector2.right * 5);
        Debug.DrawLine(from.ToXZ(), to.ToXZ());

        // calculate angle
        float angle = Vector2.SignedAngle(from2Right, from2To); // in degrees
        angle *= Mathf.Deg2Rad; // in rads

        // than, get position from angle
        float deltaX = Mathf.Cos(angle) * Mathf.Abs(from2To.x);
        float deltaY = Mathf.Sin(angle) * Mathf.Abs(from2To.y);

        Vector2Int pathDirection;
        int pathCount;
        var cellSize = GameManager.Instance.Grid.CellSize;

        // the direction of path is X ?
        if (Mathf.Abs(deltaX) >= Mathf.Abs(deltaY))
        {
            pathDirection = Vector2Int.right * (int)Mathf.Sign(-deltaX);
            pathCount = Mathf.RoundToInt(Mathf.Abs(from2To.x / cellSize));
        }
        else
        {
            pathDirection = Vector2Int.up * (int)Mathf.Sign(-deltaY);
            pathCount = Mathf.RoundToInt(Mathf.Abs(from2To.y / cellSize));
        }

        // we add one, to include current start
        pathCount++;

        Assert.IsTrue(pathCount >= 0, "TileSystem : Path count should greater or equals to '0'!");

        // calculate output
        Vector2Int[] o = new Vector2Int[pathCount];

        for (int i = 0; i < pathCount; i++)
        {
            o[i] = from + pathDirection * i;
            Debug.DrawRay(o[i].ToXZ(), Vector3.up * 10);
        }

        return o;
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
