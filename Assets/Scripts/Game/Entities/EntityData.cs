using LeonidasLegacy.MapCellEditor;
using Lortedo.Utilities.Inspector;
using MyBox;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UI.Game.HoverPopup;
using UnityEngine;

[System.Flags]
public enum OrdersType
{
    Move = 1 << 0,
    Attack = 1 << 1,
    CreateUnits = 1 << 2,
    CreateResources = 1 << 3
}

[CreateAssetMenu(menuName = "Leonidas Legacy/Entity")]
public class EntityData : SerializedScriptableObject
{
    private const string attackSettingsHeaderName = "Can Attack";
    private const string movementSettingsHeaderName = "Can Move";
    private const string createUnitSettingsHeaderName = "Can Create Unit";
    private const string generateResourcesHeaderName = "Can Generate Resources";
    private const string portraitAndPrefabGroupName = "Portrait & Prefab";

    #region Misc
    [VerticalGroup(portraitAndPrefabGroupName + "/Info"), LabelWidth(90)]
    [SerializeField] private string _entityName;

    [VerticalGroup(portraitAndPrefabGroupName + "/Info"), LabelWidth(90)]
    [SerializeField] private KeyCode _hotkey;

    [VerticalGroup(portraitAndPrefabGroupName + "/Info"), LabelWidth(90)]
    [SerializeField] private int _populationUse;

    [HorizontalGroup(portraitAndPrefabGroupName, 55)]
    [HideLabel, PreviewField(55, ObjectFieldAlignment.Left)]
    [SerializeField] private Sprite _portrait;

    [HorizontalGroup(portraitAndPrefabGroupName, 55)]
    [Required]
    [HideLabel, PreviewField(55, ObjectFieldAlignment.Left)]
    [SerializeField] private GameObject _prefab;

    [InlineEditor]
    [SerializeField] private HoverPopupData _hoverPopupData;


    public Sprite Portrait { get => _portrait; }
    public KeyCode Hotkey { get => _hotkey; }
    public GameObject Prefab { get => _prefab; }
    public HoverPopupData HoverPopupData { get => _hoverPopupData; }
    public int PopulationUse { get => _populationUse; }
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
    [SerializeField] private UnitType[] _availableUnitsForCreation;

    public UnitType[] AvailableUnitsForCreation { get => _availableUnitsForCreation; }
    #endregion

    #region Resources Generation
    [ToggleGroup(nameof(_canGenerateResources), generateResourcesHeaderName)]
    [SerializeField] private bool _canGenerateResources;

    [ToggleGroup(nameof(_canGenerateResources), "Can Generate Resources")]
    [SerializeField] private Dictionary<CellType, ResourcesWrapper> _resourcesPerCell = new Dictionary<CellType, ResourcesWrapper>();

    [Space]
    [ToggleGroup(nameof(_canGenerateResources), "Can Generate Resources")]
    [SerializeField] private float _generationTick;

    [ToggleGroup(nameof(_canGenerateResources), "Can Generate Resources")]
    [SerializeField] private float _radiusToReachCells = 5;

    public float GenerationTick { get => _generationTick; }
    public float RadiusToReachCells { get => _radiusToReachCells; }
    public Dictionary<CellType, ResourcesWrapper> ResourcesPerCell { get => _resourcesPerCell; }
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
