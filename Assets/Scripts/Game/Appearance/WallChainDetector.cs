using Game.Entities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Detect construction or deletion of neighboor on TileSystem.
/// 
/// If yes, it fire two events:
/// - OnWallOrientationChanged : WallOrientation is the wall direction.
/// - OnWallJointChanged : a wall joint is a wall in a corner or the end of a wall
/// </summary>
public class WallChainDetector : MonoBehaviour
{    
    public enum WallOrientation
    {
        NotAWallOrJoint = 0,
        NorthToSouth = 1,
        WestToEast = 2,
    }

    #region Fields
    public event Action<WallOrientation> OnWallOrientationChanged;
    public event Action<bool> OnWallJointChanged;

    [Header("IDS")]
    [SerializeField] private string[] _wallIDs = new string[] { "building_wall" }; // used on "IsWall" method

    private WallOrientation _currentWallOrientation;
    #endregion

    #region Properties
    public bool Cached_IsWallJoint { get => _currentWallOrientation == WallOrientation.NotAWallOrJoint; }
    public WallOrientation Cached_CurrentWallOrientation { get => _currentWallOrientation; }
    #endregion

    #region Methods
    #region MonoBehaviour Callbacks
    void OnEnable()
    {
        TileSystem.Instance.OnTileTerrainChanged += Instance_OnTileTerrainChanged;
    }

    void OnDisable()
    {
        if (TileSystem.Instance != null)
        {
            TileSystem.Instance.OnTileTerrainChanged -= Instance_OnTileTerrainChanged;
        }
    }
    #endregion

    #region Events Handlers
    void Instance_OnTileTerrainChanged(Vector2Int changedCoords, GameObject gameObjectAtCoords)
    {
        Vector2Int myCoords = TileSystem.Instance.WorldToCoords(transform.position);

        // only change appearance if tiles are neightboor
        if (TileSystem.Instance.DoTilesAreNeightboor(myCoords, changedCoords))
        {
            ForceCalculateWallOrientation();

            OnWallOrientationChanged?.Invoke(_currentWallOrientation);
            OnWallJointChanged?.Invoke(_currentWallOrientation == WallOrientation.NotAWallOrJoint);
        }
    }
    #endregion

    #region Private Methods
    public void ForceCalculateWallOrientation()
    {
        _currentWallOrientation = GetCurrentWallOrientation();
    }

    WallOrientation GetCurrentWallOrientation()
    {
        Assert.IsNotNull(TileSystem.Instance, "You must have a TileSystem to change appareance.");

        Vector2Int myCoords = TileSystem.Instance.WorldToCoords(transform.position);

        Vector2Int northCoords = new Vector2Int(myCoords.x, myCoords.y + 1);
        Vector2Int southCoords = new Vector2Int(myCoords.x, myCoords.y - 1);
        Vector2Int eastCoords = new Vector2Int(myCoords.x + 1, myCoords.y);
        Vector2Int westCoords = new Vector2Int(myCoords.x - 1, myCoords.y);

        bool hasNorthWall = IsWall(northCoords);
        bool hasSouthWall = IsWall(southCoords);
        bool hasEastWall = IsWall(eastCoords);
        bool hasWestWall = IsWall(westCoords);

        if (hasNorthWall && hasSouthWall && !hasEastWall && !hasWestWall) // N/S walls only
        {
            return WallOrientation.NorthToSouth;
        }
        else if (!hasNorthWall && !hasSouthWall && hasEastWall && hasWestWall) // E/W walls only
        {
            return WallOrientation.WestToEast;
        }
        else
        {
            return WallOrientation.NotAWallOrJoint;
        }
    }

    bool IsWall(Vector2Int coords)
    {
        Assert.IsNotNull(TileSystem.Instance, "You must have a TileSystem check if object at coords is a wall.");

        var tile = TileSystem.Instance.GetTile(coords);

        // not a wall if tile is empty
        if (tile == null)
            return false;

        if (tile.TryGetComponent(out Entity entity))
        {
            if (_wallIDs.Length == 0) Debug.LogWarningFormat("Wall IDS of {0} is empty!", name);

            return _wallIDs.Contains(entity.EntityID);
        }
        else
        {
            // not a wall if building doesn't have Entity on it.
            return false;
        }
    }
    #endregion
    #endregion
}
