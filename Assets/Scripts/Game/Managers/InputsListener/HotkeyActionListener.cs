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

    Dictionary<KeyCode, Command> _commands = new Dictionary<KeyCode, Command>();

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
                kvp.Value.Execute();
            }
        }
    }
    #endregion

    public void ClearCommandsHandler()
    {
        _commands.Clear();
    }

    void AddHotkey(KeyCode hotkey, Command command)
    {
        if (_commands.ContainsKey(hotkey))
        {
            Debug.LogErrorFormat("Hotkey {0} is already register. Aborting", hotkey);
            return;
        }

        _commands.Add(hotkey, command);
    }

    void AddHotkeyFromOverallAction(OverallAction overallAction, Command command)
    {
        OverallActionData overallActionData = MainRegister.Instance.GetOverallActionData(overallAction);

        Assert.IsNotNull(overallActionData, "Hotkey Listener: Cannot find hotkey of attack. Aborting listening of hotkey attack.");

        var hotkey = overallActionData.Hotkey;
        AddHotkey(hotkey, command);
    }

    /// <summary>
    /// Clear and re-set _commands dictionary.
    /// </summary>
    /// <param name="entityIDToListen">Type of entity which we listen to hotkey</param>
    public void SetHotkeyHandler(string entityIDToListen)
    {
        ClearCommandsHandler();

        var data = MainRegister.Instance.GetEntityData(entityIDToListen);

        Assert.IsNotNull(data,
            string.Format("Hotkey Listener: cannot find entity data for {0}. Aborting input listening.", entityIDToListen));

        AddHotkeys(data);
    }

    #region Add Hotkeys methods
    void AddHotkeys(EntityData data)
    {
        if (data.CanMove)
            AddMoveHotkeys();

        if (data.CanAttack)
            AddAttackHotkey();

        if (data.CanMove || data.CanAttack)
            AddStopActionHotkey();

        if (data.CanSpawnUnit)
            AddSpawnUnitHotkeys(data.AvailableUnitsForCreation);
    }

    private void AddSpawnUnitHotkeys(string[] unitsIDs)
    {
        foreach (string unitID in unitsIDs)
        {
            var unitData = MainRegister.Instance.GetEntityData(unitID);

            if (unitData != null)
            {
                KeyCode hotkey = unitData.Hotkey;
                AddHotkey(hotkey, new CreateUnitCommand(unitID));
            }
            else
            {
                Debug.LogErrorFormat("Hotkey Listener: Couldn't not find EntityData of unit {0}.", unitID);
                continue;
            }
        }
    }

    private void AddStopActionHotkey()
    {
        AddHotkeyFromOverallAction(OverallAction.Stop, new StopCommand());
    }

    void AddAttackHotkey()
    {
        AddHotkeyFromOverallAction(OverallAction.Attack, new ListenSecondClickToAttackCommand());
    }

    void AddMoveHotkeys()
    {
        AddHotkeyFromOverallAction(OverallAction.Move, new ListenSecondClickToMoveCommand());
        AddHotkeyFromOverallAction(OverallAction.Patrol, new ListenSecondClickToPatrolCommand());
    }
    #endregion
    #endregion
}
