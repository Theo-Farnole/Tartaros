using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EntityMovement : EntityComponent
{
    #region Fields
    private NavMeshAgent _navMeshAgent;
    #endregion

    #region Methods
    #region Mono Callbacks
    void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
    }
    #endregion

    #region Public methods
    public void MoveToEntity(Entity target)
    {
        if (!Entity.Data.CanMove)
            return;

        _navMeshAgent.isStopped = false;
        _navMeshAgent.SetDestination(target.transform.position);
    }

    public void MoveToPosition(Vector3 position)
    {
        if (!Entity.Data.CanMove) return;

        _navMeshAgent.isStopped = false;
        _navMeshAgent.SetDestination(position);
    }

    public void StopMoving()
    {
        if (!Entity.Data.CanMove) return;

        _navMeshAgent.SetDestination(transform.position);
        _navMeshAgent.ResetPath();

        _navMeshAgent.isStopped = true;
    }


    public bool HasReachedDestination()
    {
        if (!Entity.Data.CanMove) return true;

        if (!_navMeshAgent.pathPending)
        {
            if (_navMeshAgent.remainingDistance <= _navMeshAgent.stoppingDistance)
            {
                if (!_navMeshAgent.hasPath || _navMeshAgent.velocity.sqrMagnitude == 0f)
                {
                    return true;
                }
            }
        }

        return false;
    }
    #endregion
    #endregion
}
