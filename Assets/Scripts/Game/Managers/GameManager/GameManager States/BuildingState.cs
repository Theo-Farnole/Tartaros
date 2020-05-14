using Lortedo.Utilities.Debugging;
using Lortedo.Utilities.Pattern;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BuildingState : OwnedState<GameManager>
{
    #region Fields
    private GameObject _building = null;
    private BuildingType _buildingType;
    private EntityData _buildingData;
    #endregion

    #region Properties
    private ResourcesWrapper CurrentBuildingCost
    {
        get
        {
            if (_buildingData == null)
                Debug.LogFormat("Can't get CurrentBuildingCost because _buildingData is null!");

            return _buildingData.SpawningCost;
        }
    }
    #endregion

    #region Methods
    public BuildingState(GameManager owner, BuildingType buildingType) : base(owner)
    {
        SetCurrentBuilding(buildingType);
    }


    public override void Tick()
    {
        UpdateBuildingPosition();

        ProcessInputs();
    }

    private void ProcessInputs()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            DestroyAndRefundBuilding();
            _owner.State = null;
        }

        if (Input.GetMouseButtonDown(0))
        {
            TryConstructBuilding();
        }
    }

    void TryConstructBuilding()
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

    void SetCurrentBuilding(BuildingType buildingType)
    {
        // try to get prefab for instantiation
        if (MainRegister.Instance.TryGetBuildingData(buildingType, out EntityData buildingData))
        {
            _buildingData = buildingData;
            _buildingType = buildingType;
            _owner.Resources -= CurrentBuildingCost;

            // get prefab then instantiate
            GameObject prefab = buildingData.Prefab;
            _building = Object.Instantiate(prefab);

            EnableBuildingComponents(false);
            UpdateBuildingPosition();
        }
        else
        {
            Debug.LogErrorFormat("Building State : can't SetCurrentBuilding because cannot get building data from MainRegister of {0}.", buildingType);
        }
    }

    void UpdateBuildingPosition()
    {
        if (GameManager.Instance.Grid.GetNearestPositionFromMouse(out Vector3 newPosition))
        {
            _building.transform.position = newPosition;        
        }
        else
        {
            Debug.LogWarningFormat("Building State : Can't find nearest position from mouse. We can't update the building position.");
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
