using LeonidasLegacy.IA.Action;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EntityUnitSpawner : EntityComponent
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
        if (!Entity.Data.CanSpawnUnit) return;

        if (Entity.Data.AvailableUnitsForCreation.Contains(unitType) == false)
        {
            Debug.LogWarningFormat("Can't create {0} because it's not inside _creatableUnits of {1}.", unitType, name);
            return;
        }

        if (MainRegister.Instance.TryGetUnitData(unitType, out EntityData unitData))
        {
            if (GameManager.Instance.Resources.HasEnoughResources(unitData.SpawningCost) == false)
            {
                UIMessagesLogger.Instance.AddErrorMessage("You doesn't have enought resources to create " + unitType + ".");
                return;
            }

            // remove resources
            GameManager.Instance.Resources -= unitData.SpawningCost;

            // instantiate
            var instantiatedObject = Instantiate(unitData.Prefab, transform.position, Quaternion.identity);

            // make the entity go to the anchor position
            var entity = instantiatedObject.GetComponent<Entity>();
            Action moveToAnchorAction = new ActionMoveToPosition(entity, _anchorPosition);
            entity.SetAction(moveToAnchorAction);
        }
        else
        {
            Debug.LogErrorFormat("Entity Unit Spawn could find EntityData of {0}. Aborting method.", unitType);;
        }
    }
}
