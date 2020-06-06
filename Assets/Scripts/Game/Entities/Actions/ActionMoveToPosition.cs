using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Game.Entities.Actions
{
    public class ActionMoveToPosition : Action
    {
        private readonly Vector3 _position;
        private readonly EntityMovement _entityMovement;

        public ActionMoveToPosition(Entity owner, Vector3 position) : base(owner)
        {
            _position = position;
            _entityMovement = entity.GetCharacterComponent<EntityMovement>();
        }

        public override void OnStateExit()
        {
            _entityMovement.StopMoving();
        }

        public override void OnStateEnter()
        {
            base.OnStateEnter();

            _entityMovement.MoveToPosition(_position);
        }

        public override void Tick()
        {

            if (_entityMovement.HasReachedDestination())
            {
                _entityMovement.StopMoving();
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
