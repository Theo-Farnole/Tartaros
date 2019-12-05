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
    private GameObject _currentBuilding = null;
    private Building _currentBuildingType;
    #endregion

    #region Properties
    private ResourcesWrapper CurrentBuildingCost { get => BuildingsRegister.Instance.GetItem(_currentBuildingType).EntityData.SpawningCost; }
    #endregion

    #region Methods
    public BuildingState(GameManager owner, Building buildingType) : base(owner)
    {
        SetCurrentBuilding(buildingType);
    }

    public override void Tick()
    {
        UpdateCurrentBuildingPosition();

        if (Input.GetMouseButtonDown(0))
        {
            Vector2Int coords = TileSystem.Instance.WorldPositionToCoords(_currentBuilding.transform.position);
            GameObject tile = TileSystem.Instance.GetTile(coords);

            if (tile == null)
            {
                ConstructCurrentBuilding(coords);
            }
            else
            {
                UIMessagesLogger.Instance.AddErrorMessage("Can't build on non-empty tile");
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            DestroyCurrentBuilding();
            _owner.State = null;
        }
    }

    public void SetCurrentBuilding(Building building)
    {
        _currentBuildingType = building;

        if (_owner.Resources <= CurrentBuildingCost)
        {
            UIMessagesLogger.Instance.AddErrorMessage("GameManager doesn't have enought resources to build " + building);
            _owner.State = null;
            return;
        }

        _owner.Resources -= CurrentBuildingCost;

        GameObject prefab = BuildingsRegister.Instance.GetItem(building).Prefab;

        _currentBuilding = Object.Instantiate(prefab);
        EnableComponents(false);

        UpdateCurrentBuildingPosition();
    }

    void UpdateCurrentBuildingPosition()
    {
        Vector3? newPosition = GameManager.Instance.Grid.GetNearestPositionFromMouse();

        if (newPosition != null)
        {
            _currentBuilding.transform.position = (Vector3)newPosition;
        }
    }

    void ConstructCurrentBuilding(Vector2Int coords)
    {
        EnableComponents(true);
        _currentBuilding.GetComponent<OrdersReceiver>().StartCreatingResources();
        _currentBuilding.GetComponent<Entity>().owner = Owner.Sparta;
        DynamicsObjects.Instance.SetToParent(_currentBuilding.transform, "Building");

        TileSystem.Instance.SetTile(coords, _currentBuilding);

        // then leave
        _owner.State = null;
    }

    void DestroyCurrentBuilding()
    {
        Object.Destroy(_currentBuilding.gameObject);
        _owner.Resources += CurrentBuildingCost;
    }

    void EnableComponents(bool enabled)
    {
        var fowEntity = _currentBuilding.GetComponent<FogOfWar.FOWEntity>();
        if (fowEntity) fowEntity.enabled = enabled;

        var collider = _currentBuilding.GetComponent<Collider>();
        if (collider) collider.enabled = enabled;

        var navMeshAgent = _currentBuilding.GetComponent<NavMeshAgent>();
        if (navMeshAgent) navMeshAgent.enabled = enabled;

        var navMeshObstacle = _currentBuilding.GetComponent<NavMeshObstacle>();
        if (navMeshObstacle) navMeshObstacle.enabled = enabled;
    }
    #endregion
}
