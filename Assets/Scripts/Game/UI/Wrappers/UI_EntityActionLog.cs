using Game.Selection;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(TextMeshProUGUI))]
public class UI_EntityActionLog : MonoBehaviour
{
    private TextMeshProUGUI _textMeshProUGUI;

    private Entity _currentEntity;

    public Entity CurrentEntity { get => _currentEntity; set => _currentEntity = value; }

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
    }

    void OnDisable()
    {
        SelectionManager.OnSelectionUpdated -= SelectionManager_OnSelectionUpdated;
    }

    void SelectionManager_OnSelectionUpdated(SelectionManager.SelectionGroup[] selectedGroups, int highlightGroupIndex)
    {
        if (selectedGroups.Length == 0)
        {
            _currentEntity = null;
        }
        else
        {
            Assert.IsTrue(selectedGroups.IsIndexInsideBounds(highlightGroupIndex));
            var group = selectedGroups[highlightGroupIndex];

            Assert.IsTrue(group.unitsSelected.IsIndexInsideBounds(0));
            var firstEntity = group.unitsSelected[0];

            Assert.IsNotNull(firstEntity);
            _currentEntity = firstEntity;
        }
    }

    void UpdateEntityActionsLog()
    {
        bool currentEntitySelected = _currentEntity != null;

        _textMeshProUGUI.enabled = currentEntitySelected;

        if (currentEntitySelected)
        {
            _textMeshProUGUI.text = _currentEntity.ActionsToString();
        }
    }
}
