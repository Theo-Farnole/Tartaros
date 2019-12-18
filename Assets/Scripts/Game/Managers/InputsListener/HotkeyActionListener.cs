using CommandPattern;
using Lortedo.Utilities.Pattern;
using Registers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotkeyActionListener : Singleton<HotkeyActionListener>
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

        RegisterData data = EntitiesRegister.GetRegisterData(typeToListenHotkey);

        // move
        if (data.EntityData.CanMove)
        {
            var hotkey = OverallActionsRegister.Instance.GetItem(OverallAction.Move).Hotkey;
            _commands.Add(hotkey, new ListenSecondClickToMoveCommand());
        }

        // attack
        if (data.EntityData.CanAttack)
        {
            var hotkey = OverallActionsRegister.Instance.GetItem(OverallAction.Attack).Hotkey;
            _commands.Add(hotkey, new ListenSecondClickToAttackCommand());
        }

        // stop
        if (data.EntityData.CanMove || data.EntityData.CanAttack)
        {
            var hotkey = OverallActionsRegister.Instance.GetItem(OverallAction.Stop).Hotkey;
            _commands.Add(hotkey, new StopCommand());
        }

        // CreateUnits
        if (data.EntityData.CanCreateResources)
        {
            for (int i = 0; i < data.EntityData.AvailableUnitsForCreation.Length; i++)
            {
                UnitType unit = data.EntityData.AvailableUnitsForCreation[i];
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
        foreach (KeyCode vKey in System.Enum.GetValues(typeof(KeyCode)))
        {
            if (Input.GetKeyDown(vKey))
            {
                if (_commands.ContainsKey(vKey))
                {
                    _commands[vKey].Execute();
                }
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
