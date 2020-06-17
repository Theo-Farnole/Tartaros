using Game.Entities;
using Game.Selection;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(TextMeshProUGUI))]
public class UI_EntityActionLog : MonoBehaviour
{
    #region Fields
    private TextMeshProUGUI _textMeshProUGUI;

    private Entity _currentEntity;
    #endregion

    #region Properties
    public Entity CurrentEntity { get => _currentEntity; set => _currentEntity = value; }
    #endregion

    #region Methods
    #region MonoBehaviour Callbacks
    void Awake()
    {
        _textMeshProUGUI = GetComponent<TextMeshProUGUI>();
        Assert.IsNotNull(_textMeshProUGUI, "Missing TextMeshProUGUI in UI_EntityActionLog.");
    }

    void Update()
    {
        UpdateEntityActionsLog();
    }

    void OnEnable()
    {
        SelectionManager.OnSelectionUpdated += SelectionManager_OnSelectionUpdated;
        SelectionManager.OnSelectionClear += SelectionManager_OnSelectionClear;
    }

    void OnDisable()
    {
        SelectionManager.OnSelectionUpdated -= SelectionManager_OnSelectionUpdated;
        SelectionManager.OnSelectionClear -= SelectionManager_OnSelectionClear;
    }
    #endregion

    #region Events Handlers
    private void SelectionManager_OnSelectionUpdated(SelectionManager.SelectionGroup[] selectedGroups, int highlightGroupIndex) => _currentEntity = selectedGroups[highlightGroupIndex].unitsSelected[0];

    private void SelectionManager_OnSelectionClear() => _currentEntity = null;
    #endregion

    #region Private Methods
    void UpdateEntityActionsLog()
    {
        bool currentEntitySelected = _currentEntity != null;

        _textMeshProUGUI.enabled = currentEntitySelected;

        if (currentEntitySelected)
        {
            _textMeshProUGUI.text = _currentEntity.ActionsToString();
        }
    }
    #endregion
    #endregion
}
