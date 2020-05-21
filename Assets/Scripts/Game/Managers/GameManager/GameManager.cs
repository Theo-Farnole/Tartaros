using Game.ConstructionSystem;
using Game.MapCellEditor;
using Lortedo.Utilities.Pattern;
using UnityEngine;
using UnityEngine.Assertions;

public delegate void OnResourcesUpdate(ResourcesWrapper resources);
public delegate void OnGameOver(GameManager gameManager);
public delegate void OnVictory(GameManager gameManager);
public delegate void OnStartBuild(GameManager gameManager);
public delegate void OnStopBuild(GameManager gameManager);


public class GameManager : Singleton<GameManager>
{
    #region Fields
    public static event OnResourcesUpdate OnGameResourcesUpdate;
    public static event DoubleIntDelegate OnPopulationCountChanged;
    public static event OnGameOver OnGameOver;
    public static event OnVictory OnVictory;
    public static event OnStartBuild OnStartBuild;
    public static event OnStopBuild OnStopBuild;

    [SerializeField] private GameManagerData _data;
    [SerializeField] private CollisionScalerData _collisionScalerData;
    [SerializeField] private AttackSlotsData _attackSlotsData;
    [Space]
    [SerializeField] private SnapGridDatabase _grid;
    [SerializeField] private MapCells _mapCells;
    [Header("DEBUG")]
    [SerializeField] private bool _debugDrawSnapGrid;
    [SerializeField] private bool _debugDrawMapCells;

    private AbstractConstructionState _state = null;
    private ResourcesWrapper _resources = new ResourcesWrapper();
    private int _populationCount = 0;
    private int _maxPopulation = 0;

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

    public static bool ApplicationIsQuitting { get => _applicationIsQuitting; }
    public CollisionScalerData CollisionScalerData { get => _collisionScalerData; }
    public AttackSlotsData AttackSlotsData { get => _attackSlotsData; }
    public MapCells MapCells { get => _mapCells; }

    public int PopulationCount
    {
        get => _populationCount;

        private set
        {
            _populationCount = value;
            OnPopulationCountChanged?.Invoke(_populationCount, _maxPopulation);
        }
    }

    public int MaxPopulation
    {
        get => _maxPopulation;

        private set
        {
            _maxPopulation = value;
            OnPopulationCountChanged?.Invoke(_populationCount, _maxPopulation);
        }
    }
    #endregion

    #region Methods
    #region MonoBehaviour Callbacks
    void Awake()
    {
        Resources = _data.StartingResources;
        MaxPopulation = _data.StartMaxPopulationCount;
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

    void OnEnable()
    {
        Entity.OnSpawn += Entity_OnSpawn;
        Entity.OnDeath += Entity_OnDeath;
        WaveManager.OnWaveClear += WaveManager_OnWaveClear;
    }

    void OnDisable()
    {
        Entity.OnSpawn -= Entity_OnSpawn;
        Entity.OnDeath -= Entity_OnDeath;
        WaveManager.OnWaveClear -= WaveManager_OnWaveClear;
    }
    #endregion

    #region Events handlers
    private void Entity_OnSpawn(Entity entity)
    {
        if (entity.Team == Team.Sparta)
        {
            PopulationCount += entity.Data.PopulationUse;
            MaxPopulation += entity.Data.IncreaseMaxPopulationAmount;

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
                Assert.AreEqual(_populationCount, GetCurrentPopulation(),
                    "Game Manager : Current population isn't the same as calculated.");

                Assert.AreEqual(_maxPopulation, GetCurrentMaxPopulation(),
                    "Game Manager : Max population isn't the same as calculated.");
            }
        }
    }

    private void Entity_OnDeath(Entity entity)
    {
        if (entity.Team == Team.Sparta)
        {
            PopulationCount -= entity.Data.PopulationUse;
            MaxPopulation -= entity.Data.IncreaseMaxPopulationAmount;

            Assert.AreEqual(_populationCount, GetCurrentPopulation() - entity.Data.PopulationUse,
                "Game Manager : Current population isn't the same as calculated.");

            Assert.AreEqual(_maxPopulation, GetCurrentMaxPopulation(),
                    "Game Manager : Max population isn't the same as calculated.");

            if (entity.Type == EntityType.Temple)
            {
                GameOver();
            }
        }
    }

    private void WaveManager_OnWaveClear(int waveCountCleared)
    {
        if (waveCountCleared == _data.WavesPassedToWin)
        {
            Victory();
        }
    }
    #endregion

    #region Public methods
    public bool HasEnoughtPopulationToSpawn(EntityData unitData)
    {
        return (_populationCount + unitData.PopulationUse <= _maxPopulation);
    }

    public void StartBuilding(BuildingType buildingType)
    {
        if (MainRegister.Instance.TryGetBuildingData(buildingType, out EntityData buildingData))
        {
            var buildingCost = buildingData.SpawningCost;

            // check if we has enought resources, otherwise we create error message
            if (_resources.HasEnoughResources(buildingCost))
            {
                if (buildingData.IsConstructionChained)
                {
                    State = new ChainedConstructionState(this, buildingType);
                }
                else
                {

                    State = new ConstructionState(this, buildingType);
                }
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

    public void Invoke_OnStartBuild() => OnStartBuild?.Invoke(this);
    public void Invoke_OnStopBuild() => OnStopBuild?.Invoke(this);
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

    #region Getter / Calculate methods
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

    int GetCurrentMaxPopulation()
    {
        var entities = FindObjectsOfType<Entity>();
        int maxPopulation = _data.StartMaxPopulationCount;

        foreach (var entity in entities)
        {
            if (entity.Team == Team.Sparta)
            {
                maxPopulation += entity.Data.IncreaseMaxPopulationAmount;
            }
        }

        return maxPopulation;
    }
    #endregion
    #endregion
}
