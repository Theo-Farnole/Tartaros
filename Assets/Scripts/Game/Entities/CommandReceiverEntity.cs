using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[System.Flags]
public enum CommandType
{
    Move = 1,
    Attack = 1 << 2,
    SpawnUnit = 1 << 3
}

public class CommandReceiverEntity : MonoBehaviour
{
    #region Fields
    [SerializeField, EnumFlag] private CommandType _availableCommands;
    [Header("Attack Fields")]
    [SerializeField] private AttackData _data;
    [Header("Spawn Units Fields")]
    [SerializeField] private UnitCreationData[] _creatableUnits;


    private OwnerState<CommandReceiverEntity> _currentCommand;

    // cache variables
    private NavMeshAgent _navMeshAgent;
    private Entity _entity;
    #endregion

    #region Fields
    private OwnerState<CommandReceiverEntity> Command
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
        if (GetComponent<SelectableEntity>().Type == EntityType.Alexios)
        {
            Debug.Log("A: _availableCommands > " + _availableCommands);
        }

        _currentCommand?.Tick();
    }
    #endregion

    #region Commands Receive
    public void Move(Vector3 destination)
    {
        if (_availableCommands.HasFlag(CommandType.Move))
        {
            Command = new CommandNavMeshGoTo(this, destination);
        }
    }

    public void Attack(Transform target)
    {
        if (_availableCommands.HasFlag(CommandType.Attack))
        {
            // todo: fix -> peut se déplacer même sans MOVE
            Command = new CommandAttack(this, target);
        }
    }

    public void Stop()
    {
        Command = null;
    }
    #endregion
    #endregion
}
