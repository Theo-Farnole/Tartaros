using CommandPattern;
using Game.Selection;
using Lortedo.Utilities.Pattern;
using Registers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    void Start()
    {
        SelectionManager.OnSelectionUpdated += OnSelectionUpdated;   
    }
    void Update()
    {
        ManageHotkeyInput();
    }
    #endregion

    #region Private methods
    /// <summary>
    /// Clear and re-set _commands dictionary.
    /// </summary>
    /// <param name="typeToListenHotkey">Type of entity which we listen to hotkey</param>
    public void SetHotkeyHandler(EntityType typeToListenHotkey)
    {
        ClearCommandsHandler();

        EntityData data = EntitiesRegister.GetRegisterData(typeToListenHotkey);

        // movement
        if (data.CanMove)
        {
            // move hotkey
            var hotkey = OverallActionsRegister.Instance.GetItem(OverallAction.Move).Hotkey;
            _commands.Add(hotkey, new ListenSecondClickToMoveCommand());

            // patrol hotkey
            var patrolHotkey = OverallActionsRegister.Instance.GetItem(OverallAction.Patrol).Hotkey;
            _commands.Add(patrolHotkey, new ListenSecondClickToPatrolCommand());
        }

        // attack
        if (data.CanAttack)
        {
            var hotkey = OverallActionsRegister.Instance.GetItem(OverallAction.Attack).Hotkey;
            _commands.Add(hotkey, new ListenSecondClickToAttackCommand());
        }

        // stop
        if (data.CanMove || data.CanAttack)
        {
            var hotkey = OverallActionsRegister.Instance.GetItem(OverallAction.Stop).Hotkey;
            _commands.Add(hotkey, new StopCommand());
        }

        // CreateUnits
        if (data.CanCreateResources)
        {
            for (int i = 0; i < data.AvailableUnitsForCreation.Length; i++)
            {
                UnitType unit = data.AvailableUnitsForCreation[i];
                KeyCode hotkey = UnitsRegister.Instance.GetItem(unit).Hotkey;

                if (_commands.ContainsKey(hotkey))
                {
                    Debug.LogWarningFormat("Hotkey {0} is already register.", hotkey);
                    continue;
                }
                _commands.Add(UnitsRegister.Instance.GetItem(unit).Hotkey, new CreateUnitCommand(unit));
            }
        }
    }

    public void ClearCommandsHandler()
    {
        _commands.Clear();
    }
    
    private void ManageHotkeyInput()
    {
        foreach (var kvp in _commands)
        {
            if (Input.GetKeyDown(kvp.Key))
            {
                kvp.Value.Execute();
            }
        }
    }

    private void OnSelectionUpdated(SelectionManager.Group[] selectedGroups, int highlightGroupIndex)
    {
        if (selectedGroups.Length > 0 && selectedGroups[0] != null)
        {
            SetHotkeyHandler(selectedGroups[0].entityType);
        }
        else
        {
            ClearCommandsHandler();
        }
    }
    #endregion
    #endregion
}
