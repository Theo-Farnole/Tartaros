using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.IA.Action
{
    public class ActionPatrol : Action
    {
        private Vector3 _targetPosition;
        private Vector3 _startingPosition;

        private bool _goToTarget = true;

        public ActionPatrol(Entity owner, Vector3 targetPosition) : base(owner)
        {
            _startingPosition = owner.transform.position;
            _targetPosition = targetPosition;
        }

        public override void OnStateExit()
        {
            entity.GetCharacterComponent<EntityMovement>().StopMoving();
        }

        public override void OnStateEnter()
        {
            base.OnStateEnter();

            MoveToPosition(true);
        }

        public override void Tick()
        {
            if (entity.GetCharacterComponent<EntityMovement>().HasReachedDestination())
            {
                // switch destination
                _goToTarget = !_goToTarget;
                MoveToPosition(_goToTarget);
            }
        }

        private void MoveToPosition(bool goToTarget)
        {
            Vector3 position = goToTarget ? _targetPosition : _startingPosition;

            entity.GetCharacterComponent<EntityMovement>().MoveToPosition(position);
        }

        public override string ToString()
        {
            return string.Format("{0} patrols between {1} and {2}.", _owner.name, _startingPosition, _targetPosition);
        }

        public override bool CanExecuteAction()
        {
            return _owner.Data.CanMove;
        }
    }
}
