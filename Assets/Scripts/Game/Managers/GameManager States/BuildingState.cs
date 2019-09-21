using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingState : OwnerState<GameManager>
{
    #region Fields
    private GameObject _currentBuilding = null;
    #endregion

    #region Methods
    public BuildingState(GameManager owner) : base(owner) { }

    public override void OnStateEnter()
    {
        SetCurrentBuilding(Building.Barracks);
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
            _owner.State = null;
        }
    }

    public void SetCurrentBuilding(Building building)
    {
        var prefab = BuildingsRegister.Instance.GetItem(building);

        _currentBuilding = Object.Instantiate(prefab);
        DynamicsObjects.Instance.SetToParent(_currentBuilding.transform, "Building");

        UpdateCurrentBuildingPosition();
    }

    void UpdateCurrentBuildingPosition()
    {
        Vector3? newPosition = GameManager.Instance.Grid.GetNearestPointFromMouse();

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
