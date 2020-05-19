using Lortedo.Utilities.Debugging;
using Lortedo.Utilities.Pattern;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BuildingState : OwnedState<GameManager>
{
    #region Fields
    public readonly static int TERRAIN_LAYERMASK = LayerMask.GetMask("Terrain");

    private GameObject _building = null;
    private BuildingType _buildingType;
    private EntityData _buildingData;

    private bool _sucessfulBuild = false;
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


    public override void OnStateEnter()
    {
        _owner.Invoke_OnStartBuild();
    }

    public override void OnStateExit()
    {
        if (!_sucessfulBuild)
        {
            DestroyAndRefundBuilding();
        }

        _owner.Invoke_OnStopBuild();
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
            ConstructBuilding();
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
        if (GameManager.Instance.Grid.GetNearestPositionFromMouse(out Vector3 newPosition, TERRAIN_LAYERMASK))
        {
            if (newPosition != _building.transform.position)
            {
                _building.transform.position = newPosition;
                _building.GetComponent<EntityResourcesGeneration>().CalculateResourcesPerTick();
            }
        }
        else
        {
            Debug.LogWarningFormat("Building State : Can't find nearest position from mouse. We can't update the building position.");
        }
    }

    void ConstructBuilding()
    {
        // register tile
        bool successfulSetTile = TileSystem.Instance.TrySetTile(_building);

        if (!successfulSetTile)
            return;

        EnableBuildingComponents(true);

        _building.GetComponent<Entity>().Team = Team.Sparta;

        DynamicsObjects.Instance.SetToParent(_building.transform, "Building");

        // then leave
        _sucessfulBuild = true;
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

        if (_building.TryGetComponent(out EntityResourcesGeneration resourcesGeneration))
            resourcesGeneration.EnableResourceProduction = enabled;

        if (_building.TryGetComponent(out Entity entity))
            entity.enabled = enabled; // disable OnSpawn call

        if (_building.TryGetComponent(out EntityFogCoverable entityFogCoverable))
            entityFogCoverable.enabled = enabled;

        WallAppearance wallAppearence = _building.GetComponentInChildren<WallAppearance>();        
        if (wallAppearence)
            wallAppearence.enabled = enabled;
    }
    #endregion
}
