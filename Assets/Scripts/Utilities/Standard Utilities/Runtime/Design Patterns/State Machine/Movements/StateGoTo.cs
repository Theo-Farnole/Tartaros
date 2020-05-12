using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lortedo.Utilities.Pattern
{
    public class StateGoTo : OwnedState<Transform>
    {
        public readonly float DISTANCE_THRESHOLD = 0.05f;

        private float _speed;
        private Vector3 _destination;

        public StateGoTo(Transform owner, float speed, Vector3 destination) : base(owner)
        {
            _speed = speed;
            _destination = destination;
        }

        public override void Tick()
        {
            _owner.position = Vector3.MoveTowards(_owner.position, _destination, _speed * Time.deltaTime);

            if (Vector3.Distance(_owner.position, _destination) <= DISTANCE_THRESHOLD)
            {
                _owner.GetComponent<IDestinationReached>()?.OnDestinationReached();
            }
        }
    }
}
