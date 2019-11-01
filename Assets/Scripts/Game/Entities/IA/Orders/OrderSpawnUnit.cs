using Registers;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class OrderSpawnUnit : OwnedState<OrdersReceiver>
{
    public OrderSpawnUnit(OrdersReceiver owner, Unit unitType) : base(owner)
    {
        SpawnUnit(unitType);
    }

    public override void Tick()
    { }

    void SpawnUnit(Unit unitType)
    {
        EntityData unitData = UnitsRegister.Instance.GetItem(unitType).EntityData;

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

        GameObject prefab = UnitsRegister.Instance.GetItem(unitType).Prefab;
        OrdersReceiver orderReceiver = Object.Instantiate(prefab, _owner.Transform.position, Quaternion.identity).GetComponent<Entity>().OrdersReceiver;
        orderReceiver.Move(_owner.Transform.position + _owner.Transform.forward * 1);
    }
}
