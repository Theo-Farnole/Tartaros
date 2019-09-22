using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MoveCommand : Command
{
    private NavMeshAgent _navMeshAgent;

    public MoveCommand(GameObject owner) : base(owner)
    {
        _navMeshAgent = owner.GetComponent<NavMeshAgent>();
    }

    public override void Execute()
    {
        Vector3 target = Vector3.zero;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Grid")))
        {
            target = hit.point;
            Execute(target);
        }
    }

    public void Execute(Vector3 target)
    {
        _navMeshAgent.SetDestination(target);
    }
}
