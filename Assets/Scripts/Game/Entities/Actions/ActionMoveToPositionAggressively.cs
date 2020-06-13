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
        private readonly EntityAttack _entityAttack;

        public ActionMoveToPositionAggressively(Entity owner, Vector3 position) : base(owner)
        {
            _position = position;
            _entityAttack = entity.GetCharacterComponent<EntityAttack>();
        }

        public override void OnStateEnter()
        {
            base.OnStateEnter();

            entity.GetCharacterComponent<EntityMovement>().MoveToPosition(_position);
            entity.GetCharacterComponent<EntityDetection>().OnOpponentEnterAttackRange += OnOpponentEnterAttackRange;

            _owner.GetCharacterComponent<EntityTransitiveStop>().EnableTransitiveStop();
        }

        public override void OnStateExit()
        {
            base.OnStateExit();

            entity.GetCharacterComponent<EntityMovement>().StopMoving();
            entity.GetCharacterComponent<EntityDetection>().OnOpponentEnterAttackRange -= OnOpponentEnterAttackRange;

            _owner.GetCharacterComponent<EntityTransitiveStop>().DisableTransitiveStop();
        }

        public override void Tick()
        {
            // Empty function but it's normal
        }

        private void OnOpponentEnterAttackRange(Entity entity)
        {
            var action = new ActionAttackEntity(_owner, entity, false);
            _owner.SetAction(action);

            // after the entity killed, continue to move to position
            _owner.SetAction(this, true);
        }

        public override string ToString()
        {
            return string.Format("{0} moves to {1} aggressively.", _owner.name, _position);
        }

        public override bool CanExecuteAction()
        {
            return _owner.Data.CanMove && _owner.Data.CanAttack;
        }
    }
}
