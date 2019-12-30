using Lortedo.Utilities.Inspector;
using MyBox;
using System.Collections;
using System.Collections.Generic;
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
public class EntityData : ScriptableObject
{
    #region Misc
    [SerializeField] private Sprite _portrait;
    [SerializeField] private GameObject _prefab;
    [Space]
    [SerializeField] private KeyCode _hotkey;

    public Sprite Portrait { get => _portrait; }
    public KeyCode Hotkey { get => _hotkey; }
    public GameObject Prefab { get => _prefab; }
    #endregion

    #region Health Settings
    [Header("Health Settings")]
    [SerializeField] private bool _isInvincible = false;
    public bool IsInvincible { get => _isInvincible; }

    [DrawIf("_isInvincible", false, ComparisonType.Equals, DisablingType.ReadOnly)]
    [SerializeField] private int _hp = 10;
    public int Hp { get => _hp; }
    #endregion

    #region Spawning Cost Settings
    [Header("Spawning Cost Settings")]
    [SerializeField] private ResourcesWrapper _spawningCost;
    public ResourcesWrapper SpawningCost { get => _spawningCost; }
    #endregion

    #region Vision Settings
    [Header("Vision Settings")]
    [SerializeField, Range(1, 15)] private float _viewRadius = 3;
    public float ViewRadius { get => _viewRadius; }
    #endregion

    #region Orders: MOVE, ATTACK, CREATE UNITS, CREATE RESOURCES
    [Header("Orders")]
    [SerializeField, EnumFlag] private OrdersType _availableOrders;    

    public bool CanMove { get => _availableOrders.HasFlag(OrdersType.Move); }
    public bool CanAttack { get => _availableOrders.HasFlag(OrdersType.Attack); }
    public bool CanSpawnUnit { get => _availableOrders.HasFlag(OrdersType.CreateUnits); }
    public bool CanCreateResources { get => _availableOrders.HasFlag(OrdersType.CreateResources); }

    #region Attack
    [Separator("Attack Settings", true)]
    [Space]
    [DrawIf(nameof(_availableOrders), OrdersType.Attack, ComparisonType.HasFlag, DisablingType.ReadOnly)]
    [SerializeField] private bool _isMelee = true;
    [DrawIf(nameof(_isMelee), false, ComparisonType.Equals, DisablingType.DontDraw)]
    [SerializeField] private GameObject _prefabProjectile;
    [Space]
    [DrawIf(nameof(_availableOrders), OrdersType.Attack, ComparisonType.HasFlag, DisablingType.ReadOnly)]
    [PositiveValueOnly, SerializeField] private int _damage = 3;
    [DrawIf(nameof(_availableOrders), OrdersType.Attack, ComparisonType.HasFlag, DisablingType.ReadOnly)]
    [PositiveValueOnly, SerializeField, Tooltip("Time between each attack")] private float _attackSpeed = 1f;
    [Space]
    [DrawIf(nameof(_availableOrders), OrdersType.Attack, ComparisonType.HasFlag, DisablingType.ReadOnly)]
    [PositiveValueOnly, SerializeField, Range(0, 15)] private float _attackRadius = 3f;
    [Space]
    [SerializeField, ShowOnly] private float _damagePerSecond;

    public bool IsMelee { get => _isMelee; }
    public GameObject PrefabProjectile { get => _prefabProjectile;}
    public int Damage { get => _damage; }
    public float AttackRadius { get => _attackRadius; }
    public float AttackSpeed { get => _attackSpeed; }
    #endregion

    #region Movement
    [Separator("Movement Settings", true)]
    [Space]
    [DrawIf(nameof(_availableOrders), OrdersType.Move, ComparisonType.HasFlag, DisablingType.ReadOnly)]
    [SerializeField, PositiveValueOnly, Tooltip("Units per second")] private float _speed = 3;

    public float Speed { get => _speed; }
    #endregion

    #region Units Creation
    [Separator("Units Creation Settings", true)]
    [Space]
    [DrawIf(nameof(_availableOrders), OrdersType.CreateUnits, ComparisonType.HasFlag, DisablingType.ReadOnly)]
    [SerializeField] private UnitType[] _availableUnitsForCreation;

    public UnitType[] AvailableUnitsForCreation { get => _availableUnitsForCreation; }
    #endregion

    #region Resources Generation
    [Separator("Resources Generation", true)]
    [Space]
    [DrawIf(nameof(_availableOrders), OrdersType.CreateResources, ComparisonType.HasFlag, DisablingType.ReadOnly)]
    [SerializeField, PositiveValueOnly, Tooltip("In seconds")] private float _generationTick;
    [SerializeField] private ResourcesWrapper _resourcesAmount;

    public float GenerationTick { get => _generationTick; }    
    public ResourcesWrapper ResourcesAmount { get => _resourcesAmount; }
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
        }

        return false;
    }

    void OnValidate()
    {
        _damagePerSecond = _damage / _attackSpeed;
    }
}
