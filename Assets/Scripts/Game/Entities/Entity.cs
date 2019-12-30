using LeonidasLegacy.IA.Action;
using Lortedo.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manage action tick & queueing. Initialize EntityComponent.
/// </summary>
[SelectionBase]
public class Entity : MonoBehaviour
{
    #region Fields
    [Header("Team Configuration")]
    [SerializeField] private EntityType _type;

    [DrawIf(nameof(_type), EntitiesSystem.STARTING_INDEX_BUILDING, ComparisonType.GreaterOrEqual, DisablingType.ReadOnly)]
    [SerializeField] private Team _team;

    private EntityData _data;
    private Action _currentAction;
    private Queue<Action> _queueAction = new Queue<Action>();

    // cache variables
    private Dictionary<System.Type, EntityComponent> _components = new Dictionary<System.Type, EntityComponent>();
    #endregion

    #region Properties
    public EntityData Data { get => _data; }
    public EntityType Type { get => _type; }
    public Team Team { get => _team; set => _team = value; }
    public bool HasCurrentAction { get => (_currentAction != null); }
    public Action CurrentAction { get => _currentAction; }
    #endregion

    #region Methods
    #region Mono Callbacks
    void Awake()
    {
        InitializeComponents();
    }

    void Start()
    {
        _data = Registers.EntitiesRegister.GetRegisterData(_type).EntityData;

        if (_team == Team.Sparta) Destroy(GetCharacterComponent<EntityFogCoverable>());
        else Destroy(GetCharacterComponent<EntityFogVision>());
    }

    void Update()
    {
        _currentAction?.Tick();
    }

    void OnValidate()
    {
        if (_type.GetUnitType() != null)
        {
            _team = _type.GetOwner();
        }
    }
    #endregion

    #region Public methods
    public void Death()
    {
        SetCurrentAction(null);
        Destroy(gameObject);
    }

    /// <summary>
    /// Get cached character component.
    /// </summary>
    public T GetCharacterComponent<T>() where T : EntityComponent
    {
        return (T)_components[typeof(T)];
    }

    public void StopCurrentAction()
    {
        // do Unit have a queued action ?
        if (_queueAction.Count > 0)
        {
            SetCurrentAction(_queueAction.Dequeue());
        }
        else
        {
            SetCurrentAction(null);
        }
    }

    public void AddActionToQueue(Action action)
    {
        // instant play the action if not queue
        if (_queueAction.Count == 0)
        {
            SetCurrentAction(action);
        }
        else
        {
            _queueAction.Enqueue(action);
        }
    }

    /// <summary>
    /// Clear queue and play param action.
    /// </summary>
    /// <param name="action"></param>
    public void SetAction(Action action)
    {
        _queueAction.Clear();

        SetCurrentAction(action);
    }

    /// <summary>
    /// Clear queue and set action to null
    /// </summary>
    public void StopEveryActions()
    {
        _queueAction.Clear();

        SetCurrentAction(null);
    }
    #endregion

    #region Private methods
    void SetCurrentAction(Action newAction)
    {
        _currentAction?.OnStateExit();
        _currentAction = newAction;
        _currentAction?.OnStateEnter();
    }

    void InitializeComponents()
    {
        foreach (System.Type type in UtilsClass.GetSubclass<EntityComponent>())
        {
            EntityComponent charComponent = (EntityComponent)GetComponent(type);

            if (charComponent != null)
            {
                _components.Add(type, charComponent);
                charComponent.Entity = this;
            }
            else
            {
                // throw warning if MonoBehaviour doesn't have a required component,
                // then skip it
                Debug.LogWarning("CharacterManager miss " + type + "component.");
            }
        }
    }
    #endregion
    #endregion
}
