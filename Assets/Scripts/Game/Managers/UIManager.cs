using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    #region Fields
    [SerializeField] private Image _selectedPortrait;
    [SerializeField] private TextMeshProUGUI _waveIndicator;
    [Space]
    [EnumNamedArray(typeof(Resource))]
    [SerializeField] private TextMeshProUGUI[] _resourcesLabel;
    #endregion

    #region Methods
    void Awake()
    {
        _waveIndicator.gameObject.SetActive(false);
    }

    public void UpdateResourcesLabel(ResourcesWrapper currentResources)
    {
        _resourcesLabel[(int)Resource.Food].text = "food " + currentResources.food;
        _resourcesLabel[(int)Resource.Wood].text = "wood " + currentResources.wood;
        _resourcesLabel[(int)Resource.Gold].text = "gold " + currentResources.gold;
    }

    public void SetWaveText(int waveCount, float remainingTime)
    {
        int remainingMinutes = Mathf.FloorToInt(remainingTime / 60);
        int remainingSeconds = Mathf.FloorToInt(remainingTime % 60);

        string stringTime = remainingMinutes.ToString();

        if (remainingMinutes <= 5)
        {
            stringTime = string.Format("{0}:{1:00}", remainingMinutes, remainingSeconds);
        }

        _waveIndicator.gameObject.SetActive(true);
        _waveIndicator.text = "Wave #" + waveCount + " in " + stringTime + " minutes"; 
    }

    public void SetSelectedPortrait(EntityType ent)
    {
        Sprite sprite = null;

        var entUnitType = ent.IsUnitType();
        if (entUnitType != null)
        {
            sprite = UnitsPortraitsRegister.Instance?.GetItem((Unit)entUnitType);
        }
        else
        {
            var entBuildingType = ent.IsBuildingType();

            if (entBuildingType != null)
            {
                sprite = BuildingsPortraitsRegister.Instance?.GetItem((Building)entBuildingType);
            }
        }

        _selectedPortrait.sprite = sprite;
    }

    public void ResetSelectedPortrait()
    {
        _selectedPortrait.sprite = null;
    }

    void OnValidate()
    {
        if (_resourcesLabel.Length != Enum.GetValues(typeof(Resource)).Length)
        {
            Array.Resize(ref _resourcesLabel, Enum.GetValues(typeof(Resource)).Length);
        }
    }
    #endregion
}
