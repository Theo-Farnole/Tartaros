using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LeonidasLegacy.IA.Action
{
    public class ActionAttackEntity : Action
    {
        Entity _target;

        public ActionAttackEntity(Entity owner, Entity target) : base(owner)
        {
            _target = target;
        }

        public override void OnStateExit()
        {
            entity.GetCharacterComponent<EntityMovement>().StopMoving();
        }

        public override void Tick()
        {
            // if target has been killed
            if (_target == null)
            {
                entity.GetCharacterComponent<EntityMovement>().StopMoving();
                entity.StopCurrentAction();
                return;
            }

            EntityMovement entityMovement = entity.GetCharacterComponent<EntityMovement>();
            EntityDetection entityDetection = entity.GetCharacterComponent<EntityDetection>();

            // Can entity attack target?
            if (entityDetection.IsEntityInAttackRange(_target))
            {
                entityMovement.StopMoving();

                EntityAttack entityAttack = entity.GetCharacterComponent<EntityAttack>();
                entityAttack.DoAttack(_target);
            }
            else // Otherwise, entity go closer.
            {
                entityMovement.MoveToEntity(_target);
            }
        }
    }
}
