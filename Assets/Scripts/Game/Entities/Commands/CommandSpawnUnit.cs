using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CommandSpawnUnit : OwnerState<CommandsReceiverEntity>
{
    public CommandSpawnUnit(CommandsReceiverEntity owner, Unit unitType) : base(owner)
    {
        SpawnUnit(unitType);
    }

    public override void Tick()
    { }

    void SpawnUnit(Unit unitType)
    {
        UnitCreationData unitData = _owner.CreatableUnits.FirstOrDefault(x => x.Type == unitType);

        if (unitData == null)
        {
            Debug.LogWarning("Can't create " + unitType + " because it's not inside _creatableUnits of " + _owner.name + ".");
            return;
        }

        if (GameManager.Instance.Resources < unitData.Cost)
        {
            Debug.Log("Player doesn't have enought resources to create " + unitType + ".");
            return;
        }

        GameManager.Instance.Resources -= unitData.Cost;

        var prefab = UnitsPrefabRegister.Instance.GetItem(unitType);
        CommandsReceiverEntity commandReceiver = GameObject.Instantiate(prefab, _owner.transform.position, Quaternion.identity).GetComponent<CommandsReceiverEntity>();
        commandReceiver.Move(_owner.transform.position + _owner.transform.forward * 1);
    }
}
