namespace Game.Entities
{
    using Game.MapCellEditor;
    using MyBox;
    using Sirenix.OdinInspector;
    using System.Collections.Generic;
    using Game.UI.HoverPopup;
    using UnityEngine;
    using UnityEngine.Serialization;
    using System;
    using Game.Selection;
    using System.Linq;

    public enum GenerationType
    {
        Constant = 1,
        PerCell = 2
    }

    public enum EntityType
    {
        Unit,
        Building
    }

    [Serializable]
    public class UnitSpawnCondition
    {
        [SerializeField] private string _entityIDToSpawn = string.Empty;

        [ToggleGroup(nameof(_hasSpawnCondition))]
        [SerializeField] private bool _hasSpawnCondition = false;

        [ToggleGroup(nameof(_hasSpawnCondition))]
        [SerializeField] private int _maxAlliesOfSameIDAlive = -1;

        public string EntityIDToSpawn { get => _entityIDToSpawn; }

        /// <summary>
        /// Warning, this method call 'FindObjectOfTypes'. It can be performance heavy.
        /// </summary>
        /// <returns></returns>
        public bool DoConditionsAreMet()
        {
            if (!_hasSpawnCondition)
                return true;

            // PERFORMANCE NOTE:
            // Create an EntityManager where it store every Entity[]
            Entity[] entities = UnityEngine.Object.FindObjectsOfType<Entity>();

            if (_maxAlliesOfSameIDAlive != -1)
            {
                int alliesOfSameIDAlive =
                    entities.Where(x => x.EntityID == _entityIDToSpawn).Count()
                    + GameManager.Instance.PendingCreation.Where(x => x == _entityIDToSpawn).Count();

                bool conditionMet = alliesOfSameIDAlive < _maxAlliesOfSameIDAlive;

                if (!conditionMet)
                    return false;
            }

            return true;
        }
    }

    // Warning!
    // The code below is a bit messy.
    // 
    // Good luck explorers!

    [CreateAssetMenu(menuName = "Tartaros/Entity")]
    public class EntityData : SerializedScriptableObject
    {
        #region Fields
        private const string attackSettingsHeaderName = "Can Attack";
        private const string movementSettingsHeaderName = "Can Move";
        private const string createUnitSettingsHeaderName = "Can Create Unit";
        private const string generateResourcesHeaderName = "Can Generate Resources";
        private const string portraitAndPrefabGroupName = "Portrait & Prefab";
        private const string headerNamePopulation = "Can Give 'Population'";
        private const string headerTurnIntoAnotherEntity = "Can Turn into Another Building'";
        private const string headerToggleNavmesh = "Can Toggle Nav Mesh Obstacle";

        #region Misc
        [VerticalGroup(portraitAndPrefabGroupName + "/Info"), LabelWidth(90)]
        [SerializeField] private string _entityName;

        [VerticalGroup(portraitAndPrefabGroupName + "/Info"), LabelWidth(90)]
        [SerializeField] private EntityType _entityType;

        [VerticalGroup(portraitAndPrefabGroupName + "/Info"), LabelWidth(90)]
        [SerializeField] private KeyCode _hotkey;

        [HorizontalGroup(portraitAndPrefabGroupName, 72)]
        [HideLabel, PreviewField(72, ObjectFieldAlignment.Left)]
        [SerializeField] private Sprite _portrait;

        [HorizontalGroup(portraitAndPrefabGroupName, 72)]
        [Required]
        [HideLabel, PreviewField(72, ObjectFieldAlignment.Left)]
        [SerializeField] private GameObject _prefab;

        [SerializeField] private int _populationUse;

        [ShowIf(nameof(_entityType), EntityType.Building)]
        [SerializeField] private Vector2Int _tileSize = Vector2Int.one;

        [ShowIf(nameof(_entityType), EntityType.Unit)]
        [SerializeField] private float _radius = 0.5f;

        [SerializeField] private HoverPopupData _hoverPopupData;
        #endregion

        #region Health Settings
        [BoxGroup("Health Settings")]
        [SerializeField] private bool _isInvincible = false;

