using Game.MapCellEditor;
using Lortedo.Utilities.Inspector;
using MyBox;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using Game.UI.HoverPopup;
using UnityEngine;

public enum GenerationType
{
    Constant = 1,
    PerCell = 2
}

[CreateAssetMenu(menuName = "Tartaros/Entity")]
public class EntityData : SerializedScriptableObject
{
    private const string attackSettingsHeaderName = "Can Attack";
    private const string movementSettingsHeaderName = "Can Move";
    private const string createUnitSettingsHeaderName = "Can Create Unit";
    private const string generateResourcesHeaderName = "Can Generate Resources";
    private const string portraitAndPrefabGroupName = "Portrait & Prefab";
    private const string headerNamePopulation = "Can Give 'Population'";

    #region Misc
    [VerticalGroup(portraitAndPrefabGroupName + "/Info"), LabelWidth(90)]
    [SerializeField] private string _entityName;

    [VerticalGroup(portraitAndPrefabGroupName + "/Info"), LabelWidth(90)]
    [SerializeField] private KeyCode _hotkey;

    [VerticalGroup(portraitAndPrefabGroupName + "/Info"), LabelWidth(90)]
    [SerializeField] private int _populationUse;

    [VerticalGroup(portraitAndPrefabGroupName + "/Info"), LabelWidth(90)]
    [SerializeField] private Vector2Int _tileSize = Vector2Int.one;

    [HorizontalGroup(portraitAndPrefabGroupName, 72)]
    [HideLabel, PreviewField(72, ObjectFieldAlignment.Left)]
    [SerializeField] private Sprite _portrait;

    [HorizontalGroup(portraitAndPrefabGroupName, 72)]
    [Required]
    [HideLabel, PreviewField(72, ObjectFieldAlignment.Left)]
    [SerializeField] private GameObject _prefab;

    [SerializeField] private HoverPopupData _hoverPopupData;


    public Sprite Portrait { get => _portrait; }
    public KeyCode Hotkey { get => _hotkey; }
    public GameObject Prefab { get => _prefab; }
    public HoverPopupData HoverPopupData { get => _hoverPopupData; }
    public int PopulationUse { get => _populationUse; }
    public Vector2Int TileSize { get => _tileSize; }
    #endregion

    #region Health Settings
    [BoxGroup("Health Settings")]
    [SerializeField] private bool _isInvincible = false;

    [BoxGroup("Health Settings")]
    [DisableIf("_isInvincible")]
    [SerializeField] private int _hp = 10;

    public bool IsInvincible { get => _isInvincible; }
    public int Hp { get => _hp; }
    #endregion

    #region Spawning Cost Settings
    [BoxGroup("Spawning Cost Settings")]
    [SerializeField] private ResourcesWrapper _spawningCost;
    public ResourcesWrapper SpawningCost { get => _spawningCost; }
    #endregion

    #region Vision Settings
    [BoxGroup("Vision Settings")]
    [SerializeField, Range(1, 15)] private float _viewRadius = 3;
    public float ViewRadius { get => _viewRadius; }
    #endregion

    [BoxGroup("Construction")]
    [Tooltip("Is construction like a wall ?")]
    [SerializeField] private bool _isConstructionChained;
    public bool IsConstructionChained { get => _isConstructionChained; }

    #region Orders: MOVE, ATTACK, CREATE UNITS, CREATE RESOURCES
    public bool CanMove { get => _canMove; }
    public bool CanAttack { get => _canAttack; }
    public bool CanSpawnUnit { get => _canCreateUnit; }
    public bool CanCreateResources { get => _canGenerateResources; }

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
    [SerializeField] private float _attackSpeed = 1f;

    [Space]
    [ToggleGroup(nameof(_canAttack), attackSettingsHeaderName)]
    [PositiveValueOnly]
    [Range(0, 15)]
    [SerializeField] private float _attackRadius = 3f;

    [Space]
    [ToggleGroup(nameof(_canAttack), attackSettingsHeaderName)]
    [Sirenix.OdinInspector.ReadOnly]
    [SerializeField] private float _damagePerSecond;

    public bool IsMelee { get => _isMelee; }
    public GameObject PrefabProjectile { get => _prefabProjectile; }
    public int Damage { get => _damage; }
    public float AttackRadius { get => _attackRadius; }
    public float AttackSpeed { get => _attackSpeed; }
    #endregion

    #region Movement
    [ToggleGroup(nameof(_canMove), movementSettingsHeaderName)]
    [SerializeField] private bool _canMove;

    [ToggleGroup(nameof(_canMove), movementSettingsHeaderName)]
    [Tooltip("Units per second")]
    [PositiveValueOnly]
    [SerializeField] private float _speed = 3;

    public float Speed { get => _speed; }
    #endregion

    #region Units Creation
    [ToggleGroup(nameof(_canCreateUnit), createUnitSettingsHeaderName)]
    [SerializeField] private bool _canCreateUnit;

    [ToggleGroup(nameof(_canCreateUnit), createUnitSettingsHeaderName)]
    [SerializeField] private string[] _availableUnitsForCreation;

    public string[] AvailableUnitsForCreation { get => _availableUnitsForCreation; }
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

    public float GenerationTick { get => _generationTick; }
    public float RadiusToReachCells { get => _radiusToReachCells; }
    public Dictionary<CellType, ResourcesWrapper> ResourcesPerCell { get => _resourcesPerCell; }
    public ResourcesWrapper ConstantResourcesGeneration { get => _constantResourcesGeneration; set => _constantResourcesGeneration = value; }
    public GenerationType GenerationType { get => _generationType; }
    #endregion

    #region Give population
    [ToggleGroup(nameof(_increaseMaxPopulation), headerNamePopulation)]
    [SerializeField] private bool _increaseMaxPopulation;

    [ToggleGroup(nameof(_increaseMaxPopulation), headerNamePopulation)]
    [SerializeField] private int _increaseMaxPopulationAmount;
    
    public int IncreaseMaxPopulationAmount { get => _increaseMaxPopulation ? _increaseMaxPopulationAmount : 0; }
    #endregion
    #endregion

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
        _damagePerSecond = _damage / _attackSpeed;
    }
}
