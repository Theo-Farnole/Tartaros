using CommandPattern;
using Game.Selection;
using Lortedo.Utilities.Pattern;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public class HotkeyActionListener : MonoBehaviour
{
    public enum AskCursor
    {
        None = 0,
        Move = 1,
        Attack = 2
    }

    Dictionary<KeyCode, Action> _commands = new Dictionary<KeyCode, Action>();

    #region Methods
    #region MonoBehaviour Callbacks
    void Update()
    {
        ManageHotkeyInputs();
    }

    void OnEnable()
    {
        SelectionManager.OnSelectionUpdated += OnSelectionUpdated;
    }

    void OnDisable()
    {
        SelectionManager.OnSelectionUpdated -= OnSelectionUpdated;
    }
    #endregion

    #region Events Handlers
    private void OnSelectionUpdated(SelectionManager.SelectionGroup[] selectedGroups, int highlightGroupIndex)
    {
        if (selectedGroups.Length > 0 && selectedGroups[0] != null)
        {
            Assert.IsTrue(selectedGroups.IsIndexInsideBounds(highlightGroupIndex), "Should not happen: the passed group from SelectionManager should always have an index inside bounds.");

            SetHotkeyHandler(selectedGroups[highlightGroupIndex].entityID);
        }
        else
        {
            ClearCommandsHandler();
        }
    }
    #endregion

    #region Inputs management
    private void ManageHotkeyInputs()
    {
        foreach (var kvp in _commands)
        {
            if (Input.GetKeyDown(kvp.Key))
            {
                kvp.Value?.Invoke();
            }
        }
    }
    #endregion

    public void ClearCommandsHandler()
    {
        _commands.Clear();
    }

    /// <summary>
    /// Clear and re-set _commands dictionary.
    /// </summary>
    /// <param name="entityIDToListen">Type of entity which we listen to hotkey</param>
    public void SetHotkeyHandler(string entityIDToListen)
    {
        ClearCommandsHandler();

        var data = MainRegister.Instance.GetEntityData(entityIDToListen);        

        AddHotkeys(data);
    }

    #region Add Hotkeys methods
    void AddHotkeys(EntityData data)
    {
        Assert.IsNotNull(data, string.Format("Hotkey Listener: cannot find entity data. Aborting input listening."));

        var orders = data.GetAvailableOrders();

        foreach (var order in orders)
        {
            AddOrderHotkey(order);
        }
    }

    void AddOrderHotkey(OrderContent orderContent)
    {
        AddHotkey(orderContent.Hotkey, orderContent.OnClick);
    }

    void AddHotkey(KeyCode hotkey, Action action)
    {
        if (_commands.ContainsKey(hotkey))
        {
            Debug.LogErrorFormat("Hotkey {0} is already register. Aborting", hotkey);
            return;
        }

        _commands.Add(hotkey, action);
    }
    #endregion
    #endregion
}
