using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Change model if the game object is a wall joint.
/// </summary>
public class WallAppearance : MonoBehaviour
{
    private enum WallOrientation
    {
        NotAWall = 0,
        NorthToSouth = 1,
        WestToEast = 2,
    }

    private const string debugLogHeader = "Wall Apperance : ";

    [Header("OBJECTS LINKING")]
    [SerializeField] private GameObject _jointModel;
    [SerializeField] private GameObject _wallModel;

    [Header("IDS")]
    [SerializeField] private string _wallID = "building_wall";

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
        Vector2Int myCoords = TileSystem.Instance.WorldToCoords(transform.position);

        // only change appearance if tiles are neightboor
        if (TileSystem.Instance.DoTilesAreNeightboor(myCoords, changedCoords))
        {
            ChangeAppearance();
        }
    }

    void ChangeAppearance()
    {
        Assert.IsNotNull(TileSystem.Instance, "You must have a TileSystem to change appareance.");

        bool isWallJoint = Calculate_IsWallJoint(out WallOrientation wallOrientation);

        _jointModel.SetActive(isWallJoint);
        _wallModel.SetActive(!isWallJoint);

        if (!isWallJoint)
        {
            Quaternion rotation = WallOrientationToRotation(wallOrientation);
            _wallModel.transform.rotation = rotation;
        }
    }

    bool Calculate_IsWallJoint(out WallOrientation wallOrientation)
    {
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
            wallOrientation = WallOrientation.NorthToSouth;
            return false;
        }
        else if (!hasNorthWall && !hasSouthWall && hasEastWall && hasWestWall) // E/W walls only
        {
            wallOrientation = WallOrientation.WestToEast;
            return false;
        }
        else
        {
            wallOrientation = WallOrientation.NotAWall;            
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
            if (entity.EntityID == _wallID)
                return true;
        }

        return false;
    }

    Quaternion WallOrientationToRotation(WallOrientation wallOrientation)
    {
        switch (wallOrientation)
        {
            case WallOrientation.NotAWall:                
            case WallOrientation.NorthToSouth:
                return Quaternion.Euler(0, 0, 0);

            case WallOrientation.WestToEast:
                return Quaternion.Euler(0, 90, 0);
            default:
                throw new System.NotImplementedException();
        }
    }
}
