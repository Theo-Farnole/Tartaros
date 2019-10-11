using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;


public class OrderReceiver
{
    #region Class
    public class CollisionScaler
    {
        private MonoBehaviour _owner;
        private NavMeshAgent _navMeshAgent;

        private Coroutine _currentCoroutine = null;
        private float _startingRadius = 3;

        private float ReducedRadius { get => _startingRadius * GameManager.Instance.CollisionScalerData.CollisionScaleDownPercent; }

        public CollisionScaler(MonoBehaviour owner)
        {
            _owner = owner;
            _navMeshAgent = owner.GetComponent<NavMeshAgent>();

            _startingRadius = _navMeshAgent.radius;
        }

        public void ReduceRadius()
        {
            if (_currentCoroutine != null)
            {
                _owner.StopCoroutine(_currentCoroutine);
            }

            _currentCoroutine = new Timer(_owner, GameManager.Instance.CollisionScalerData.ReduceTime, (float t) =>
            {
                _navMeshAgent.radius = _startingRadius + t * (ReducedRadius - _startingRadius);
            },
            () =>
            {
                _navMeshAgent.radius = ReducedRadius;
            }).Coroutine;
        }

        public void IncreaseRadius()
        {
            if (_currentCoroutine != null)
            {
                _owner.StopCoroutine(_currentCoroutine);
            }

            

            _currentCoroutine = new Timer(_owner, GameManager.Instance.CollisionScalerData.IncreaseTime, (float t) =>
            {
                _navMeshAgent.radius = ReducedRadius + t * (_startingRadius - ReducedRadius);
            },
            () =>
            {
                _navMeshAgent.radius = _startingRadius;
            }).Coroutine;
        }
    }
    #endregion

    #region Fields
    private OwnerState<OrderReceiver> _currentState;
    private CollisionScaler _collisionScaler;
    private Entity _entity;

    // cache variables
    private NavMeshAgent _navMeshAgent;
    #endregion

    #region Properties
    private OwnerState<OrderReceiver> State
    {
        get => _currentState;

        set
        {
            _currentState?.OnStateExit();
            _currentState = value;
            _currentState?.OnStateEnter();
        }
    }

    public NavMeshAgent NavMeshAgent { get => _navMeshAgent; }

    public Entity Entity { get => _entity;  }
    public Transform Transform { get => _entity.transform;  }
    public Unit[] CreatableUnits { get => _entity.Data.AvailableUnitsForCreation; }

    public bool CanMove { get => _entity.Data == null ? false : _entity.Data.CanMove; }
    public bool CanAttack { get => _entity.Data == null ? false : _entity.Data.CanAttack; }
    public bool CanSpawnUnit { get => _entity.Data == null ? false : _entity.Data.CanSpawnUnit; }
    public CollisionScaler CollisionScaler1 { get => _collisionScaler; }
    #endregion

    #region Methods    
    public OrderReceiver(Entity owner)
    {
        _entity = owner;
        _navMeshAgent = owner.GetComponent<NavMeshAgent>();

        if (_navMeshAgent != null)
        {
            _collisionScaler = new CollisionScaler(owner);
        }
    }

    public void Tick()
    {
        _currentState?.Tick();
    }    

    public bool CanOverallAction(OverallAction overallAction)
    {
        switch (overallAction)
        {
            case OverallAction.Stop:
                return true;

            case OverallAction.Move:
                return CanMove;

            case OverallAction.Attack:
                return CanAttack;
        }

        return false;
    }

    #region States Modifier
    public void Move(Vector3 destination)
    {
        if (CanMove)
        {
            State = new OrderNavMeshMove(this, destination);
        }
    }

    public void Attack(Transform target)
    {
        if (CanAttack)
        {
            State = new OrderAttack(this, target, CanMove);
        }
    }

    public void SpawnUnit(Unit unitType)
    {
        if (CanSpawnUnit)
        {
            _currentState = new OrderSpawnUnit(this, unitType);
        }
    }

    public void Stop()
    {
        State = null;
    }
    #endregion
    #endregion
}
