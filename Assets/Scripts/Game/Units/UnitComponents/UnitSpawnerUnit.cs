using LeonidasLegacy.IA.Action;
using Registers;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitSpawnerUnit : UnitComponent
{
    private Vector3 _anchorPosition;

    void Start()
    {
        _anchorPosition = transform.position + transform.forward * 1f;    
    }

    public void SetAnchorPosition(Vector3 anchorPosition)
    {
        _anchorPosition = anchorPosition;
    }

    public void SpawnUnit(UnitType unitType)
    {
        if (UnitManager.Data.AvailableUnitsForCreation.Contains(unitType) == false)
        {
            Debug.LogWarningFormat("Can't create {0} because it's not inside _creatableUnits of {1}.", unitType, name);
            return;
        }

        var unitData = UnitsRegister.Instance.GetItem(unitType);

        if (GameManager.Instance.Resources.HasEnoughResources(unitData.EntityData.SpawningCost) == false)
        {
            UIMessagesLogger.Instance.AddErrorMessage("You doesn't have enought resources to create " + unitType + ".");
            return;
        }

        // remove resources
        GameManager.Instance.Resources -= unitData.EntityData.SpawningCost;

        // instantiate
        var instantiatedObject = Object.Instantiate(unitData.Prefab, transform.position, Quaternion.identity);

        // set action
        var unit = instantiatedObject.GetComponent<Unit>();
        Action moveToAnchorAction = new ActionMoveToPosition(unit, _anchorPosition);
        unit.SetAction(moveToAnchorAction);
    }
}
