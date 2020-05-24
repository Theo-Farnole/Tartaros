using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Get EntityData from EntityType
/// </summary>
public class MainRegister : SingletonSerializedMonoBehaviour<MainRegister>
{
    #region Fields
    public const string debugLogHeader = "Main Register : ";

    [TabGroup("Entities")]
    [SerializeField] private Dictionary<string, EntityData> _entitiesRegisters = new Dictionary<string, EntityData>();

    [TabGroup("Overall Actions")]
    [SerializeField] private Dictionary<OverallAction, OverallActionData> _overallActionsRegister = new Dictionary<OverallAction, OverallActionData>();
    #endregion

    #region Methods
    #region Public methods
    public bool TryGetEntityData(string entityID, out EntityData entityData)
    {
        if (_entitiesRegisters.ContainsKey(entityID))
        {
            entityData = _entitiesRegisters[entityID];
            return true;
        }
        else
        {
            throw new MissingFieldException(string.Format(debugLogHeader + "ID: {0} doesn't exist in register.", entityID));
        }
    }

    public bool TryGetOverallActionData(OverallAction overallAction, out OverallActionData overallActionData)
    {
        if (_overallActionsRegister != null && _overallActionsRegister.ContainsKey(overallAction))
        {
            overallActionData = _overallActionsRegister[overallAction];
            return true;
        }
        else
        {
            throw new MissingFieldException(string.Format(debugLogHeader + "OverallAction: {0} doesn't exist in register.", overallAction));
        }
    }
    #endregion
    #endregion
}

