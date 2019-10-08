using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class HeathComponent
{
    #region Fields
    [Header("Health Slider Behaviour")]
    [SerializeField] protected Slider _healthSlider;
    [SerializeField] private bool _hideHealthSliderIfFull = true;

    private int _hp;
    private int _maxHp;

    private Entity _entity;
    #endregion

    #region Properties
    public int MaxHp { get => _maxHp; }
    public int Hp { get => _hp; }
    public bool IsAlive { get => _hp > 0 ? true : false; }
    #endregion

    #region Methods
    public void Init(Entity ent)
    {
        _entity = ent;

        _hp = _entity.Data.Hp;
        _maxHp = _entity.Data.Hp;

        UpdateHealthSlider();
    }

    /// <summary>
    /// GoDamage to this entity.
    /// </summary>
    /// <param name="damage">Amount of hp to be removed</param>
    /// <param name="attacker">Entity which do damage to the entity</param>
    public void GetDamage(int damage, Entity attacker)
    {
        if (_entity.Data.IsInvincible)
            return;

        _hp -= damage;
        _hp = Mathf.Clamp(_hp, 0, _maxHp);

        UpdateHealthSlider();

        if (!IsAlive)
        {
            _entity.GetComponent<IEntityKilled>().Death(attacker);
        }
    }

    void UpdateHealthSlider()
    {
        if (_healthSlider == null)
            return;

        // hide or not the slider
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
}
