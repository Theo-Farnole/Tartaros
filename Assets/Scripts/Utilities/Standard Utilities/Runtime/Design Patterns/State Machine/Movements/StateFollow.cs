using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lortedo.Utilities.Pattern
{
    public class StateFollow : OwnedState<Transform>
    {
        public readonly float DISTANCE_THRESHOLD = 0.05f;

        private float _speed;
        private Transform _target;

        public StateFollow(Transform owner, float speed, Transform target) : base(owner)
        {
            _speed = speed;
            _target = target;
        }

        public override void Tick()
        {
            _owner.position = Vector3.MoveTowards(_owner.position, _target.position, _speed * Time.deltaTime);

            if (Vector3.Distance(_owner.position, _target.position) <= DISTANCE_THRESHOLD)
            {
                _owner.GetComponent<IDestinationReached>()?.OnDestinationReached();
            }
        }
    }
}
