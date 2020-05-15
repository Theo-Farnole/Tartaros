using LeonidasLegacy.MapCellEditor;
using Lortedo.Utilities.Pattern;
using TF.Cheats;
using UnityEngine;
using UnityEngine.Assertions;

public delegate void OnResourcesUpdate(ResourcesWrapper resources);


public class GameManager : Singleton<GameManager>
{
    #region Fields
    public static event OnResourcesUpdate OnGameResourcesUpdate;
    public static event IntDelegate OnPopulationUse;

    [SerializeField] private GameManagerData _data;
    [SerializeField] private CollisionScalerData _collisionScalerData;
    [SerializeField] private AttackSlotsData _attackSlotsData;
    [Space]
    [SerializeField] private SnapGridDatabase _grid;
    [SerializeField] private MapCells _mapCells;
    [Header("DEBUG")]
    [SerializeField] private bool _debugDrawSnapGrid;
    [SerializeField] private bool _debugDrawMapCells;

    private OwnedState<GameManager> _state = null;
    private ResourcesWrapper _resources = new ResourcesWrapper();
    private int _populationUse = 0;

    private static bool _applicationIsQuitting = false;
    #endregion

    #region Properties
    public SnapGridDatabase Grid { get => _grid; }
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
        }
    }
    public OwnedState<GameManager> State
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

    public static bool ApplicationIsQuitting { get => _applicationIsQuitting; }
    public CollisionScalerData CollisionScalerData { get => _collisionScalerData; }
    public AttackSlotsData AttackSlotsData { get => _attackSlotsData; }
    public MapCells MapCells { get => _mapCells; }

    public int PopulationUse
    {
        get => _populationUse;

        private set
        {
            _populationUse = value;
            OnPopulationUse?.Invoke(_populationUse);
        }
    }
    #endregion

    #region Methods
    #region MonoBehaviour Callbacks
    void Awake()
    {
        Resources = _data.StartingResources;
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

        if (_debugDrawMapCells)
            _mapCells?.DrawGizmos();
    }

    void OnEnable()
    {
        Entity.OnSpawn += Entity_OnSpawn;
        Entity.OnDeath += Entity_OnDeath;
    }

    void OnDisable()
    {
        Entity.OnSpawn -= Entity_OnSpawn;
        Entity.OnDeath -= Entity_OnDeath;
    }
    #endregion

    #region Events handlers
    private void Entity_OnSpawn(Entity entity)
    {
        if (entity.Team == Team.Sparta)
        {
            PopulationUse += entity.Data.PopulationUse;

            // On start, if there is already created Entity in scene assert fails.
            //
            // ex: 
            // There 3 entities in the scene.
            // The first entity send OnSpawn. The population equals 1.
            // Then the second. The population equals 2.
            // However because there is 2 entities, GetCurrentPopulation returns directly 2.            
            // So, at the first entity's OnSpawn 1 != 2. 
            // To avoid that, we add delay
            //
            // We could have used frameCount. But there is not 'Time.frameSinceLeveLoad'
            if (Time.timeSinceLevelLoad > 0.2f)
            {
                Assert.AreEqual(_populationUse, GetCurrentPopulation(),
                    "Game Manager : Current population isn't the same as calculated.");
            }
        }
    }

    private void Entity_OnDeath(Entity entity)
    {
        if (entity.Team == Team.Sparta)
        {
            PopulationUse -= entity.Data.PopulationUse;

            Assert.AreEqual(_populationUse, GetCurrentPopulation() - entity.Data.PopulationUse,
                "Game Manager : Current population isn't the same as calculated.");
        }
    }
    #endregion

    #region Public methods
    public void StartBuilding(BuildingType buildingType)
    {
        if (MainRegister.Instance.TryGetBuildingData(buildingType, out EntityData buildingData))
        {
            var buildingCost = buildingData.SpawningCost;

            // check if we has enought resources, otherwise we create error message
            if (_resources.HasEnoughResources(buildingCost))
            {
                State = new BuildingState(this, buildingType);
            }
            else
            {
                UIMessagesLogger.Instance.AddErrorMessage("You doesn't have enough resources to build " + buildingType);
            }
        }
        else
        {
            Debug.LogErrorFormat("GameManager: can't start building {0}, because the corresponding EntityData cannont be get.", buildingType);
        }
    }

    int GetCurrentPopulation()
    {
        var entities = FindObjectsOfType<Entity>();
        int populationUsage = 0;

        foreach (var entity in entities)
        {
            if (entity.Team == Team.Sparta)
            {
                populationUsage += entity.Data.PopulationUse;
            }
        }

        return populationUsage;
    }
    #endregion
    #endregion
}
