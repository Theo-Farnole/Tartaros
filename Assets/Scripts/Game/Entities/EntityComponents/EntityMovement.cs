using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EntityMovement : EntityComponent
{
    #region Fields
    private const string debugLogHeader = "Entity Movement : ";
    private const float reachDestinationThreshold = 0.3f;
    private NavMeshAgent _navMeshAgent;
    #endregion

    #region Methods
    #region Mono Callbacks
    void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
    }

    void Start()
    {
        if (_navMeshAgent != null)
        {
            SetupNavMeshAgent();
        }
    }

    void OnDisable()
    {
        StopMoving();
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

        SetAvoidance(Avoidance.Move);

        _navMeshAgent.isStopped = false;
        _navMeshAgent.SetDestination(target.transform.position);
    }

    public void MoveToPosition(Vector3 position)
    {
        if (!Entity.Data.CanMove) return;

        SetAvoidance(Avoidance.Move);

        _navMeshAgent.isStopped = false;
        _navMeshAgent.SetDestination(position);
    }

    public void StopMoving()
    {
        if (!Entity.Data.CanMove) return;

        SetAvoidance(Avoidance.Idle);

        _navMeshAgent.isStopped = true;
    }


    public bool HasReachedDestination()
    {
        if (!Entity.Data.CanMove) return true;

        if (_navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance)
        {
            if (!_navMeshAgent.hasPath || Mathf.Abs(_navMeshAgent.velocity.sqrMagnitude) < float.Epsilon)
            {
                return true;
            }
        }

        return false;
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
        _navMeshAgent.radius = diameter / 2;
    }
    #endregion
    #endregion
}
