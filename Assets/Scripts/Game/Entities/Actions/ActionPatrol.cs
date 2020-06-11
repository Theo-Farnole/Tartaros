using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Entities.Actions
{
    /// <summary>
    /// The Entity walks between '_targetPosition' & '_startingPosition'.
    /// </summary>
    public class ActionPatrol : Action
    {
        private readonly Vector3 _targetPosition;
        private readonly Vector3 _startingPosition;

        private readonly EntityMovement _entityMovement;

        private bool _goToTarget = true;

        public ActionPatrol(Entity owner, Vector3 targetPosition) : base(owner)
        {
            _startingPosition = owner.transform.position;
            _targetPosition = targetPosition;

            _entityMovement = entity.GetCharacterComponent<EntityMovement>();
        }

        public override void OnStateEnter()
        {
            base.OnStateEnter();

            MoveToPosition(true);

            _owner.GetCharacterComponent<EntityTransitiveStop>().EnableTransitiveStop();
        }

        public override void OnStateExit()
        {
            _entityMovement.StopMoving();
            _owner.GetCharacterComponent<EntityTransitiveStop>().DisableTransitiveStop();
        }

        public override void Tick()
        {
            if (_entityMovement.HasReachedDestination())
            {
                // switch destination
                _goToTarget = !_goToTarget;
                MoveToPosition(_goToTarget);
            }
        }

        private void MoveToPosition(bool goToTarget)
        {
            Vector3 position = goToTarget ? _targetPosition : _startingPosition;

            _entityMovement.MoveToPosition(position);
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
