using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandAttack : OwnerState<CommandsReceiverEntity>
{
    #region Fields
    private Transform _target;
    private AttackSlots.Slot _slot;
    private int _slotIndex = -1;

    private bool _canMove = false;
    private float _attackTimer = 0;

    // cache fields
    private Entity _targetEntity;
    private AttackSlots _attackSlots;
    #endregion

    #region Methods
    public CommandAttack(CommandsReceiverEntity owner, Transform target, bool canMove = false) : base(owner)
    {
        if (target == null)
        {
            Debug.LogError("Can't set null target on CommandAttack. Aborting it.");
            owner.Stop();
            return;
        }

        _target = target;
        _canMove = canMove;
        _targetEntity = _target.GetComponent<Entity>();

        // get slot and assign position
        if (_canMove)
        {
            _attackSlots = _targetEntity.GetAttackSlots(owner.Data.AttackRange);

            _slotIndex = _attackSlots.GetNearestAvailableSlotIndex(owner.transform.position);
            _slot = _attackSlots.AssignSlot(_slotIndex);
        }
    }

    public override void OnStateExit()
    {
        // if slot assigned, release it
        if (_slotIndex != -1)
        {
            _attackSlots.ReleaseSlot(_slotIndex);
        }
    }

    public override void Tick()
    {
        _attackTimer += Time.deltaTime;

        // is in attackrange ?
        if (Vector3.Distance(_owner.transform.position, _target.position) <= _owner.Data.AttackRange)
        {
            if (_owner.NavMeshAgent != null) _owner.NavMeshAgent.isStopped = true;

            // can attack ?
            if (_attackTimer >= _owner.Data.AttackSpeed)
            {
                _attackTimer = 0;
                _targetEntity.GetDamage(_owner.Data.Damage, _owner.Entity);
            }
        }
        else
        {
            if (!_canMove || _slot == null)
                return;

            _owner.NavMeshAgent.isStopped = false;
            _owner.NavMeshAgent.SetDestination(_target.position + _slot.localPosition);
        }
    }
    #endregion
}
