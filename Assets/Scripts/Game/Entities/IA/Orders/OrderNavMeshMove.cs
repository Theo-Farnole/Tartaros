using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class OrderNavMeshMove : OwnerState<OrdersReceiver>
{
    public OrderNavMeshMove(OrdersReceiver owner, Vector3 destination) : base(owner)
    {
        _owner.NavMeshAgent.SetDestination(destination);
    }

    public override void OnStateEnter()
    {
        _owner.NavMeshAgent.isStopped = false;
        _owner.CollisionScaler1?.ReduceRadius();
    }

    public override void OnStateExit()
    {
        _owner.NavMeshAgent.isStopped = true;
        _owner.CollisionScaler1?.IncreaseRadius();
    }

    public override void Tick()
    {
        float dist = _owner.NavMeshAgent.remainingDistance;

        if (!_owner.NavMeshAgent.pathPending && _owner.NavMeshAgent.remainingDistance <= _owner.NavMeshAgent.stoppingDistance)
        {
            _owner.Stop();
        }
    }
}
