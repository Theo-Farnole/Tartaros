using Lortedo.Utilities.Pattern;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommandPattern
{
    public class CreateUnitCommand : Command
    {
        private string _unitID;

        public CreateUnitCommand(string unitID)
        {
            _unitID = unitID;
        }

        public override void Execute()
        {
            SelectedGroupsActionsCaller.OrderSpawnUnits(_unitID);
        }
    }
}