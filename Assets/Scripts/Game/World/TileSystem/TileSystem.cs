using Game.Entities;
using Game.FogOfWar;
using Lortedo.Utilities.Pattern;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public delegate void OnTileTerrainChanged(Vector2Int coords, GameObject gameObjectAtCoords);

[System.Flags]
public enum TileFlag
{
    None = 0,
    Free = 1,
    Visible = 2
}

/// <summary>
/// This script used in building to know if a tile is free or not.
/// </summary>
public class TileSystem : Singleton<TileSystem>
{
    #region Fields
    private const string debugLogHeader = "Tile System : ";

    public event OnTileTerrainChanged OnTileTerrainChanged;

    private Dictionary<Vector2Int, GameObject> _tiles = new Dictionary<Vector2Int, GameObject>();
    #endregion

    #region Methods
    #region MonoBehaviour Callbacks
    private void Start()
    {
        ResetAllTiles();
    }

    void OnEnable()
    {
        Entity.OnDeath += Entity_OnDeath;
    }

    void OnDisable()
    {
        Entity.OnDeath -= Entity_OnDeath;
    }
    #endregion

    #region Events Handlers
    private void Entity_OnDeath(Entity entity)
    {
        if (entity.Data.EntityType == EntityType.Building)
        {
            Vector3 worldPosition = entity.transform.position;
            Vector2Int coords = WorldToCoords(worldPosition);

            // only set tile to null
            // if tile is register at it position
            //
            // without this 'if', in construction mode
            // pending construction building can't overwrite constructed building
            if (GetTile(coords) == entity.gameObject)
            {
                SetTile(null, entity.Data.TileSize, coords, true);
            }
        }
    }
    #endregion

    #region IsTile Methods
    public bool IsTileFree(Vector3 position)
        => IsTileFree(WorldToCoords(position));

    public bool IsTileFree(Vector2Int coords)
    {
        return (_tiles.ContainsKey(coords) && _tiles[coords] == null);
    }

    public bool IsTileVisible(Vector3 position)
    {
        Assert.IsNotNull(FOWManager.Instance, "Missing FOWManager.");

        return FOWManager.Instance.TryGetTile(position, out FogState fogState) && fogState == FogState.Visible;
    }

    public bool DoTileContainsEntityOfID(Vector3 worldPosition, string entityID)
        => DoTileContainsEntityOfID(WorldToCoords(worldPosition), entityID);

    public bool DoTileContainsEntityOfID(Vector2Int coords, string entityID)
    {
        GameObject tile = GetTile(coords);

        Assert.IsNotNull(tile, "Tile is null. Can't get it entity of type.");

        return GetTileID(coords) == entityID;
    }
    #endregion

    #region Tiles Getter Methods
    private string GetTileID(Vector2Int coords)
    {
        GameObject tile = GetTile(coords);

        if (tile != null && tile.TryGetComponent(out Entity entity))
        {
            return entity.EntityID;
        }
        else
        {
            throw new System.NotSupportedException("Can't get ID because the tile is null or has not entity component on it.");
        }
    }
    #endregion

    #region Fill Conditions Methods
    public bool DoTilesFillConditions(Vector3 worldPosition, Vector2Int size, TileFlag condition, bool logBuildError = false)
        => DoTilesFillConditions(worldPosition, size, condition, string.Empty, logBuildError);

    public bool DoTilesFillConditions(Vector3 worldPosition, Vector2Int size, TileFlag condition, string entityID, bool logBuildError = false)
    {
        Vector2Int coords = WorldToCoords(worldPosition);
        Vector2Int uncenteredCoords = CoordsToUncenteredCoords(coords, size);

        for (int x = 0; x < size.x; x++)
        {
            for (int y = 0; y < size.y; y++)
            {
                Vector2Int coordsOffset = new Vector2Int(x, y);
                Vector2Int tileCoords = uncenteredCoords + coordsOffset;

                bool fillConditions = DoTileFillConditions(worldPosition, tileCoords, condition, entityID, logBuildError);

                //Debug.DrawRay(CoordsToWorld(tileCoords), Vector3.up * 5, fillConditions ? Color.green : Color.red, 3f);

                if (!fillConditions)
                    return false;
            }
        }

        return true;
    }

    private bool DoTileFillConditions(Vector3 worldPosition, Vector2Int tileCoords, TileFlag condition, bool logBuildError = false)
        => DoTileFillConditions(worldPosition, tileCoords, condition, string.Empty, logBuildError);

