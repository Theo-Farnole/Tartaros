using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandAttack : OwnerState<CommandsReceiver>
{
    #region Enum
    enum State
    {
        GoToSlot,
        Attack
    }
    #endregion

    #region Fields
    private Transform _target;
    private AttackSlots.Slot _currentSlot;

    private State _currentState = State.Attack;

    private float _initialDistance;
    private bool _canMove = false;
    private float _attackTimer = 0;

    // cache fields
    private Entity _targetEntity;
    private AttackSlots _attackSlots;
    #endregion

    #region Properties
    private float AttackRange { get => _owner.Entity.Data.AttackRange; }
    private State CurrentState
    {
        get => _currentState;

        set
        {
            // update navMeshAgent avoidance & stopped value
            switch (value)
            {
                // disallow GoToSlot state if entity can't move.
                case State.GoToSlot:
                    if (_canMove)
                    {
                        _currentState = value;

                        if (_owner.NavMeshAgent)
                        {
                            _owner.NavMeshAgent.isStopped = false;
                        }
                    }
                    break;

                case State.Attack:
                    _currentState = value;
                    if (_owner.NavMeshAgent)
                    {
                        _owner.NavMeshAgent.isStopped = true;
                        _owner.NavMeshAgent.avoidancePriority = AvoidanceSystem.GetFightPriority();
                    }
                    break;
            }
        }
    }
    #endregion

    #region Methods
    public CommandAttack(CommandsReceiver owner, Transform target, bool canMove = false) : base(owner)
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

        _initialDistance = Vector3.Distance(_owner.Transform.position, _target.position);

        if (_canMove)
        {
            AssignSlot();
        }
    }

    #region Commands Callbacks
    public override void OnStateExit()
    {
        // if slot assigned, release it
        if (_currentSlot != null)
        {
            _attackSlots.ReleaseSlot(_currentSlot);
        }

        // reset avoidancePriority
        if (_owner.NavMeshAgent == null)
        {
            _owner.NavMeshAgent.avoidancePriority = AvoidanceSystem.GetIdlePriority();
        }
    }

    public override void Tick()
    {
        _attackTimer += Time.deltaTime;

        if (_target == null)
        {
            _owner.Stop();
            return;
        }

        switch (CurrentState)
        {
            case State.GoToSlot:
                GotoSlot();
                break;

            case State.Attack:
                TryAttack();
                break;
        }
    }
    #endregion

    #region States Methods
    private void TryAttack()
    {
        // check if entity is close enought to attack
        if (Vector3.Distance(_owner.Transform.position, _target.position) > AttackRange)
        {
            CurrentState = State.GoToSlot;
            return;
        }

        // Is attack timer allow attack ?
        if (_attackTimer >= _owner.Entity.Data.AttackSpeed)
        {
            _attackTimer = 0;
            _targetEntity.GetDamage(_owner.Entity.Data.Damage, _owner.Entity);
        }
    }

    private void GotoSlot()
    {
        _owner.NavMeshAgent.SetDestination(_target.position + _currentSlot.localPosition);
        _owner.NavMeshAgent.avoidancePriority = AvoidanceSystem.GetHasTargetPriority(_owner.NavMeshAgent.remainingDistance, _initialDistance);

        if (_attackSlots.GetNearestSlot(_owner.Transform.position) != _currentSlot)
        {
            AssignSlot();
        }

        if (!_owner.NavMeshAgent.pathPending && _owner.NavMeshAgent.remainingDistance <= _owner.NavMeshAgent.stoppingDistance)
        {
            CurrentState = State.Attack;
        }
    }
    #endregion

    private void AssignSlot()
    {
        _attackSlots = _targetEntity.GetAttackSlots(AttackRange);

        // if slot assigned, release it
        if (_currentSlot != null)
        {
            _attackSlots.ReleaseSlot(_currentSlot);
        }

        _currentSlot = _attackSlots.AssignNearestSlot(_owner.Transform.position);

        CurrentState = State.GoToSlot;
    }
    #endregion
}
