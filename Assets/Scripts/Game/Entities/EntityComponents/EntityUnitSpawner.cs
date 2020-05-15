using LeonidasLegacy.IA.Action;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EntityUnitSpawner : EntityComponent
{
    private static readonly string debugLogHeader = "Entity Unit Spawn : ";

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
            Debug.LogWarningFormat(debugLogHeader + "Can't create {0} because it's not inside _creatableUnits of {1}.", unitType, name);
            return;
        }

        if (MainRegister.Instance.TryGetUnitData(unitType, out EntityData unitData))
        {
            if (GameManager.Instance != null)
            {
                if (!GameManager.Instance.HasEnoughtPopulationToSpawn(unitData))
                {
                    UIMessagesLogger.Instance.AddErrorMessage(string.Format("Build more house to create {0} unit.", unitType));
                    return;
                }
            }
            else
            {
                Debug.LogErrorFormat(debugLogHeader + " Missing GameManager. Can't prevent spawning if max population reached.");
            }

            if (GameManager.Instance.Resources.HasEnoughResources(unitData.SpawningCost) == false)
            {
                UIMessagesLogger.Instance.AddErrorMessage("You doesn't have enought resources to create " + unitType + ".");
                return;
            }

            // remove resources
            GameManager.Instance.Resources -= unitData.SpawningCost;
            
            var instantiatedObject = Instantiate(unitData.Prefab, transform.position, Quaternion.identity);            
            MoveGameObjectToAnchor(instantiatedObject);
        }
        else
        {
            Debug.LogErrorFormat(debugLogHeader + "Entity Unit Spawn could find EntityData of {0}. Aborting method.", unitType); ;
        }
    }

    private void MoveGameObjectToAnchor(GameObject instantiatedObject)
    {
        if (instantiatedObject.TryGetComponent(out Entity entity))
        {
            Action moveToAnchorAction = new ActionMoveToPosition(entity, _anchorPosition);
            entity.SetAction(moveToAnchorAction);
        }
        else
        {
            Debug.LogErrorFormat(debugLogHeader + "Can't make {0} go to anchor point, because it has not Entity component.", name);
        }
    }
}
