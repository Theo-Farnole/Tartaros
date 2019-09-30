using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

[System.Flags]
public enum CommandType
{
    Move = 1,
    Attack = 1 << 2,
    SpawnUnit = 1 << 3
}

public class CommandsReceiverEntity : MonoBehaviour
{
    #region Fields
    [SerializeField, EnumFlag] private CommandType _availableCommands;
    [Header("Attack Fields")]
    [SerializeField] private AttackData _data;
    [Header("Spawn Units Fields")]
    [SerializeField] private UnitCreationData[] _creatableUnits;

    private OwnerState<CommandsReceiverEntity> _currentCommand;

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
    #endregion

    #region Methods
    #region MonoBehaviour Callbacks
    void Awake()
    {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _entity = GetComponent<Entity>();
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
            Command = new CommandNavMeshGoTo(this, destination);
        }
    }

    public void Attack(Transform target)
    {
        if (CanAttack)
        {
            Command = new CommandAttack(this, target);
        }
    }

    public void SpawnUnit(Unit unitType)
    {
        _currentCommand = new CommandSpawnUnit(this, unitType);
    }

    public void Stop()
    {
        Command = null;
    }
    #endregion
    #endregion
}
