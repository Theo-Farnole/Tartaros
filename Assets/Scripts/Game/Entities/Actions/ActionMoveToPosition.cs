using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Game.IA.Action
{
    public class ActionMoveToPosition : Action
    {
        private Vector3 _position;

        public ActionMoveToPosition(Entity owner, Vector3 position) : base(owner)
        {
            _position = position;
        }

        public override void OnStateExit()
        {
            entity.GetCharacterComponent<EntityMovement>().StopMoving();
        }

        public override void OnStateEnter()
        {
            base.OnStateEnter();

            entity.GetCharacterComponent<EntityMovement>().MoveToPosition(_position); 
        }

        public override void Tick()
        {
            if (entity.GetCharacterComponent<EntityMovement>().HasReachedDestination())
            {
                entity.GetCharacterComponent<EntityMovement>().StopMoving();
                entity.StopCurrentAction();
            }
        }

        public override string ToString()
        {
            return string.Format("{0} moves to {1}.", _owner.name, _position);
        }

        public override bool CanExecuteAction()
        {
            return entity.Data.CanMove;
        }
    }
}
