using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandAttack : OwnerState<CommandReceiverEntity>
{
    #region Fields
    private Transform _target;
    private float _attackTimer = 0;
    private bool _canMove = false;
    #endregion

    #region Methods
    public CommandAttack(CommandReceiverEntity owner, Transform target, bool canMove = false) : base(owner)
    {
        _target = target;
        _canMove = canMove;
    }

    public override void Tick()
    {
        _attackTimer += Time.deltaTime;

        if (_target == null)
            return;

        // is in attackrange ?
        if (Vector3.Distance(_owner.transform.position, _target.position) <= _owner.Data.AttackRange)
        {
            if (_owner.NavMeshAgent != null) _owner.NavMeshAgent.isStopped = true;

            // can attack ?
            if (_attackTimer >= _owner.Data.AttackSpeed)
            {
                _attackTimer = 0;
                _target.GetComponent<Entity>().GetDamage(_owner.Data.Damage, _owner.Entity);
            }
        }
        else
        {
            if (!_canMove)
                return;

            _owner.NavMeshAgent.isStopped = false;
            _owner.NavMeshAgent.SetDestination(_target.position);
        }
    }
    #endregion
}
