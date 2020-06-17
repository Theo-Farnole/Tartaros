namespace Game.GameManagers
{
    using Game.ConstructionSystem;
    using Game.Entities;
    using Game.MapCellEditor;
    using Game.WaveSystem;
    using Lortedo.Utilities.Pattern;
    using System.Collections.Generic;    
    using UnityEngine;
    using UnityEngine.Assertions;
    using Game.TileSystem;
    using Game.GameState;

    public delegate void OnResourcesUpdate(ResourcesWrapper resources);
    public delegate void OnGameOver(GameManager gameManager);
    public delegate void OnVictory(GameManager gameManager);
    public delegate void OnStartBuild(GameManager gameManager);
    public delegate void OnStopBuild(GameManager gameManager);
    public delegate void OnBuildSuccessful(GameManager gameManager);
    public delegate void HasNotEnoughtResources(GameManager gameManager, ResourcesWrapper cost);
    public delegate void PendingCreationChanged(string id);

    public partial class GameManager : Singleton<GameManager>
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

        void OnGUI()
        {
            _state?.OnGUI();
        }

        void OnEnable()
        {
            Entity.OnDeath += Entity_OnDeath;
            WaveManager.OnFinalWaveClear += Victory;
            GameState.OnGamePaused += GameState_OnGamePaused;
        }

        void OnDisable()
        {
            Entity.OnDeath -= Entity_OnDeath;
            WaveManager.OnFinalWaveClear -= Victory;
            GameState.OnGamePaused -= GameState_OnGamePaused;
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

        private void GameState_OnGamePaused()
        {
            State = null;
        }
        #endregion

        #region Public methods
        public bool HasEnoughtResources(ResourcesWrapper resources) => _resources.HasEnoughResources(resources);

        public bool HasEnoughtPopulationToSpawn() => _populationManager.HasEnoughtPopulationToSpawn();

        public bool HasEnoughtPopulationToSpawn(EntityData unitData) => _populationManager.HasEnoughtPopulationToSpawn(unitData);

        public void Invoke_OnStartBuild() => OnStartBuild?.Invoke(this);

        public void Invoke_OnStopBuild() => OnStopBuild?.Invoke(this);

        public void Invoke_OnBuildSuccessful() => OnBuildSuccessful?.Invoke(this);

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


                PendingCreationEntityRemoved?.Invoke(entityID);
            }
        }

        public void StartBuilding(string buildingID)
        {
            if (GameState.IsGamePaused())
                return;

            EntityData buildingData = MainRegister.Instance.GetEntityData(buildingID);

            Assert.IsNotNull(buildingData, string.Format("GameManager: can't start building {0}, because the corresponding EntityData cannont be get.", buildingID));

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
                UIMessagesLogger.Instance.LogErrorFormat("You doesn't have enough resources to build ", buildingID);
                HasNotEnoughtResources?.Invoke(this, buildingCost);
            }
        }


        public Entity SpawnEntity(string entityID, Vector3 position, Quaternion rotation, Team team)
        {
            Assert.IsNotNull(MainRegister.Instance, "MainRegister is missing. Can't spawn unit");
            Assert.IsNotNull(ObjectPooler.Instance, "MainRegister is missing. Can't spawn unit");

            EntityData entityData = MainRegister.Instance.GetEntityData(entityID);

            Resources -= entityData.SpawningCost;

            Entity instanciatedEntity = ObjectPooler.Instance.SpawnFromPool(entityData.Prefab, position, rotation, true).GetComponent<Entity>();

            instanciatedEntity.Team = team;

            if (entityData.EntityType == EntityType.Building)
            {
                TileSystem.Instance.SetTile(instanciatedEntity.gameObject, entityData.TileSize);
            }

            TF.Assertations.Assert.IsTrue(instanciatedEntity.IsIdle, "Spawn entity should be idle. Check if StopActions if called on Entity's death.");

            return instanciatedEntity;
        }

        public bool CanSpawnEntity(string entityID, bool logErrors)
        {
            Assert.IsNotNull(MainRegister.Instance, "MainRegister is missing. Can't spawn unit");

            EntityData unitData = MainRegister.Instance.GetEntityData(entityID);

            if (unitData.PopulationUse > 0 && !HasEnoughtPopulationToSpawn())
            {
                if (logErrors)
                {
                    UIMessagesLogger.Instance.LogErrorFormat("Build more house to create {0} unit.", entityID);
                }

                return false;
            }

            if (!Resources.HasEnoughResources(unitData.SpawningCost))
            {
                if (logErrors)
                {
                    UIMessagesLogger.Instance.LogError("You doesn't have enought resources to create " + entityID + ".");
                    HasNotEnoughtResources?.Invoke(this, unitData.SpawningCost);
                }

                return false;
            }

            return true;
        }
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

#if UNITY_EDITOR
    public partial class GameManager : Singleton<GameManager>
    {
        void OnDrawGizmos()
        {
            if (_debugDrawSnapGrid)
                _grid?.DrawGizmos();

            if (_debugDrawMapCells)
                _mapCells?.DrawGizmos();
        }
    }
#endif
}