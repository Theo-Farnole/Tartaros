using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Game.Entities.Actions
{
    /// <summary>
    /// Walk to the _position until an enemy is seen. If enemy's seen, attack it.
    /// After the enemy's death, go back walking to '_position'.
    /// </summary>
    public class ActionMoveToPositionAggressively : Action
    {
        private readonly Vector3 _position;

        public ActionMoveToPositionAggressively(Entity owner, Vector3 position) : base(owner)
        {
            _position = position;
        }

        public override void OnStateEnter()
        {
            base.OnStateEnter();

            entity.GetCharacterComponent<EntityMovement>().MoveToPosition(_position);
        }

        public override void OnStateExit()
        {
            entity.GetCharacterComponent<EntityMovement>().StopMoving();
        }

        public override void Tick()
        {
            bool hasFindEnemyToAttack = entity.GetCharacterComponent<EntityAttack>().TryStartActionAttackNearestEnemy();

            if (hasFindEnemyToAttack)
            {
                _owner.SetAction(this, true);
            }
        }

        public override string ToString()
        {
            return string.Format("{0} moves to {1} aggressively.", _owner.name, _position);
        }

        public override bool CanExecuteAction()
        {
            return _owner.Data.CanMove || _owner.Data.CanAttack;
        }
    }
}
