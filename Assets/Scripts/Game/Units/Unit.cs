using LeonidasLegacy.IA.Action;
using Lortedo.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SelectionBase]
public class Unit : MonoBehaviour
{
    #region Fields
    [Header("Team Configuration")]
    [SerializeField] private EntityType _type;
    [SerializeField, MyBox.ReadOnly] private Team _team;

    private UnitData _data;
    private Action _currentAction;
    private Queue<Action> _queueAction = new Queue<Action>();

    // cache variables
    private Dictionary<System.Type, UnitComponent> _components = new Dictionary<System.Type, UnitComponent>();
    #endregion

    #region Properties
    public UnitData Data { get => _data; }
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

        if (_type.IsUnitType() != null)
        {
            _team = _type.GetOwner();
        }

    }

    void Update()
    {
        _currentAction?.Tick();
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
    public T GetCharacterComponent<T>() where T : UnitComponent
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

    public void Stop()
    {
        SetCurrentAction(null);
        _queueAction.Clear();
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
        foreach (System.Type type in UtilsClass.GetSubclass<UnitComponent>())
        {
            UnitComponent charComponent = (UnitComponent)GetComponent(type);

            if (charComponent != null)
            {
                _components.Add(type, charComponent);
                charComponent.UnitManager = this;
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
