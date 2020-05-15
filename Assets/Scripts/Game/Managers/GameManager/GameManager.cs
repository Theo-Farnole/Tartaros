using LeonidasLegacy.MapCellEditor;
using Lortedo.Utilities.Pattern;
using UnityEngine;

public delegate void OnResourcesUpdate(ResourcesWrapper resources);
public delegate void OnTileTerrainChanged(Vector2Int coords, GameObject gameObjectAtCoords);

public class GameManager : Singleton<GameManager>
{
    #region Fields
    public static event OnResourcesUpdate OnGameResourcesUpdate;
    // refactor: should be in tile system
    // however, we set here to avoir LawOfDemeter 
    // ex: GameManager.Instance.TileSystem.OnTileTerrainChanged += [...]
    public event OnTileTerrainChanged OnTileTerrainChanged;

    [SerializeField] private GameManagerData _data;
    [SerializeField] private CollisionScalerData _collisionScalerData;
    [SerializeField] private AttackSlotsData _attackSlotsData;
    [Space]
    [SerializeField] private SnapGridDatabase _grid;
    [SerializeField] private MapCells _mapCells;
    [Header("DEBUG")]
    [SerializeField] private bool _debugDrawSnapGrid;

    private OwnedState<GameManager> _state = null;
    private ResourcesWrapper _resources = new ResourcesWrapper();    
    private TileSystem _tileSystem;

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
    public TileSystem TileSystem { get => _tileSystem; }
    #endregion

    #region Methods
    #region MonoBehaviour Callbacks
    void Awake()
    {
        Resources = _data.StartingResources;
    }

    void Start()
    {
        _tileSystem = new TileSystem();
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

        _mapCells?.DrawGizmos();
    }
    #endregion

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

    // refactor:
    // we'll remove this function when TileSystem'll be refactorized
    // this method is only called by TileSystem.
    public void InvokeOnTileTerrainChanged(Vector2Int coords, GameObject gameObjectAtCoords)
    {
        OnTileTerrainChanged?.Invoke(coords, gameObjectAtCoords);
    }
    #endregion
}
