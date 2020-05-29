using Game.IA.Action;
using Lortedo.Utilities;
using Lortedo.Utilities.Pattern;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Assertions;

public delegate void EntityDelegate(Entity entity);
public delegate void OnTeamSwap(Entity entity, Team oldTeam, Team newTeam);

/// <summary>
/// Manage action tick & queueing. Initialize EntityComponent.
/// </summary>
[SelectionBase]
public class Entity : MonoBehaviour, IPooledObject
{
    #region Fields
    public static event EntityDelegate OnSpawn;
    public static event EntityDelegate OnDeath;
    public static event OnTeamSwap OnTeamSwap;

    [Header("Team Configuration")]
    [SerializeField] private string _entityID;
    [SerializeField, ReadOnly] private Team _team;


    private EntityData _data;
    private Action _currentAction;
    private Queue<Action> _queueAction = new Queue<Action>();

    // cache variables
    private Dictionary<System.Type, EntityComponent> _components = new Dictionary<System.Type, EntityComponent>();
    #endregion

    #region Properties
    public bool IsSpawned { get; private set; }
    public bool IsInstanciate { get => this != null && gameObject.activeInHierarchy; }

    public EntityData Data
    {
        get
        {
            if (_data == null)
            {
                Assert.IsNotNull(MainRegister.Instance, "MainRegister is missing the scene.There'll be a lot of errors!");

                _data = MainRegister.Instance.GetEntityData(_entityID);

                Assert.IsNotNull(_data,
                    string.Format("Entity : EntityData not founded or is null for entity {1} of type {0} :/", _entityID, name));
            }

            return _data;
        }
    }
    public string EntityID { get => _entityID; }
    public Team Team
    {
        get => _team;
        set
        {
            if (value == _team)
                return;

            var oldTeam = _team;
            _team = value;
            SetupTeamComponents();

            OnTeamSwap?.Invoke(this, oldTeam, _team);
        }
    }
    public bool HasCurrentAction { get => (_currentAction != null); }
    public Action CurrentAction { get => _currentAction; }
    public bool IsIdle { get => !HasCurrentAction; }
    public string ObjectTag { get; set; }
    #endregion

    #region Methods
    #region Mono Callbacks
    void Start()
    {
        OnObjectSpawn();
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

        Assert.IsNotNull(Data.Prefab, "Entity : Can't enqueue gameObject in ObjectPooler because Data.Prefab is null.");
        ObjectPooler.Instance.EnqueueGameObject(Data.Prefab, gameObject, true);

        OnDeath?.Invoke(this);

        IsSpawned = false;
    }

    /// <summary>
    /// Get cached character component.
    /// </summary>
    public T GetCharacterComponent<T>() where T : EntityComponent
    {
        System.Type key = typeof(T);

        // if component 'T' doesn't exits, get it.
        if (!_components.ContainsKey(key))
            RegisterComponent((T)GetComponent(key));

        return (T)_components[key];
    }

    public void RegisterComponent(EntityComponent component)
    {
        System.Type key = component.GetType();

        Assert.IsFalse(_components.ContainsKey(key), string.Format("Can't register component {0} because it already contains a key.", key));

        _components.Add(key, component);
        component.Entity = this;
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
        if (!action.CanExecuteAction())
        {
            Debug.LogFormat("Entity : Can't do {0}", action.ToString());
            if (!addToActionQueue) _queueAction.Clear();
            return;
        }

        if (addToActionQueue)
        {
            if (HasCurrentAction)
            {
                _queueAction.Enqueue(action);
            }
            else
            {
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

    public string ActionsToString()
    {
        StringBuilder sb = new StringBuilder();

        string labelCurrentAction = _currentAction != null ? _currentAction.ToString() : "NO CURRENT ACTION";
        sb.AppendLine(labelCurrentAction);

        foreach (var action in _queueAction)
        {
            sb.AppendLine(action.ToString());
        }

        return sb.ToString();
    }

    public void OnObjectSpawn()
    {
        if (IsSpawned)
            return;

        IsSpawned = true;

        SetupTeamComponents();
        OnSpawn?.Invoke(this);
    }
    #endregion

    #region Private methods
    void SetCurrentAction(Action newAction)
    {
        _currentAction?.OnStateExit();
        _currentAction = newAction;
        _currentAction?.OnStateEnter();
    }

    private void SetupTeamComponents()
    {
        bool isAVisionGiver = (_team == Team.Player);

        GetCharacterComponent<EntityFogVision>().enabled = isAVisionGiver;
        GetCharacterComponent<EntityFogCoverable>().enabled = !isAVisionGiver;
        GetCharacterComponent<EntityMinimap>().UpdatePointColor();
    }
    #endregion
    #endregion
}
