using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace LeonidasLegacy.IA.Action
{
    public class ActionMoveToPosition : Action
    {
        private Vector3 _position;

        public ActionMoveToPosition(Unit owner, Vector3 position) : base(owner)
        {
            _position = position;
        }

        public override void OnStateExit()
        {
            unitManager.GetCharacterComponent<UnitMovement>().StopMoving();
        }

        public override void OnStateEnter()
        {
            unitManager.GetCharacterComponent<UnitMovement>().MoveToPosition(_position); 
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
