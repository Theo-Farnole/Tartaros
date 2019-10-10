using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StateSpawnUnit : OwnerState<CommandsReceiver>
{
    public StateSpawnUnit(CommandsReceiver owner, Unit unitType) : base(owner)
    {
        SpawnUnit(unitType);
    }

    public override void Tick()
    { }

    void SpawnUnit(Unit unitType)
    {
        EntityData unitData = UnitsDataRegister.Instance.GetItem(unitType);

        if (unitData == null)
        {
            Debug.LogWarning("Can't create " + unitType + " because it's not inside _creatableUnits of " + _owner.Entity.name + ".");
            return;
        }

        if (GameManager.Instance.Resources.HasEnoughtResources(unitData.SpawningCost) == false)
        {
            Debug.Log("Player doesn't have enought resources to create " + unitType + ".");
            return;
        }

        GameManager.Instance.Resources -= unitData.SpawningCost;

        var prefab = UnitsPrefabRegister.Instance.GetItem(unitType);
        CommandsReceiver commandReceiver = Object.Instantiate(prefab, _owner.Transform.position, Quaternion.identity).GetComponent<Entity>().CommandReceiver;
        commandReceiver.Move(_owner.Transform.position + _owner.Transform.forward * 1);
    }
}
