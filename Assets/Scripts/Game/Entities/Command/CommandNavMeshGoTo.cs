using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CommandNavMeshGoTo : OwnerState<CommandsReceiverEntity>
{
    public CommandNavMeshGoTo(CommandsReceiverEntity owner, Vector3 destination) : base(owner)
    {
        _owner.NavMeshAgent.SetDestination(destination);
    }

    public override void OnStateEnter()
    {
        _owner.NavMeshAgent.isStopped = false;
    }

    public override void OnStateExit()
    {
        _owner.NavMeshAgent.isStopped = true;
    }

    public override void Tick()  { }
}