        [BoxGroup("Health Settings")]
        [DisableIf("_isInvincible")]
        [SerializeField] private int _hp = 10;
        #endregion

        #region Spawning Cost Settings
        [BoxGroup("Spawning Settings")]
        [SerializeField] private ResourcesWrapper _spawningCost;

        [BoxGroup("Spawning Settings")]
        [SerializeField] private float _creationDuration;
        #endregion

        #region Vision Settings
        [BoxGroup("Vision Settings")]
        [SerializeField, Range(1, 15)] private float _viewRadius = 3;        

        [BoxGroup("Vision Settings")]
        [SerializeField] private bool _canDetectEntities = false;
        #endregion

        [BoxGroup("Construction")]
        [Tooltip("Is construction like a wall ?")]
        [SerializeField] private bool _isConstructionChained;        

        #region Orders: MOVE, ATTACK, CREATE UNITS, CREATE RESOURCES
        #region Attack     
        [ToggleGroup(nameof(_canAttack), attackSettingsHeaderName)]
        [SerializeField] private bool _canAttack;

        [ToggleGroup(nameof(_canAttack), attackSettingsHeaderName)]
        [SerializeField] private bool _isMelee = true;

        [ToggleGroup(nameof(_canAttack), attackSettingsHeaderName)]
        [DisableIf(nameof(_isMelee))]
        [SerializeField] private GameObject _prefabProjectile;

        [Space]
        [ToggleGroup(nameof(_canAttack), attackSettingsHeaderName)]
        [PositiveValueOnly]
        [SerializeField] private int _damage = 3;

        [ToggleGroup(nameof(_canAttack), attackSettingsHeaderName)]
        [PositiveValueOnly]
        [Tooltip("Time between each attack")]
        [FormerlySerializedAs("_attackSpeed ")]
        [SerializeField] private float _attackPerSecond = 1f;

        [Space]
        [ToggleGroup(nameof(_canAttack), attackSettingsHeaderName)]
        [PositiveValueOnly]
        [Range(0, 15)]
        [SerializeField] private float _attackRadius = 3f;

        [Space]
        [ToggleGroup(nameof(_canAttack), attackSettingsHeaderName)]
        [Sirenix.OdinInspector.ReadOnly]
        [SerializeField] private float _damagePerSecond;
        #endregion

        #region Movement
        [ToggleGroup(nameof(_canMove), movementSettingsHeaderName)]
        [SerializeField] private bool _canMove;

        [ToggleGroup(nameof(_canMove), movementSettingsHeaderName)]
        [Tooltip("Units per second")]
        [PositiveValueOnly]
        [SerializeField] private float _speed = 3;        
        #endregion

        #region Units Creation
        [ToggleGroup(nameof(_canCreateUnit), createUnitSettingsHeaderName)]
        [SerializeField] private bool _canCreateUnit;

        [ToggleGroup(nameof(_canCreateUnit), createUnitSettingsHeaderName)]
        [SerializeField] private UnitSpawnCondition[] _unitsSpawnConditions;        
        #endregion

        #region Resources Generation
        [ToggleGroup(nameof(_canGenerateResources), generateResourcesHeaderName)]
        [SerializeField] private bool _canGenerateResources;

        [Space]
        [ToggleGroup(nameof(_canGenerateResources), "Can Generate Resources")]
        [SerializeField] private float _generationTick;

        [ToggleGroup(nameof(_canGenerateResources), "Can Generate Resources")]
        [SerializeField] private GenerationType _generationType = GenerationType.Constant;

        [ToggleGroup(nameof(_canGenerateResources), "Can Generate Resources")]
        [ShowIf(nameof(_generationType), Value = GenerationType.PerCell)]
        [SerializeField] private Dictionary<CellType, ResourcesWrapper> _resourcesPerCell = new Dictionary<CellType, ResourcesWrapper>();

        [ToggleGroup(nameof(_canGenerateResources), "Can Generate Resources")]
        [ShowIf(nameof(_generationType), Value = GenerationType.PerCell)]
        [SerializeField] private float _radiusToReachCells = 5;

