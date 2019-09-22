using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MovableEntity : MonoBehaviour
{
    private NavMeshAgent _navMeshAgent;

    void Awake()
    {
        _navMeshAgent = gameObject.GetComponent<NavMeshAgent>();
    }

    public void GoTo(Vector3 target)
    {
        _navMeshAgent.isStopped = false;
        _navMeshAgent.SetDestination(target);
    }

    public void Stop()
    {
        _navMeshAgent.isStopped = true;
    }
}
