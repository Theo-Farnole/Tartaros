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
    [Space]
    [SerializeField] private UICommandsWrapper[] _commandsWrapper = new UICommandsWrapper[2];
    #endregion

    #region Methods
    #region MonoBehaviour Callbacks
    void Awake()
    {
        _waveIndicator.gameObject.SetActive(false);

        for (int i = 0; i < _selectedGroupWrapper.Length; i++)
        {
            _selectedGroupWrapper[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < _commandsWrapper.Length; i++)
        {
            _commandsWrapper[i].gameObject.SetActive(false);
        }
    }

    void OnValidate()
    {
        if (_resourcesLabel.Length != Enum.GetValues(typeof(Resource)).Length)
        {
            Array.Resize(ref _resourcesLabel, Enum.GetValues(typeof(Resource)).Length);
        }
    }
    #endregion

    #region SelectGroups Methods
    public void UpdateSelectedGroups(SelectionManager.Group[] selectedGroups, int highlightGroupIndex)
    {
        for (int i = 0; i < _selectedGroupWrapper.Length; i++)
        {
            if (i >= selectedGroups.Length)
            {
                _selectedGroupWrapper[i].gameObject.SetActive(false);
                continue;
            }

            _selectedGroupWrapper[i].gameObject.SetActive(true);
            _selectedGroupWrapper[i].portrait.sprite = PortraitsManager.GetPortrait(selectedGroups[i].entityType);
            _selectedGroupWrapper[i].unitsCount.text = selectedGroups[i].selectedEntities.Count.ToString();
        }


        UpdateHighlightGroup(highlightGroupIndex);

        CommandsReceiverEntity commandReceiver = null;

        if (highlightGroupIndex != -1)
        {
            commandReceiver = selectedGroups[highlightGroupIndex].selectedEntities[0].GetComponent<CommandsReceiverEntity>();
        }

        UpdateCommandsPanel(commandReceiver);
    }

    void UpdateHighlightGroup(int highlightGroupIndex)
    {
        for (int i = 0; i < _selectedGroupWrapper.Length; i++)
        {
            bool isHighlight = (i == highlightGroupIndex);

            _selectedGroupWrapper[i].SetHighlight(isHighlight);
        }
    }

    void UpdateCommandsPanel(CommandsReceiverEntity commandReceiver)
    {
        // hide every commands wrapper
        for (int i = 0; i < _commandsWrapper.Length; i++)
        {
            _commandsWrapper[i].gameObject.SetActive(false);
        }

        if (commandReceiver == null)
            return;

        // assign buttons to SpawnUnit Command
        var creatableUnits = commandReceiver.CreatableUnits;

        for (int i = 0; i < _commandsWrapper.Length; i++)
        {
            bool isActive = (i < creatableUnits.Length);
            _commandsWrapper[i].gameObject.SetActive(isActive);

            if (isActive)
            {
                _commandsWrapper[i].commandLabel.text = creatableUnits[i].Type.ToString();
                //_commandsWrapper[i].backgroundButton.sprite = UnitsPortraitsRegister.Instance.GetItem(creatableUnits[i].Type);

                var type = creatableUnits[i].Type;
                _commandsWrapper[i].button.onClick.RemoveAllListeners();
                _commandsWrapper[i].button.onClick.AddListener(() => commandReceiver.SpawnUnit(type));
            }
        }
    }
    #endregion

    #region GameManager Methods
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
    #endregion
    #endregion
}
