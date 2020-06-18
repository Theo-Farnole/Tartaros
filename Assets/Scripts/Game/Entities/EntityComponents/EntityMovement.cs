namespace Game.Entities
{
    using Game.Entities.Actions;
    using System;
    using UnityEngine;
    using UnityEngine.AI;

    /// <summary>
    /// This script manage the moving of entity.
    /// </summary>
    public partial class EntityMovement : AbstractEntityComponent
    {
        #region Fields
        private const string debugLogHeader = "Entity Movement : ";
        private const float reachDestinationThreshold = 0.5f;

        private Vector3 _destination;
        private bool _hasReachedDestination = false;

        // cache variable
        private NavMeshAgent _navMeshAgent;
        private CapsuleCollider _collider;
        #endregion

        #region Events
        public event Action<Vector3> DestinationReached;
        public event Action<Vector3> StartMove;
        /// <summary>
        /// Called on DestinationReached and on StopMove();
        /// </summary>
        public event System.Action MovementStopped;
        #endregion

        #region Properties
        public Vector3 Destination { get => _destination; }
        #endregion

        #region Methods
        #region Mono Callbacks
        void Awake()
        {
            _navMeshAgent = GetComponent<NavMeshAgent>();
            _collider = GetComponent<CapsuleCollider>();
        }

        void Start()
        {
            if (_navMeshAgent != null)
            {
                _navMeshAgent.speed = Entity.Data.Speed;
            }
        }

        void Update()
        {
            if (!Entity.Data.CanMove)
                return;

            if (_navMeshAgent.isStopped)
                return;

            bool oldHasReachedDestination = _hasReachedDestination;
            _hasReachedDestination = HasReachedDestination();

            // has just reached destination ?
            if (_hasReachedDestination && !oldHasReachedDestination)
            {
                _navMeshAgent.isStopped = true;

                DestinationReached?.Invoke(_destination);
                MovementStopped?.Invoke();
            }
        }
        #endregion

        #region Public methods
        public void SetAvoidance(Avoidance avoidance)
        {
            if (!Entity.Data.CanMove)
                return;

            _navMeshAgent.avoidancePriority = avoidance.ToPriority();
        }

        public void MoveToEntity(Entity target)
        {
            if (!Entity.Data.CanMove)
                return;

            MoveToPosition(target.transform.position);
        }

        public void MoveToPosition(Vector3 position)
        {
            if (!Entity.Data.CanMove)
                return;

            _destination = position;

            SetAvoidance(Avoidance.Move);

            if (_navMeshAgent.isStopped)
            {
                StartMove?.Invoke(position);
            }

            _navMeshAgent.isStopped = false;
            _navMeshAgent.SetDestination(position);
        }

        public void StopMoving()
        {
            if (!Entity.Data.CanMove)
                return;

            SetAvoidance(Avoidance.Idle);

            if (!_navMeshAgent.isStopped)
            {
                MovementStopped?.Invoke();
            }

            _navMeshAgent.isStopped = true;
        }


        public bool HasReachedDestination()
        {
            if (!Entity.Data.CanMove)
                return true;

            if (_navMeshAgent.isStopped)
                return true;

            return Vector3.Distance(transform.position, _destination) <= _navMeshAgent.stoppingDistance + 0.1f;
        }
        #endregion
        #endregion
    }

#if UNITY_EDITOR
    public partial class EntityMovement : AbstractEntityComponent
    {
        private void OnDrawGizmos()
        {
            if (!Application.isPlaying)
                return;

            // default _destination == Vector3.zero
            if (_destination == Vector3.zero)
                return;

            if (!Entity.GetCharacterComponent<EntitySelectable>().IsSelected)
                return;

            DrawLineToDestination();
            DrawDestination();
        }

        private void DrawLineToDestination()
        {
            Gizmos.color = Color.white;
            Gizmos.DrawLine(transform.position, _destination);
        }

        private void DrawDestination()
        {
            if (_navMeshAgent == null)
                return;

            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(_destination, _navMeshAgent.stoppingDistance);
        }
    }
#endif
}
