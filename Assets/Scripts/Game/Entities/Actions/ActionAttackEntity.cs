using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Entities.Actions
{
    /// <summary>
    /// This script make the Entity go closer to '_target'.
    /// Than, the Entity stop moving and attacks '_target'.
    /// </summary>
    public class ActionAttackEntity : Action
    {
        private readonly Entity _target;

        private readonly EntityMovement _entityMovement;
        private readonly EntityDetection _entityDetection;
        private readonly EntityAttack _entityAttack;

        public ActionAttackEntity(Entity owner, Entity target) : base(owner)
        {
            _target = target;

            _entityMovement = entity.GetCharacterComponent<EntityMovement>();
            _entityDetection = entity.GetCharacterComponent<EntityDetection>();
            _entityAttack = entity.GetCharacterComponent<EntityAttack>();
        }

        public override void OnStateExit()
        {
            entity.GetCharacterComponent<EntityMovement>().StopMoving();
        }

        public override void Tick()
        {
            // if target has been killed
            if (!_target.IsInstanciate)
            {
                TargetDeath();
                return;
            }

            // Can entity attack target?
            if (_entityDetection.IsEntityInAttackRange(_target))
            {
                _entityMovement.StopMoving();

                _entityAttack.DoAttack(_target);
            }
            else // Otherwise, entity go closer.
            {
                _entityMovement.MoveToEntity(_target);
            }
        }

        public override string ToString()
        {
            return string.Format("{0} attacks {1}.", _owner.name, _target.name);
        }

        public override bool CanExecuteAction()
        {
            return _target != null;
        }

        private void TargetDeath()
        {
            // try to auto attack nearest enemy
            bool attacksANewEnemy = entity.GetCharacterComponent<EntityAttack>().TryStartActionAttackNearestEnemy();

            // if not enemy nearest, stop current action
            if (!attacksANewEnemy)
            {
                // WARNING:
                // If we LeaveAction without checking if is attacking an enemy
                // We'll overwrite 'attack nearest action'.
                LeaveAction();
            }            
        }
    }
}
