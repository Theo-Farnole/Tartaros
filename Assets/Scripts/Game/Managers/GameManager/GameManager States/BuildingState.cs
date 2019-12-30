using Lortedo.Utilities.Debugging;
using Lortedo.Utilities.Pattern;
using Registers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BuildingState : OwnedState<GameManager>
{
    #region Fields
    private GameObject _building = null;
    private BuildingType _buildingType;
    #endregion

    #region Properties
    private ResourcesWrapper CurrentBuildingCost { get => BuildingsRegister.Instance.GetItem(_buildingType).EntityData.SpawningCost; }
    #endregion

    #region Methods
    public BuildingState(GameManager owner, BuildingType buildingType) : base(owner)
    {
        SetCurrentBuilding(buildingType);
    }

    public override void Tick()
    {
        UpdateBuildingPosition();

        // Can we build ?
        if (Input.GetMouseButtonDown(0))
        {
            Vector2Int coords = _owner.TileSystem.WorldPositionToCoords(_building.transform.position);
            GameObject tile = _owner.TileSystem.GetTile(coords);

            // Is tile where we want to build is free ?
            if (tile == null)
            {
                ConstructBuilding(coords);
            }
            else
            {
                UIMessagesLogger.Instance.AddErrorMessage("Can't build on non-empty tile");
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            DestroyAndRefundBuilding();

            _owner.State = null;
        }
    }

    void SetCurrentBuilding(BuildingType building)
    {
        _buildingType = building;
        _owner.Resources -= CurrentBuildingCost;
                 
        // get prefab then instantiate
        GameObject prefab = BuildingsRegister.Instance.GetItem(building).Prefab;
        _building = Object.Instantiate(prefab);

        EnableBuildingComponents(false);
        UpdateBuildingPosition();
    }

    void UpdateBuildingPosition()
    {
        Vector3? newPosition = GameManager.Instance.Grid.GetNearestPositionFromMouse();

        if (newPosition != null)
        {
            _building.transform.position = (Vector3)newPosition;
        }
    }

    void ConstructBuilding(Vector2Int coords)
    {
        EnableBuildingComponents(true);
        
        _building.GetComponent<Entity>().Team = Team.Sparta;

        DynamicsObjects.Instance.SetToParent(_building.transform, "Building");

        // register tile
        _owner.TileSystem.SetTile(coords, _building);

        // then leave
        _owner.State = null;
    }

    void DestroyAndRefundBuilding()
    {
        Object.Destroy(_building.gameObject);
        _owner.Resources += CurrentBuildingCost;
    }

    void EnableBuildingComponents(bool enabled)
    {
        var fowEntity = _building.GetComponent<EntityFogVision>();
        if (fowEntity) fowEntity.enabled = enabled;

        var collider = _building.GetComponent<Collider>();
        if (collider) collider.enabled = enabled;

        var navMeshAgent = _building.GetComponent<NavMeshAgent>();
        if (navMeshAgent) navMeshAgent.enabled = enabled;

        var navMeshObstacle = _building.GetComponent<NavMeshObstacle>();
        if (navMeshObstacle) navMeshObstacle.enabled = enabled;
    }
    #endregion
}
