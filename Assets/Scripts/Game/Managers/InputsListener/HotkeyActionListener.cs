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
    void Start()
    {
        SetHotkeyFromConstructionOrders();
    }

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
            SetHotkeyFromEntityOrders(selectedGroups[highlightGroupIndex].entityID);
        }
        else
        {
            SetHotkeyFromConstructionOrders();
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

                // if a key change the selection, and so the hotkeys, System throws an exception.
                // To avoid that, we return after one hotkey has been pressed.
                return;
            }
        }
    }
    #endregion

    void ClearCommandsHandler()
    {
        _commands.Clear();
    }

    void SetHotkeyFromEntityOrders(string entityIDToListen)
    {
        ClearCommandsHandler();

        var data = MainRegister.Instance.GetEntityData(entityIDToListen);        

        AddHotkeys(data);
    }
    
    void SetHotkeyFromConstructionOrders()
    {
        ClearCommandsHandler();

        var constructionOrders = GameManager.Instance.ManagerData.GetConstructionOrders();

        foreach (var order in constructionOrders)
        {
            AddOrderHotkey(order);
        }
    }

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
        if (_commands.ContainsKey(orderContent.Hotkey))
        {
            Debug.LogWarningFormat("Hotkey {0} is already register. Aborting", orderContent.Hotkey);
            return;
        }

        _commands.Add(orderContent.Hotkey, orderContent.OnClick);
    }    
    #endregion
}
