using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LeonidasLegacy.IA.Action
{
    public class ActionMoveToPositionAggressively : Action
    {
        public ActionMoveToPositionAggressively(Unit owner, Vector3 position) : base(owner)
        {
            owner.GetCharacterComponent<UnitMovement>().MoveToPosition(position);
        }

        public override void OnStateExit()
        {
            unitManager.GetCharacterComponent<UnitMovement>().StopMoving();
        }

        public override void Tick()
        {
            unitManager.GetCharacterComponent<UnitAttack>().StartActionAttackNearestEnemy();
        }
    }
}
