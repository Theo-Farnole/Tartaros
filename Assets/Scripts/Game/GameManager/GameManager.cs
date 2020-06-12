using Game.ConstructionSystem;
using Game.Entities;
using Game.MapCellEditor;
using Game.WaveSystem;
using Lortedo.Utilities.Pattern;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public delegate void OnResourcesUpdate(ResourcesWrapper resources);
public delegate void OnGameOver(GameManager gameManager);
public delegate void OnVictory(GameManager gameManager);
public delegate void OnStartBuild(GameManager gameManager);
public delegate void OnStopBuild(GameManager gameManager);
public delegate void OnBuildSuccessful(GameManager gameManager);
public delegate void HasNotEnoughtResources(GameManager gameManager, ResourcesWrapper cost);
public delegate void PendingCreationChanged(string id);

public class GameManager : Singleton<GameManager>
{
    #region Fields
    private const string debugLogHeader = "Game Manager : ";

    public static event OnResourcesUpdate OnGameResourcesUpdate;

    public static event OnGameOver OnGameOver;
    public static event OnVictory OnVictory;
    public static event OnStartBuild OnStartBuild;
    public static event OnStopBuild OnStopBuild;
    public static event OnBuildSuccessful OnBuildSuccessful;
    public static event HasNotEnoughtResources HasNotEnoughtResources;

    public static event PendingCreationChanged PendingCreationEntityAdded;
    public static event PendingCreationChanged PendingCreationEntityRemoved;

    [Header("COMPONENTS")]
    [SerializeField] private SnapGridDatabase _grid;
    [SerializeField] private MapCells _mapCells;
    [SerializeField] private PopulationManager _populationManager;
    [Header("DATA")]
    [SerializeField] private GameManagerData _data;
    [SerializeField] private AttackSlotsData _attackSlotsData;
    [Header("DEBUG")]
    [SerializeField] private bool _debugDrawSnapGrid;
    [SerializeField] private bool _debugDrawMapCells;

    private AbstractConstructionState _state = null;
    private ResourcesWrapper _resources = new ResourcesWrapper();

    private static bool _applicationIsQuitting = false;
    private List<string> _pendingCreation = new List<string>();
    #endregion

    #region Properties
    public ResourcesWrapper Resources
    {
        get
        {
            return _resources;
        }
        set
        {
            _resources = value;
            OnGameResourcesUpdate?.Invoke(_resources);

            if (!_resources.HasEnoughResources(ResourcesWrapper.Zero))
                Debug.LogWarning(debugLogHeader + "Resources are negative.");
        }
    }

    public AbstractConstructionState State
    {
        get
        {
            return _state;
        }

        set
        {
            _state?.OnStateExit();
            _state = value;
            _state?.OnStateEnter();
        }
    }

    public SnapGridDatabase Grid { get => _grid; }
    public static bool ApplicationIsQuitting { get => _applicationIsQuitting; }
    public AttackSlotsData AttackSlotsData { get => _attackSlotsData; }
    public MapCells MapCells { get => _mapCells; }

    public GameManagerData ManagerData { get => _data; }
    public List<string> PendingCreation { get => _pendingCreation; }
    public bool DebugDrawMapCells { get => _debugDrawMapCells; set => _debugDrawMapCells = value; }  // used in MapCellsEditorWindow
    #endregion

    #region Methods
    #region MonoBehaviour Callbacks
    void Awake()
    {
        Resources = _data.StartingResources;

        Assert.IsNotNull(_populationManager, "Missing population manager in GameManager.");
        _populationManager.StartPopulation = _data.StartMaxPopulationCount;
    }

    void Update()
    {
        _state?.Tick();
    }

    void OnApplicationQuit()
    {
        _applicationIsQuitting = true;
    }

    void OnDrawGizmos()
    {
        if (_debugDrawSnapGrid)
            _grid?.DrawGizmos();

#if UNITY_EDITOR
        if (_debugDrawMapCells)
            _mapCells?.DrawGizmos();
#endif
    }

    void OnGUI()
    {
        _state?.OnGUI();
    }

    void OnEnable()
    {
        Entity.OnDeath += Entity_OnDeath;
        WaveManager.OnFinalWaveClear += Victory;
    }

    void OnDisable()
    {
        Entity.OnDeath -= Entity_OnDeath;
        WaveManager.OnFinalWaveClear -= Victory;
    }
    #endregion

    #region Events handlers
    private void Entity_OnDeath(Entity entity)
    {
        if (!entity.IsSpawned)
            return;

        if (entity.Team == Team.Player)
        {
            if (entity.EntityID == _data.LoseOnDestroyedEntityID)
            {
                GameOver();
            }
        }
    }
    #endregion

    #region Public methods
    public bool HasEnoughtResources(ResourcesWrapper resources)
        => _resources.HasEnoughResources(resources);

    public bool HasEnoughtPopulationToSpawn()
        => _populationManager.HasEnoughtPopulationToSpawn();

    public bool HasEnoughtPopulationToSpawn(EntityData unitData)
        => _populationManager.HasEnoughtPopulationToSpawn(unitData);

    public void AddPendingCreationEntity(string entityID)
    {
        _pendingCreation.Add(entityID);

        EntityData entityData = MainRegister.Instance.GetEntityData(entityID);

        _populationManager.PopulationCount += entityData.PopulationUse;
        Resources -= entityData.SpawningCost;

        PendingCreationEntityAdded?.Invoke(entityID);
    }

    public void RemovePendingCreationEntity(string entityID)
    {
        int pendingCreationBeforeRemove = _pendingCreation.Count;

        if (_pendingCreation.Remove(entityID))
        {
            EntityData entityData = MainRegister.Instance.GetEntityData(entityID);

            _populationManager.PopulationCount -= entityData.PopulationUse;
            Resources += entityData.SpawningCost;

            Debug.LogFormat("Remove pending: before {0} / after {1}", pendingCreationBeforeRemove, _pendingCreation.Count);

            PendingCreationEntityRemoved?.Invoke(entityID);
        }
    }

    public void StartBuilding(string buildingID)
    {
        EntityData buildingData = MainRegister.Instance.GetEntityData(buildingID);

        Assert.IsNotNull(buildingData,
            string.Format("GameManager: can't start building {0}, because the corresponding EntityData cannont be get.", buildingID));

        var buildingCost = buildingData.SpawningCost;

        // check if we has enought resources, otherwise we create error message
        if (_resources.HasEnoughResources(buildingCost))
        {
            if (buildingData.IsConstructionChained)
            {
                State = new ChainedConstructionState(this, buildingID);
            }
            else
            {
                State = new ConstructionState(this, buildingID);
            }
        }
        else
        {
            UIMessagesLogger.Instance.AddErrorMessage("You doesn't have enough resources to build " + buildingID);
            HasNotEnoughtResources?.Invoke(this, buildingCost);
        }
    }

    public void Invoke_OnStartBuild() => OnStartBuild?.Invoke(this);
    public void Invoke_OnStopBuild() => OnStopBuild?.Invoke(this);
    public void Invoke_OnBuildSuccessful() => OnBuildSuccessful?.Invoke(this);
    #endregion

    #region Private methods
    // called from Entity_OnDeath() if entity was a temple
    void GameOver()
    {
        _state = null;
        OnGameOver?.Invoke(this);
    }

    // called from WaveManager_OnWaveClear()
    void Victory()
    {
        _state = null;
        OnVictory?.Invoke(this);
    }
    #endregion
    #endregion
}