    private bool DoTileFillConditions(Vector3 worldPosition, Vector2Int tileCoords, TileFlag condition, string entityID, bool logBuildError = false)
    {
        if (condition.HasFlag(TileFlag.Free))
        {
            if (!IsTileFree(tileCoords))
            {
                if (logBuildError) UIMessagesLogger.Instance.AddErrorMessage("Can't on non-empty tile.");
                return false;
            }
        }

        if (condition.HasFlag(TileFlag.Visible))
        {
            Assert.IsNotNull(FOWManager.Instance, "FOWManager missing");

            Vector3 tileWorldPosition = CoordsToWorld(tileCoords);

            Debug.DrawRay(tileWorldPosition, Vector3.up * 3f, Color.magenta, 1f);

            if (!IsTileVisible(tileWorldPosition))
            {
                if (logBuildError) UIMessagesLogger.Instance.AddErrorMessage("Can't on non-visible tile.");
                return false;
            }
        }

        if (!string.IsNullOrEmpty(entityID))
        {
            if (GetTile(tileCoords) != null && !DoTileContainsEntityOfID(tileCoords, entityID))
            {
                if (logBuildError) UIMessagesLogger.Instance.AddErrorMessage(string.Format("Can't build on {0} building.", GetTileID(tileCoords)));
                return false;
            }
        }

        return true;
    }
    #endregion

    #region Get Tiles Methods
    public GameObject GetTile(Vector2Int coords)
    {
        if (_tiles.ContainsKey(coords))
        {
            return _tiles[coords];
        }

        return null;
    }

    public GameObject GetTile(Vector3 worldPosition)
        => GetTile(WorldToCoords(worldPosition));

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
    #endregion

    #region Set Tiles Methods    
    public bool TrySetTile(GameObject gameObject, Vector2Int buildingSize, TileFlag condition)
        => TrySetTile(gameObject, buildingSize, condition, string.Empty);

    public bool TrySetTile(GameObject gameObject, Vector2Int buildingSize, TileFlag condition, string entityID)
    {
        Vector3 worldPosition = gameObject.transform.position;

        bool tilesFree = DoTilesFillConditions(worldPosition, buildingSize, condition, entityID);

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

    public void SetTile(GameObject gameObject, Vector2Int buildingSize, bool canBuildOnNonEmptyCell = false)
        => SetTile(gameObject, buildingSize, WorldToCoords(gameObject.transform.position), canBuildOnNonEmptyCell);

    public void SetTile(GameObject gameObject, Vector2Int buildingSize, Vector2Int coords, bool canBuildOnNonEmptyCell = false)
    {
        Vector2Int uncenteredCoords = CoordsToUncenteredCoords(coords, buildingSize);

        for (int x = 0; x < buildingSize.x; x++)
        {
            for (int y = 0; y < buildingSize.y; y++)
            {
                Vector2Int coordsOffset = new Vector2Int(x, y);
                Vector2Int tileCoords = uncenteredCoords + coordsOffset;

                bool setTileSuccesful = TrySetTile(gameObject, tileCoords, canBuildOnNonEmptyCell);

                Assert.IsTrue(setTileSuccesful, "At this code section, 'setTileSuccessful' should always be true.");
            }
        }
    }

    private bool TrySetTile(GameObject gameObject, Vector2Int coords, bool canBuildOnNonEmptyCell = false)
    {
        if (_tiles.ContainsKey(coords) == false)
        {
            Debug.LogError("Can't set GameObject on Tile " + coords + ". It doesn't exist!");
            return false;
        }

        if (!canBuildOnNonEmptyCell && _tiles[coords] != null)
        {
            UIMessagesLogger.Instance.AddErrorMessage("You can't build on non-empty cell.");
            return false;
        }

        _tiles[coords] = gameObject;
        OnTileTerrainChanged?.Invoke(coords, gameObject);

        return true;
    }

    private void ResetAllTiles()
    {
        int midCellCount = GameManager.Instance.Grid.CellCount / 2;
        int cellCount = GameManager.Instance.Grid.CellCount;

        for (int x = 0; x <= cellCount; x++)
        {
            for (int y = 0; y <= cellCount; y++)
            {
                _tiles.Add(new Vector2Int(x, y), null);
            }
        }
    }
    #endregion

    #region Conversion methods
    private Vector2Int CoordsToUncenteredCoords(Vector2Int centeredCoords, Vector2Int size)
    {
        if (size.x % 2 != 0) size.x--;
        if (size.y % 2 != 0) size.y--;

        Vector2Int uncenteredCoords = centeredCoords - size / 2;

        return uncenteredCoords;
    }

    public Vector2Int WorldToCoords(Vector3 position)
        => GameManager.Instance.Grid.WorldToCoords(position);

    public Vector3 CoordsToWorld(Vector2Int coords)
        => GameManager.Instance.Grid.CoordsToWorldPosition(coords);
    #endregion

    #region Path Finding methods
    public Vector2Int[] GetPath(Vector3 from, Vector3 to)
    => GetPath(WorldToCoords(from), WorldToCoords(to));

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
    #endregion
}
