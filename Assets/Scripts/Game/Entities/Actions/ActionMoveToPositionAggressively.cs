using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LeonidasLegacy.IA.Action
{
    public class ActionMoveToPositionAggressively : Action
    {
        private Vector3 _position;

        public ActionMoveToPositionAggressively(Entity owner, Vector3 position) : base(owner)
        {
            _position = position;
        }

        public override void OnStateEnter()
        {
            entity.GetCharacterComponent<EntityMovement>().MoveToPosition(_position);
        }

        public override void OnStateExit()
        {
            entity.GetCharacterComponent<EntityMovement>().StopMoving();
        }

        public override void Tick()
        {
            entity.GetCharacterComponent<EntityAttack>().TryStartActionAttackNearestEnemy();
        }
    }
}
