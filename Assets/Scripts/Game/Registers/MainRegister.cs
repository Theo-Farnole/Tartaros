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
    public EntityData GetEntityData(string entityID)
    {
        Assert.IsNotNull(_entitiesRegisters);
        Assert.IsTrue(_entitiesRegisters.ContainsKey(entityID),
            string.Format(debugLogHeader + "ID: {0} doesn't exist in register.", entityID));

        return _entitiesRegisters[entityID];
    }

    public OverallActionData GetOverallActionData(OverallAction overallAction)
    {
        Assert.IsNotNull(_overallActionsRegister);
        Assert.IsTrue(_overallActionsRegister.ContainsKey(overallAction),
            string.Format(debugLogHeader + "OverallAction: {0} doesn't exist in register.", overallAction));

        return _overallActionsRegister[overallAction];
    }
    #endregion
    #endregion
}

