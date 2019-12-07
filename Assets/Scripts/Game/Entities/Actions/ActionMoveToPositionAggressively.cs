using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LeonidasLegacy.IA.Action
{
    public class ActionMoveToPositionAggressively : Action
    {
        public ActionMoveToPositionAggressively(Entity owner, Vector3 position) : base(owner)
        {
            owner.GetCharacterComponent<EntityMovement>().MoveToPosition(position);
        }

        public override void OnStateExit()
        {
            entity.GetCharacterComponent<EntityMovement>().StopMoving();
        }

        public override void Tick()
        {
            entity.GetCharacterComponent<EntityAttack>().StartActionAttackNearestEnemy();
        }
    }
}
