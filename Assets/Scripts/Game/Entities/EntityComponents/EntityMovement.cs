using Game.Entities.Actions;
using Lortedo.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Game.Entities
{
    /// <summary>
    /// This script manage the moving of entity.
    /// </summary>
    public class EntityMovement : EntityComponent
    {
        #region Fields
        private const string debugLogHeader = "Entity Movement : ";
        private const float reachDestinationThreshold = 0.5f;
        private const float radiusThreshold = 0.1f;

        public event Action<Vector3> DestinationReached;
        public event Action<Vector3> StartMove;
        /// <summary>
        /// Called on DestinationReached and on StopMove();
        /// </summary>
        public event System.Action MovementStopped;

        [SerializeField] private EntityShiftData _shiftData;

        private Vector3 _destination;
        private bool _hasReachedDestination = false;

        // cache variable
        private NavMeshAgent _navMeshAgent;
        private CapsuleCollider _collider;
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
                SetupNavMeshAgent();
            }
        }

        void Update()
        {
            if (!Entity.Data.CanMove)
                return;

            var oldHasReachedDestination = _hasReachedDestination;
            _hasReachedDestination = HasReachedDestination();

            // has just reached destination ?
            if (!oldHasReachedDestination && _hasReachedDestination)
            {
                DestinationReached?.Invoke(_destination);
                MovementStopped?.Invoke();
            }
        }

        void OnEnable()
        {
            Entity.GetCharacterComponent<EntityDetection>().OnAllyEnterShiftRange += EntityMovement_OnAllyEnterShiftRange;
        }

        void OnDisable()
        {
            Entity.GetCharacterComponent<EntityDetection>().OnAllyEnterShiftRange -= EntityMovement_OnAllyEnterShiftRange;

            StopMoving();
        }

        private void OnDrawGizmos()
        {
            if (!Application.isPlaying)
                return;

            if (_navMeshAgent == null)
                return;

            // default _destination == Vector3.zero
            if (_destination == Vector3.zero)
                return;

            if (!Entity.GetCharacterComponent<EntitySelectable>().IsSelected)
                return;

            Gizmos.color = Color.white;
            Gizmos.DrawLine(transform.position, _destination);

            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(_destination, _navMeshAgent.stoppingDistance);
        }
        #endregion

        #region Events Handlers
        private void EntityMovement_OnAllyEnterShiftRange(Entity ally)
        {
            // Make the entity shift to let the hitEntity walks throught the crowd
            if (Entity.IsIdle && !ally.IsIdle)
            {
                Shift(ally);
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

            _navMeshAgent.isStopped = false;
            _navMeshAgent.SetDestination(position);

            StartMove?.Invoke(position);
        }

        public void StopMoving()
        {
            if (!Entity.Data.CanMove)
                return;

            SetAvoidance(Avoidance.Idle);

            _navMeshAgent.isStopped = true;
            MovementStopped?.Invoke();
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

        #region Private methods
        private void SetupNavMeshAgent()
        {
            _navMeshAgent.speed = Entity.Data.Speed;

            if (Entity.Data.TileSize.x != Entity.Data.TileSize.y)
            {
                Debug.LogWarningFormat(debugLogHeader + "TileSize isn't a square. We set nav mesh agent's radius with highest value of tile size.");
            }

            float diameter = Mathf.Max(Entity.Data.TileSize.x, Entity.Data.TileSize.y);
            _navMeshAgent.radius = diameter / 2 - radiusThreshold;
        }

        private void Shift(Entity hitEntity)
        {
            Vector3 fleeHitEntityDirection = Quaternion.Euler(0, -90, 0) * -Lortedo.Utilities.Math.Direction(Entity.transform.position, hitEntity.transform.position);

            var action = new ActionMoveToPosition(Entity, transform.position + fleeHitEntityDirection * _shiftData.ShiftLength);

            Entity.SetAction(action, false);
        }
        #endregion
        #endregion
    }
}
