using CommandPattern;
using Registers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotkeyManager : Singleton<HotkeyManager>
{
    public enum AskCursor
    {
        None = 0,
        Move = 1,
        Attack = 2
    }

    [System.NonSerialized] public bool askCursor = false;
    [System.NonSerialized] public AskCursor askCursorType = AskCursor.None;

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

    void ManageAskCursor()
    {
        if (!askCursor)
            return;

        if (!Input.GetMouseButtonDown(0))
            return;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        switch (askCursorType)
        {
            case AskCursor.Attack:
                if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Entity")))
                {
                    OrdersGiverManager.Instance.OrderAttack(hit.transform);
                    askCursor = false;
                }
                break;

            case AskCursor.Move:
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Grid")))
                {
                    OrdersGiverManager.Instance.OrderMovement(hit.point);
                    askCursor = false;
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

    public void SetCommandsHandler(EntityType entityType)
    {        
        ClearCommandsHandler();

        RegisterData data = EntitiesRegister.GetRegisterData(entityType);

        // move
        if (data.EntityData.CanMove)
        {
            _commands.Add(OverallActionsRegister.Instance.GetItem(OverallAction.Move).Hotkey, new AskForMoveDestinationCommand());
        }

        // attack
        if (data.EntityData.CanAttack)
        {
            _commands.Add(OverallActionsRegister.Instance.GetItem(OverallAction.Attack).Hotkey, new AskForAttackTargetCommand());
        }

        // stop
        if (data.EntityData.CanAttack)
        {
            _commands.Add(OverallActionsRegister.Instance.GetItem(OverallAction.Stop).Hotkey, new StopCommand());
        }

        // CreateUnits
        for (int i = 0; i < data.EntityData.AvailableUnitsForCreation.Length; i++)
        {
            Unit unit = data.EntityData.AvailableUnitsForCreation[i];
            _commands.Add(UnitsRegister.Instance.GetItem(unit).Hotkey, new CreateUnitCommand(unit));
        }
    }

    public void ClearCommandsHandler()
    {
        _commands.Clear();
    }
    #endregion
}
