using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class UnitMovement : UnitComponent
{
    #region Fields
    public readonly static float DISTANCE_THRESHOLD = 0.3f;

    private NavMeshAgent _navMeshAgent;
    #endregion

    #region Methods
    #region Mono Callbacks
    void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
    }

    //void Update()
    //{
    //    Debug.LogFormat("{0} nav mesh agent is stopped = {1}", name, _navMeshAgent.isStopped);    
    //}
    #endregion

    #region Public methods
    public void MoveToUnit(Unit unit)
    {
        _navMeshAgent.isStopped = false;
        _navMeshAgent.SetDestination(unit.transform.position);
    }

    public bool IsUnitInAttackRange(Unit unit)
    {
        return Vector3.Distance(transform.position, unit.transform.position) <= UnitManager.Data.AttackRadius;
    }

    public bool HasReachedDestination()
    {        
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

    public void MoveToPosition(Vector3 position)
    {
        _navMeshAgent.isStopped = false;
        _navMeshAgent.SetDestination(position);
    }

    public void StopMoving()
    {
        _navMeshAgent.SetDestination(transform.position);
        _navMeshAgent.ResetPath();

        _navMeshAgent.isStopped = true;
    }

    public bool IsNearFromUnit(Unit unit)
    {
        if (Vector3.Distance(transform.position, unit.transform.position) <= DISTANCE_THRESHOLD)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool IsNearFromPosition(Vector3 position)
    {
        if (Vector3.Distance(transform.position, position) <= DISTANCE_THRESHOLD)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    #endregion
    #endregion
}
