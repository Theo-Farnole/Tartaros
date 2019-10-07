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
public class Entity : MonoBehaviour
{
    #region Fields
    [Header("Owner Configuration")]
    [SerializeField] private EntityType _type;
    [SerializeField, ReadOnly] private Owner _owner;
    [Header("Health Slider Behaviour")]
    [SerializeField] protected Slider _healthSlider;
    [SerializeField] private bool _hideHealthSliderIfFull = true;
    [Header("Fog of War Settings")]
    [SerializeField] private FOWEntity _fowEntity;

    private CommandsReceiver _commandsReceiver;
    private EntityData _entityData;

    private Dictionary<float, AttackSlots> _rangeToSlots = new Dictionary<float, AttackSlots>();
    private int _hp;
    private int _maxHp;
    #endregion

    #region Properties
    public int MaxHp { get => _maxHp; }
    public int Hp { get => _hp; }
    public bool IsAlive { get => _hp > 0 ? true : false; }

    public Owner Owner { get => _owner; }
    public EntityType Type { get => _type; }

    public EntityData Data { get => _entityData; }
    public FOWEntity FowEntity { get => _fowEntity; }
    public CommandsReceiver CommandReceiver { get => _commandsReceiver; }
    #endregion

    #region Methods
    #region MonoBehaviour Callbacks
    void Awake()
    {
        _owner = _type.GetOwner();
        _commandsReceiver = new CommandsReceiver(this);
    }

    void Start()
    {
        _entityData = DataRegister.GetData(_type);
        _fowEntity.Init(this);

        _hp = _entityData.Hp;
        _maxHp = _entityData.Hp;

        UpdateHealthSlider();
    }

    void Update()
    {
        _commandsReceiver.Tick();
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

    #region AttackSlots Methods
    public AttackSlots GetAttackSlots(float slotRange)
    {
        // add entry in dictionary if nonexistant
        if (_rangeToSlots.ContainsKey(slotRange) == false)
        {
            _rangeToSlots.Add(slotRange, new AttackSlots(transform, slotRange, GameManager.Instance.AttackSlotsData.DistanceBetweenSlot));
        }

        return _rangeToSlots[slotRange];
    }
    #endregion

    #region Health, death & UI Methods
    /// <summary>
    /// GoDamage to this entity.
    /// </summary>
    /// <param name="damage">Amount of hp to be removed</param>
    /// <param name="attacker">Entity which do damage to the entity</param>
    public void GetDamage(int damage, Entity attacker)
    {
        if (_entityData.IsInvincible)
            return;

        _hp -= damage;
        _hp = Mathf.Clamp(_hp, 0, _maxHp);

        UpdateHealthSlider();

        if (!IsAlive)
        {
            Death(attacker);
        }
    }

    /// <summary>
    /// Play feedbacks, than destroy entity.
    /// </summary>
    /// <param name="killer">Entity which removed the last hp of the entity</param>
    private void Death(Entity killer)
    {
        Destroy(gameObject);
    }

    /// <summary>
    /// Update value of health slider, and hide it if option is ticked.
    /// </summary>
    private void UpdateHealthSlider()
    {
        if (_healthSlider == null)
            return;

        // active or not the slider
        if (_hideHealthSliderIfFull)
        {
            bool isFullLife = (_hp == _maxHp);
            _healthSlider.gameObject.SetActive(!isFullLife);
        }

        // update the value
        _healthSlider.maxValue = _maxHp;
        _healthSlider.value = _hp;
    }
    #endregion
    #endregion
}
