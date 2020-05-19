using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Change model if the game object is a wall joint.
/// </summary>
public class WallAppearance : MonoBehaviour
{
    private const string debugLogHeader = "Wall Apperance : ";

    [SerializeField] private GameObject _jointModel;
    [SerializeField] private GameObject _wallModel;

    void OnEnable()
    {
        if (TileSystem.Instance != null)
        {
            TileSystem.Instance.OnTileTerrainChanged += Instance_OnTileTerrainChanged;            
        }
        else
        {
            Debug.LogErrorFormat(debugLogHeader + "TileSystem is missing. Can't update wall apperance.");
        }

        ChangeAppearance();
    }

    void OnDisable()
    {
        if (TileSystem.Instance != null)
        {
            TileSystem.Instance.OnTileTerrainChanged -= Instance_OnTileTerrainChanged;
        }
    }

    void Instance_OnTileTerrainChanged(Vector2Int changedCoords, GameObject gameObjectAtCoords)
    {
        Vector2Int myCoords = TileSystem.Instance.WorldPositionToCoords(transform.position);

        // only change appearance if tiles are neightboor
        if (TileSystem.Instance.DoTilesAreNeightboor(myCoords, changedCoords))
        {
            ChangeAppearance();
        }
    }

    void ChangeAppearance()
    {
        Assert.IsNotNull(TileSystem.Instance, "You must have a TileSystem to change appareance.");

        bool isWallJoint = Calculate_IsWallJoint();

        _jointModel.SetActive(isWallJoint);
        _wallModel.SetActive(!isWallJoint);

        Debug.LogFormat(debugLogHeader + " changing appearance of {1} to {0}.", isWallJoint ? "joint" : "wall", name);
    }

    bool Calculate_IsWallJoint()
    {
        Vector2Int myCoords = TileSystem.Instance.WorldPositionToCoords(transform.position);

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
            return false;
        }
        else if (!hasNorthWall && !hasSouthWall && hasEastWall && hasWestWall) // E/W walls only
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    bool IsWall(Vector2Int coords)
    {
        Assert.IsNotNull(TileSystem.Instance, "You must have a TileSystem check if object at coords is a wall.");

        var tile = TileSystem.Instance.GetTile(coords);

        if (tile == null)
            return false;

        if (tile.TryGetComponent(out Entity entity))
        {
            if (entity.Type == EntityType.Wall)
                return true;
        }

        return false;
    }
}
