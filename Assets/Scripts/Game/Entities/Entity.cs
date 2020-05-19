using Game.IA.Action;
using Lortedo.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void EntityDelegate(Entity ent);

/// <summary>
/// Manage action tick & queueing. Initialize EntityComponent.
/// </summary>
[SelectionBase]
public class Entity : MonoBehaviour
{
    #region Fields
    public static event EntityDelegate OnSpawn;
    public static event EntityDelegate OnDeath;

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
    public EntityData Data
    {
        get
        {
            if (_data == null)
            {
                if (MainRegister.Instance == null)
                    Debug.LogErrorFormat("MainRegister is missing the scene. There'll be a lot of errors!");

                bool dataFounded = MainRegister.Instance.TryGetEntityData(_type, out EntityData data);
                _data = data; // without this line, there is plenty of errors

                if (!dataFounded)
                    Debug.LogErrorFormat("Entity : EntityData not founded for entity {1} of type {0} :/", _type, name);
                else if (_data == null)
                    Debug.LogErrorFormat("Entity : EntityData founded is null for entity {1} of type {0} :/", _type, name);
            }

            return _data;
        }
    }
    public EntityType Type { get => _type; }
    public Team Team { get => _team; set => _team = value; }
    public bool HasCurrentAction { get => (_currentAction != null); }
    public Action CurrentAction { get => _currentAction; }
    #endregion

    #region Methods
    #region Mono Callbacks
    void Awake()
    {
        SetTeam();
        InitializeComponents();
    }

    void Start()
    {
        if (_team == Team.Sparta) Destroy(GetCharacterComponent<EntityFogCoverable>());
        else Destroy(GetCharacterComponent<EntityFogVision>());

        OnSpawn?.Invoke(this);
    }

    void Update()
    {
        _currentAction?.Tick();
    }

    void OnValidate()
    {
        SetTeam();
    }

    void SetTeam()
    {
        // only set team if it's a unit
        if (_type.TryGetUnitType(out UnitType unitType))
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

        OnDeath?.Invoke(this);
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

    /// <summary>
    /// 
    /// </summary>
    /// <param name="action"></param>
    /// <param name="addToActionQueue"></param>
    public void SetAction(Action action, bool addToActionQueue = false)
    {
        if (addToActionQueue)
        {
            if (HasCurrentAction)
            {
                Debug.Log("Enqueu");
                _queueAction.Enqueue(action);
            }
            else
            {
                Debug.Log("SetCurrentAction");
                SetCurrentAction(action);
            }
        }
        else
        {
            _queueAction.Clear();
            SetCurrentAction(action);
        }
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
