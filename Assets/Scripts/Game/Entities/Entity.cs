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
    [SerializeField] private Owner _owner;
    [Header("Entity Config")]
    [SerializeField] protected HealthData _entityData;
    [SerializeField] public bool isInvincible = false;
    [Header("Entity Config")]
    [SerializeField] protected Slider _healthSlider;
    [SerializeField] private bool _hideHealthSliderIfFull = true;

    private Dictionary<float, AttackSlots> _rangeToSlots = new Dictionary<float, AttackSlots>();
    private int _hp;
    private int _maxHp;
    #endregion

    #region Properties
    public int MaxHp { get => _maxHp; }
    public int Hp { get => _hp; }
    public bool IsAlive { get => _hp > 0 ? true : false; }
    public Owner Owner { get => _owner; }
    #endregion

    #region Methods
    #region MonoBehaviour Callbacks
    void Awake()
    {
        if (_entityData != null)
        {
            _hp = _entityData.Hp;
            _maxHp = _entityData.Hp;

            UpdateHealthSlider();
        }
        else
        {
            Debug.Log("Il manque une entity data pour " + transform.name);
        }
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
        if (isInvincible)
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
