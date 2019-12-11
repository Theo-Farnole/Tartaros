using Lortedo.Utilities.Pattern;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommandPattern
{
    public class CreateUnitCommand : Command
    {
        private UnitType _unit;

        public CreateUnitCommand(UnitType unit)
        {
            _unit = unit;
        }

        public override void Execute()
        {
            SelectedGroupsActionsCaller.OrderSpawnUnits(_unit);
        }
    }
}