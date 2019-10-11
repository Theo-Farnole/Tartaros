using FogOfWar;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Owner
{
    Sparta,
    Persian,
    Nature
}

[SelectionBase]
public class Entity : MonoBehaviour, IEntityKilled
{
    #region Fields
    [Header("Owner Configuration")]
    [SerializeField] private EntityType _type;
    [SerializeField, ReadOnly] private Owner _owner;
    [Header("Miscellaneous Components")]
    [SerializeField] private HeathComponent _healthComponent;
    [SerializeField] private FOWEntity _fowEntity;

    private OrderReceiver _ordersReceiver;
    private EntityData _entityData;

    private Dictionary<float, AttackSlots> _rangeToSlots = new Dictionary<float, AttackSlots>();
    #endregion

    #region Properties
    public Owner Owner { get => _owner; }
    public EntityType Type { get => _type; }

    public EntityData Data { get => _entityData; }

    public FOWEntity FowEntity { get => _fowEntity; }
    public OrderReceiver OrdersReceiver { get => _ordersReceiver; }
    public HeathComponent HealthComponent { get => _healthComponent; }
    #endregion

    #region Methods
    #region MonoBehaviour Callbacks
    void Awake()
    {
        _owner = _type.GetOwner();
        _ordersReceiver = new OrderReceiver(this);
    }

    void Start()
    {
        _entityData = Registers.EntitiesRegister.GetRegisterData(_type).EntityData;

        _fowEntity.Init(this);
        _healthComponent.Init(this);
    }

    void Update()
    {
        _ordersReceiver.Tick();
    }

    void OnValidate()
    {
        _owner = _type.GetOwner();
    }

    void OnDestroy()
    {
        _fowEntity.RemoveFromFOWManager();
    }

    void OnDrawGizmos()
    {
        if (DebugUtils.Active)
        {
            foreach (var attackSlots in _rangeToSlots)
            {
                attackSlots.Value.GizmosDrawSlots();
            }
        }
    }
    #endregion

    public AttackSlots GetAttackSlots(float slotRange)
    {
        // add entry in dictionary if nonexistant
        if (_rangeToSlots.ContainsKey(slotRange) == false)
        {
            _rangeToSlots.Add(slotRange, new AttackSlots(transform, slotRange, GameManager.Instance.AttackSlotsData.DistanceBetweenSlot));
        }

        return _rangeToSlots[slotRange];
    }

    /// <summary>
    /// Play feedbacks, than destroy entity.
    /// </summary>
    /// <param name="killer">Entity which removed the last hp of the entity</param>
    void IEntityKilled.Death(Entity killer)
    {
        Destroy(gameObject);
    }
    #endregion
}
