using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

[System.Flags]
public enum CommandType
{
    Move = 1 << 0,
    Attack = 1 << 1,
    SpawnUnit = 1 << 2
}

public class CommandsReceiverEntity : MonoBehaviour
{
    #region Class
    public class CollisionScaler
    {
        private NavMeshAgent _navMeshAgent;
        private Coroutine _currentCoroutine = null;
        private float _startingRadius = 3;

        private MonoBehaviour _owner;

        private float ReducedRadius { get => _startingRadius * GameManager.Instance.CollisionScalerData.CollisionScaleDownPercent; }

        public CollisionScaler(MonoBehaviour owner, NavMeshAgent navMeshAgent)
        {
            _owner = owner;
            _navMeshAgent = navMeshAgent;

            _startingRadius = navMeshAgent.radius;
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
    [SerializeField, EnumFlag] private CommandType _availableCommands;
    [Header("Attack Fields")]
    [SerializeField] private AttackData _data;
    [Header("Spawn Units Fields")]
    [SerializeField] private UnitCreationData[] _creatableUnits;

    private OwnerState<CommandsReceiverEntity> _currentCommand;
    private CollisionScaler _collisionScaler;

    // cache variables
    private NavMeshAgent _navMeshAgent;
    private Entity _entity;
    #endregion

    #region Properties
    private OwnerState<CommandsReceiverEntity> Command
    {
        get
        {
            return _currentCommand;
        }

        set
        {
            _currentCommand?.OnStateExit();
            _currentCommand = value;
            _currentCommand?.OnStateEnter();
        }
    }

    public AttackData Data { get => _data; }
    public NavMeshAgent NavMeshAgent { get => _navMeshAgent; }
    public Entity Entity { get => _entity; }

    public UnitCreationData[] CreatableUnits { get => _creatableUnits; }

    public bool CanMove { get => _availableCommands.HasFlag(CommandType.Move); }
    public bool CanAttack { get => _availableCommands.HasFlag(CommandType.Attack); }
    public bool CanSpawnUnit { get => _availableCommands.HasFlag(CommandType.SpawnUnit); }
    public CollisionScaler CollisionScaler1 { get => _collisionScaler; }
    #endregion

    #region Methods
    #region MonoBehaviour Callbacks
    void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _entity = GetComponent<Entity>();

        if (_navMeshAgent != null)
        {
            _collisionScaler = new CollisionScaler(this, _navMeshAgent);
            _navMeshAgent.avoidancePriority = Random.Range(0, 101);
        }
    }

    void Update()
    {
        _currentCommand?.Tick();
    }
    #endregion

    #region Commands Receive
    public void Move(Vector3 destination)
    {
        if (CanMove)
        {
            Command = new CommandNavMeshMove(this, destination);
        }

        else
        {
            Debug.Log("Can't move");
        }
    }

    public void Attack(Transform target)
    {
        if (CanAttack)
        {
            Command = new CommandAttack(this, target, CanMove);
        }
        else
        {
            Debug.Log("Can't attack");
        }
    }

    public void SpawnUnit(Unit unitType)
    {
        if (CanSpawnUnit)
        {
            _currentCommand = new CommandSpawnUnit(this, unitType);
        }
    }

    public void Stop()
    {
        Command = null;
    }
    #endregion
    #endregion
}