        [ToggleGroup(nameof(_canGenerateResources), "Can Generate Resources")]
        [ShowIf(nameof(_generationType), Value = GenerationType.Constant)]
        [SerializeField] private ResourcesWrapper _constantResourcesGeneration;
        #endregion

        #region Give population
        [ToggleGroup(nameof(_increaseMaxPopulation), headerNamePopulation)]
        [SerializeField] private bool _increaseMaxPopulation;

        [ToggleGroup(nameof(_increaseMaxPopulation), headerNamePopulation)]
        [SerializeField] private int _increaseMaxPopulationAmount;        
        #endregion

        #region Can Turn in Another Building
        [ToggleGroup(nameof(_canTurnIntoAnotherEntity), headerTurnIntoAnotherEntity)]
        [SerializeField] private bool _canTurnIntoAnotherEntity;

        [ToggleGroup(nameof(_canTurnIntoAnotherEntity), headerTurnIntoAnotherEntity)]
        [SerializeField] private string[] _turnIntoAnotherEntityList;
        #endregion

        #region Can Toggle Nav Mesh
        [ToggleGroup(nameof(_canToggleNavMeshObstacle), headerToggleNavmesh)]
        [SerializeField] private bool _canToggleNavMeshObstacle;

        [ToggleGroup(nameof(_canToggleNavMeshObstacle), headerToggleNavmesh)]
        [SerializeField] private OrderContent _orderToggleNavMeshObstacle;
        #endregion
        #endregion
        #endregion

        #region Properties
        public Sprite Portrait { get => _portrait; }
        public KeyCode Hotkey { get => _hotkey; }
        public GameObject Prefab { get => _prefab; }
        public HoverPopupData HoverPopupData { get => _hoverPopupData; }
        public int PopulationUse { get => _populationUse; }
        /// <summary>
        /// The size a building have on the world. TileSize equals to zero if the entity is an unit.
        /// </summary>
        public Vector2Int TileSize { get => _entityType == EntityType.Building ? _tileSize : Vector2Int.zero; }
        public EntityType EntityType { get => _entityType; }
        public string EntityName { get => _entityName; }

        public bool IsInvincible { get => _isInvincible; }
        public int Hp { get => _hp; }

        public ResourcesWrapper SpawningCost { get => _spawningCost; }
        public float CreationDuration { get => _creationDuration; }

        public float ViewRadius { get => _viewRadius; }
        public bool IsConstructionChained { get => _isConstructionChained; }

        public bool CanMove { get => _canMove; }
        public bool CanAttack { get => _canAttack; }
        public bool CanSpawnUnit { get => _canCreateUnit; }
        public bool CanCreateResources { get => _canGenerateResources; }

        public bool IsMelee { get => _isMelee; }
        public GameObject PrefabProjectile { get => _prefabProjectile; }
        public int Damage { get => _damage; }
        public float AttackRadius { get => _attackRadius; }
        public float AttackPerSecond { get => _attackPerSecond; }

        public float Speed { get => _speed; }
        public UnitSpawnCondition[] UnitsSpawnConditions { get => _unitsSpawnConditions; }

        public float GenerationTick { get => _generationTick; }
        public float RadiusToReachCells { get => _radiusToReachCells; }
        public Dictionary<CellType, ResourcesWrapper> ResourcesPerCell { get => _resourcesPerCell; }
        public ResourcesWrapper ConstantResourcesGeneration { get => _constantResourcesGeneration; set => _constantResourcesGeneration = value; }
        public GenerationType GenerationType { get => _generationType; }

        public int IncreaseMaxPopulationAmount { get => _increaseMaxPopulation ? _increaseMaxPopulationAmount : 0; }

        public bool CanToggleNavMeshObstacle { get => _canToggleNavMeshObstacle; }
        public bool CanDetectEntities { get => _canDetectEntities; }

        public bool CanTurnIntoAnotherBuilding { get => _canTurnIntoAnotherEntity; }
        public string[] TurnIntoAnotherBuildingsList { get => _canTurnIntoAnotherEntity ? _turnIntoAnotherEntityList : null; }
        #endregion

