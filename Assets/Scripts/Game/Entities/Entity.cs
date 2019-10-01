using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


[SelectionBase]
public class Entity : MonoBehaviour
{
    #region Fields
    [Header("Entity Config")]
    [SerializeField] protected HealthData _entityData;
    [SerializeField] public bool isInvincible = false;
    [Space]
    [SerializeField] protected Slider _healthSlider;
    [SerializeField] private bool _hideHealthSliderIfFull = true;

    protected int _hp;
    private int _maxHp;
    #endregion

    #region Properties
    public int MaxHp { get => _maxHp; }
    public int Hp { get => _hp; }
    public bool IsAlive { get => _hp > 0 ? true : false; }
    #endregion

    #region Methods
    protected virtual void Awake()
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

    /**
     * Reduce entity's HP.
     */
    virtual public void GetDamage(int damage, Entity attacker)
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

    protected void UpdateHealthSlider()
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

    protected virtual void Death(Entity killer)
    {
        Destroy(gameObject);
    }
    #endregion
}
