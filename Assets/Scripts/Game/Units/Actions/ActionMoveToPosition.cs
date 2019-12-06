using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace LeonidasLegacy.IA.Action
{
    public class ActionMoveToPosition : Action
    {
        public ActionMoveToPosition(Unit owner, Vector3 position) : base(owner)
        {
            unitManager.GetCharacterComponent<UnitMovement>().MoveToPosition(position);
        }

        public override void OnStateExit()
        {
            unitManager.GetCharacterComponent<UnitMovement>().StopMoving();
        }

        public override void Tick()
        {
            if (unitManager.GetCharacterComponent<UnitMovement>().HasReachedDestination())
            {
                unitManager.GetCharacterComponent<UnitMovement>().StopMoving();
                unitManager.StopCurrentAction();
            }
        }
    }
}
