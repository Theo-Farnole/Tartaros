using Registers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingState : OwnerState<GameManager>
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
        if (_currentBuilding == null)
            return;

        UpdateCurrentBuildingPosition();

        if (Input.GetMouseButtonDown(0))
        {
            Vector2Int coords = TileSystem.Instance.WorldPositionToCoords(_currentBuilding.transform.position);
            GameObject tile = TileSystem.Instance.GetTile(coords);

            if (tile == null)
            {
                CreateCurrentBuilding(coords);
            }
            else
            {
                Debug.LogWarning("Can't build on non-empty tile");
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Object.Destroy(_currentBuilding.gameObject);
            _owner.Resources += CurrentBuildingCost;
            _owner.State = null;
        }
    }

    public void SetCurrentBuilding(Building building)
    {
        _currentBuildingType = building;

        if (_owner.Resources <= CurrentBuildingCost)
        {
            Debug.LogWarning("GameManager doesn't have enought resources to build " + building);
            _owner.State = null;
            return;
        }

        _owner.Resources -= CurrentBuildingCost;

        GameObject prefab = BuildingsRegister.Instance.GetItem(building).Prefab;

        _currentBuilding = Object.Instantiate(prefab);
        DynamicsObjects.Instance.SetToParent(_currentBuilding.transform, "Building");

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

    void CreateCurrentBuilding(Vector2Int coords)
    {
        TileSystem.Instance.SetTile(coords, _currentBuilding);

        // then leave
        _owner.State = null;
    }
    #endregion
}
