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
    [SerializeField] private UISelectedGroupWrapper[] _selectedGroupWrapper = new UISelectedGroupWrapper[5];
    #endregion

    #region Methods
    void Awake()
    {
        _waveIndicator.gameObject.SetActive(false);

        for (int i = 0; i < _selectedGroupWrapper.Length; i++)
        {
            _selectedGroupWrapper[i].gameObject.SetActive(false);
        }
    }

    public void UpdateResourcesLabel(ResourcesWrapper currentResources)
    {
        _resourcesLabel[(int)Resource.Food].text = "food " + currentResources.food;
        _resourcesLabel[(int)Resource.Wood].text = "wood " + currentResources.wood;
        _resourcesLabel[(int)Resource.Gold].text = "gold " + currentResources.gold;
    }

    public void UpdateSelectedGroups(KeyValuePair<SelectionManager.SelectionKey, List<SelectableEntity>>[] selectedGroups)
    {
        for (int i = 0; i < _selectedGroupWrapper.Length; i++)
        {
            if (i >= selectedGroups.Length)
            {
                _selectedGroupWrapper[i].gameObject.SetActive(false);
                continue;
            }

            _selectedGroupWrapper[i].gameObject.SetActive(true);
            _selectedGroupWrapper[i].portrait.sprite = PortraitsManager.GetPortrait(selectedGroups[i].Key.entityType);
            _selectedGroupWrapper[i].unitsCount.text = selectedGroups[i].Value.Count.ToString();
        }
    }

    public void UpdateHighlightGroup(int highlightGroupIndex)
    {
        for (int i = 0; i < _selectedGroupWrapper.Length; i++)
        {
            bool isHighlight = (i == highlightGroupIndex);

            _selectedGroupWrapper[i].SetHighlight(isHighlight);
        }
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

    void OnValidate()
    {
        if (_resourcesLabel.Length != Enum.GetValues(typeof(Resource)).Length)
        {
            Array.Resize(ref _resourcesLabel, Enum.GetValues(typeof(Resource)).Length);
        }
    }
    #endregion
}
