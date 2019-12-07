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

    [HideInInspector] public bool waitingForMouseClick = false;
    [HideInInspector] public AskCursor waitingForInputCursor = AskCursor.None;

    Dictionary<KeyCode, Command> _commands = new Dictionary<KeyCode, Command>();

    #region Methods
    #region MonoBehaviour Callbacks
    void Update()
    {
        ManageHotkeyInput();
    }

    void LateUpdate()
    {
        ManageAskCursor();
    }
    #endregion

    #region Public methods
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
            _commands.Add(hotkey, new WaitForMoveDestinationCommand());
        }

        // attack
        if (data.EntityData.CanAttack)
        {
            _commands.Add(OverallActionsRegister.Instance.GetItem(OverallAction.Attack).Hotkey, new WaitForAttackTargetCommand());
        }

        // stop
        if (data.EntityData.CanMove || data.EntityData.CanAttack)
        {
            _commands.Add(OverallActionsRegister.Instance.GetItem(OverallAction.Stop).Hotkey, new StopCommand());
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
    #endregion

    #region Private methods
    void ManageAskCursor()
    {
        if (!waitingForMouseClick)
            return;

        if (!Input.GetMouseButtonDown(0))
            return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        switch (waitingForInputCursor)
        {
            case AskCursor.Attack:
                if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Entity")))
                {
                    CallActionsToSelectedGroups.OrderAttackUnit(hit.transform.GetComponent<Entity>());
                    waitingForMouseClick = false;
                }
                break;

            case AskCursor.Move:
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Grid")))
                {
                    CallActionsToSelectedGroups.OrderMoveToPosition(hit.point);
                    waitingForMouseClick = false;
                }
                break;
        }
    }

    void ManageHotkeyInput()
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
    #endregion
    #endregion
}
