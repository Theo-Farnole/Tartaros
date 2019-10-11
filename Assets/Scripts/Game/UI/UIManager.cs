using Registers;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    #region Fields
    [Header("Game Information Panel")]
    [SerializeField] private GameObject _panelGameInformation;
    [Space]
    [SerializeField] private TextMeshProUGUI _waveIndicator;
    [SerializeField, EnumNamedArray(typeof(Resource))] private TextMeshProUGUI[] _resourcesLabel;

    [Header("Selection Panel")]
    [SerializeField] private GameObject _panelSelection;
    [Space]
    [SerializeField] private UISelectedGroupWrapper[] _selectedGroupWrapper = new UISelectedGroupWrapper[5];
    [SerializeField] private UICommandsWrapper[] _wrapperOverallOrders = new UICommandsWrapper[2];
    [SerializeField] private UICommandsWrapper[] _wrapperSpawnUnitsOrders = new UICommandsWrapper[2];

    [Header("Construction Panel")]
    [SerializeField] private GameObject _panelConstruction;
    [Space]
    [SerializeField, EnumNamedArray(typeof(Building))] private Button[] _buildingButtons;
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

        for (int i = 0; i < _wrapperSpawnUnitsOrders.Length; i++)
        {
            _wrapperSpawnUnitsOrders[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < _buildingButtons.Length; i++)
        {
            Building type = (Building)i + EntitiesSystem.STARTING_INDEX_BUILDING;
            _buildingButtons[i].onClick.AddListener(() => GameManager.Instance.StartBuilding(type));

            _buildingButtons[i].GetComponent<Image>().sprite = Registers.BuildingsRegister.Instance.GetItem(type).Portrait;
        }

        DisplayConstructionPanel();
    }

    void OnValidate()
    {
        if (_resourcesLabel.Length != Enum.GetValues(typeof(Resource)).Length)
        {
            Array.Resize(ref _resourcesLabel, Enum.GetValues(typeof(Resource)).Length);
        }

        if (_buildingButtons.Length != Enum.GetValues(typeof(Building)).Length)
        {
            Array.Resize(ref _buildingButtons, Enum.GetValues(typeof(Building)).Length);
        }
    }
    #endregion

    #region Construction Panel Methods
    public void DisplayConstructionPanel()
    {
        _panelConstruction.SetActive(true);
        _panelSelection.SetActive(false);
    }
    #endregion

    #region Selection Panel Methods
    public void DisplayPanelSelection()
    {
        _panelConstruction.SetActive(false);
        _panelSelection.SetActive(true);
    }

    public void UpdateSelection(SelectionManager.Group[] selectedGroups, int highlightGroupIndex)
    {
        UpdateSelectedGroups(selectedGroups);
        UpdateHighlightGroup(highlightGroupIndex);

        if (highlightGroupIndex >= 0 && highlightGroupIndex < selectedGroups.Length)
        {
            UpdateOrdersPanel(selectedGroups[highlightGroupIndex].selectedEntities[0].Entity.OrdersReceiver);
        }
    }

    void UpdateSelectedGroups(SelectionManager.Group[] selectedGroups)
    {
        for (int i = 0; i < _selectedGroupWrapper.Length; i++)
        {
            if (i >= selectedGroups.Length)
            {
                _selectedGroupWrapper[i].gameObject.SetActive(false);
                continue;
            }

            _selectedGroupWrapper[i].gameObject.SetActive(true);
            _selectedGroupWrapper[i].portrait.sprite = EntitiesRegister.GetRegisterData(selectedGroups[i].entityType).Portrait;
            _selectedGroupWrapper[i].unitsCount.text = selectedGroups[i].selectedEntities.Count.ToString();
        }

    }

    void UpdateHighlightGroup(int highlightGroupIndex)
    {
        for (int i = 0; i < _selectedGroupWrapper.Length; i++)
        {
            bool isHighlight = (i == highlightGroupIndex);

            _selectedGroupWrapper[i].SetHighlight(isHighlight);
        }
    }

    void UpdateOrdersPanel(OrderReceiver orderReceiver)
    {
        // hide every commands wrapper
        for (int i = 0; i < _wrapperSpawnUnitsOrders.Length; i++)
        {
            _wrapperSpawnUnitsOrders[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < _wrapperOverallOrders.Length; i++)
        {
            _wrapperOverallOrders[i].gameObject.SetActive(false);
        }

        if (orderReceiver == null)
            return;

        // assign buttons to OverallAction
        foreach (OverallAction action in Enum.GetValues(typeof(OverallAction)))
        {
            int index = (int)action;

            if (orderReceiver.CanOverallAction(action))
            {
                _wrapperOverallOrders[index].gameObject.SetActive(true);

                _wrapperOverallOrders[index].hotkey.text = OverallActionsRegister.Instance.GetItem(action).Hotkey.ToString();
                _wrapperOverallOrders[index].backgroundButton.sprite = OverallActionsRegister.Instance.GetItem(action).Portrait;

                _wrapperOverallOrders[index].button.onClick.RemoveAllListeners();

                switch (action)
                {
                    case OverallAction.Stop:
                        _wrapperOverallOrders[index].button.onClick.AddListener(() =>
                        {
                            OrderGiverManager.Instance.OrderStop();
                        });
                        break;

                    case OverallAction.Move:
                        _wrapperOverallOrders[index].button.onClick.AddListener(() =>
                        {
                            HotkeyManager.Instance.askCursor = true;
                            HotkeyManager.Instance.askCursorType = HotkeyManager.AskCursor.Move;
                        });
                        break;

                    case OverallAction.Attack:
                        _wrapperOverallOrders[index].button.onClick.AddListener(() =>
                        {
                            HotkeyManager.Instance.askCursor = true;
                            HotkeyManager.Instance.askCursorType = HotkeyManager.AskCursor.Attack;
                        });
                        break;
                }
            }
            else
            {
                _wrapperOverallOrders[index].gameObject.SetActive(false);
            }
        }

        // assign buttons to SpawnUnit
        for (int i = 0; i < _wrapperSpawnUnitsOrders.Length; i++)
        {
            if (i < orderReceiver.CreatableUnits.Length)
            {
                _wrapperSpawnUnitsOrders[i].gameObject.SetActive(true);

                Unit unitType = orderReceiver.CreatableUnits[i];
                _wrapperSpawnUnitsOrders[i].hotkey.text = UnitsRegister.Instance.GetItem(unitType).Hotkey.ToString();
                _wrapperSpawnUnitsOrders[i].backgroundButton.sprite = UnitsRegister.Instance.GetItem(unitType).Portrait;

                _wrapperSpawnUnitsOrders[i].button.onClick.RemoveAllListeners();
                _wrapperSpawnUnitsOrders[i].button.onClick.AddListener(() => OrderGiverManager.Instance.OrderSpawnUnits(unitType));
            }
            else
            {
                _wrapperSpawnUnitsOrders[i].gameObject.SetActive(false);
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
