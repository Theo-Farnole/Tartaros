using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommandPattern
{
    public class CreateUnitCommand : Command
    {
        private Unit _unit;

        public CreateUnitCommand(Unit unit)
        {
            _unit = unit;
        }

        public override void Execute()
        {
            OrdersGiverManager.Instance.OrderSpawnUnits(_unit);
        }
    }
}