        #region Methods
        public float GetRadius()
        {
            switch (_entityType)
            {
                case EntityType.Unit:
                    return _radius;

                case EntityType.Building:
                    return Mathf.Max(_tileSize.x, _tileSize.y);

                default:
                    throw new NotSupportedException();
            }
        }

        [Obsolete("Use GetRadius() instead")]
        public float GetBiggerTileSize()
        {
            return Mathf.Max(_tileSize.x, _tileSize.y);
        }

        public bool CanDoOverallAction(OverallAction overallAction)
        {
            switch (overallAction)
            {
                case OverallAction.Stop:
                    return CanMove && CanAttack;

                case OverallAction.Move:
                    return CanMove;

                case OverallAction.Attack:
                    return CanAttack;

                case OverallAction.Patrol:
                    return CanMove;
            }

            return false;
        }

        void OnValidate()
        {
            _damagePerSecond = _damage * _attackPerSecond;
        }

        public UnitSpawnCondition GetSpawnCondition(string entityID)
        {
            return _unitsSpawnConditions
                .Where(x => x.EntityIDToSpawn == entityID)
                .FirstOrDefault();
        }

        public OrderContent[] GetAvailableOrders()
        {
            List<OrderContent> output = new List<OrderContent>();

            GetUnitsSpawnOrders(output);
            GetTurnIntoEntityOrders(output);
            GetToggleNavMeshObstacleOrder(output);
            GetOverallActionsOrders(output);

            return output.ToArray();
        }

        private void GetToggleNavMeshObstacleOrder(List<OrderContent> output)
        {
            if (!_canToggleNavMeshObstacle)
                return;

            if (_orderToggleNavMeshObstacle.OnClick == null)
                _orderToggleNavMeshObstacle.OnClick = () => SelectedGroupsActionsCaller.OrderToggleNavMeshObstacle();

            output.Add(_orderToggleNavMeshObstacle);
        }

        #region Private Methods
        private void GetUnitsSpawnOrders(List<OrderContent> output)
        {
            if (!_canCreateUnit)
                return;

            foreach (var unitSpawnCondition in _unitsSpawnConditions)
            {
                var entityData = MainRegister.Instance.GetEntityData(unitSpawnCondition.EntityIDToSpawn);

                bool conditionMet = unitSpawnCondition.DoConditionsAreMet();
                Action onClickAction;

                if (conditionMet)
                    onClickAction = () => SelectedGroupsActionsCaller.OrderSpawnUnits(unitSpawnCondition.EntityIDToSpawn);
                else
                    onClickAction = null;

                var order = new OrderContent(
                    entityData.Hotkey,
                    entityData.Portrait,
                    entityData.HoverPopupData,
                    onClickAction,
                    1,
                    conditionMet
                );

                output.Add(order);
            }
        }

        private void GetOverallActionsOrders(List<OrderContent> output)
        {
            foreach (OverallAction overallAction in Enum.GetValues(typeof(OverallAction)))
            {
                if (CanDoOverallAction(overallAction))
                {
                    OverallActionData overallActionData = MainRegister.Instance.GetOverallActionData(overallAction);

                    var order = new OrderContent(
                        overallActionData.Hotkey,
                        overallActionData.Portrait,
                        overallActionData.HoverPopupData,
                        overallAction.ToOrder(),
                        2
                    );

                    output.Add(order);
                }
            }
        }

        private void GetTurnIntoEntityOrders(List<OrderContent> output)
        {
            if (!_canTurnIntoAnotherEntity)
                return;

            foreach (var entityID in _turnIntoAnotherEntityList)
            {
                var entityData = MainRegister.Instance.GetEntityData(entityID);

                var order = new OrderContent(
                    entityData.Hotkey,
                    entityData.Portrait,
                    entityData.HoverPopupData,
                    () => SelectedGroupsActionsCaller.OrderTurnIntoEntities(entityID),
                    1
                );

                output.Add(order);
            }
        }
        #endregion
        #endregion
    }
}
