using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EntityHealth : EntityComponent
{
    [Header("Health Slider Behaviour")]
    [SerializeField] private Canvas _sliderCanvas;
    [SerializeField] private Slider _healthSlider;
    [SerializeField] private bool _hideHealthSliderIfFull = true;

    private int _hp;
    private int _maxHp;

    public int MaxHp { get => _maxHp; }
    public int Hp { get => _hp; }
    public bool IsAlive { get => _hp > 0 ? true : false; }

    void Start()
    {
        _hp = Entity.Data.Hp;
        _maxHp = Entity.Data.Hp;

        UpdateHealthSlider();
    }

    /// <summary>
    /// GoDamage to this entity.
    /// </summary>
    /// <param name="damage">Amount of hp to be removed</param>
    /// <param name="attacker">Entity which do damage to the entity</param>
    public void GetDamage(int damage, Entity attacker)
    {
        if (Entity.Data.IsInvincible)
            return;

        _hp -= damage;
        _hp = Mathf.Clamp(_hp, 0, _maxHp);

        UpdateHealthSlider();

        if (!IsAlive)
        {
            Entity.Death();
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
            bool sliderCanvasActivation = !isFullLife;

            // only set active if needed
            if (_sliderCanvas.gameObject.activeSelf != sliderCanvasActivation)
            {
                _sliderCanvas.gameObject.SetActive(sliderCanvasActivation);
            }
        }

        // update the value
        _healthSlider.maxValue = _maxHp;
        _healthSlider.value = _hp;
    }
}
