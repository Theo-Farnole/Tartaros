using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Get EntityData from EntityType
/// </summary>
public class MainRegister : SingletonSerializedMonoBehaviour<MainRegister>
{
    #region Fields
    [TabGroup("Buildings")]
    [SerializeField] private Dictionary<BuildingType, EntityData> _buildingsRegister = new Dictionary<BuildingType, EntityData>();

    [TabGroup("Units")]
    [SerializeField] private Dictionary<UnitType, EntityData> _unitsRegister = new Dictionary<UnitType, EntityData>();

    [TabGroup("Overall Actions")]
    [SerializeField] private Dictionary<OverallAction, OverallActionData> _overallActionsRegister = new Dictionary<OverallAction, OverallActionData>();
    #endregion

    #region Methods
    #region MonoBehaviour Callbacks
    void Start()
    {
        CheckForErrorsInRegisters();
    }

    void OnValidate()
    {
        CheckForErrorsInRegisters();
    }

    private void CheckForErrorsInRegisters()
    {
        CheckForErrorsInRegister(_buildingsRegister);
        CheckForErrorsInRegister(_unitsRegister);
        CheckForErrorsInRegister(_overallActionsRegister);
    }
    #endregion

    #region Public methods
    public bool TryGetUnitData(UnitType unitType, out EntityData unitData)
    {
        if (_unitsRegister != null && _unitsRegister.ContainsKey(unitType))
        {
            unitData = _unitsRegister[unitType];
            return true;
        }

        unitData = null;
        return false;
    }

    public bool TryGetBuildingData(BuildingType buildingType, out EntityData buildingData)
    {
        if (_buildingsRegister != null && _buildingsRegister.ContainsKey(buildingType))
        {
            buildingData = _buildingsRegister[buildingType];
            return true;
        }

        buildingData = null;
        return false;
    }

    public bool TryGetEntityData(EntityType entityType, out EntityData entityData)
    {
        if (entityType.TryGetUnitType(out UnitType unitType))
        {
            return TryGetUnitData(unitType, out entityData);
        }
        else if (entityType.TryGetBuildingType(out BuildingType buildingType))
        {
            return TryGetBuildingData(buildingType, out entityData);
        }
        else
        {
            Debug.LogErrorFormat("Register: Can't get EntityData of {0} because it's not a UnitType or BuildingType.", entityType);

            entityData = null;
            return false;
        }
    }

    public bool TryGetOverallActionData(OverallAction overallAction, out OverallActionData overallActionData)
    {
        if (_overallActionsRegister != null && _overallActionsRegister.ContainsKey(overallAction))
        {
            overallActionData = _overallActionsRegister[overallAction];
            return true;
        }

        overallActionData = null;
        return false;

    }
    #endregion

    #region Private methods
    void CheckForErrorsInRegister<TKey, TValue>(Dictionary<TKey, TValue> dictionary) where TKey : struct, Enum
    {
        var enumValues = Enum.GetValues(typeof(TKey));

        // browse the 'TKey' enum
        foreach (TKey enumValue in enumValues)
        {
            // ignore 'None' value
            // because we don't want a prefab as 'None'
            if (enumValue.ToString() == "None")
                continue;

            if (!dictionary.ContainsKey(enumValue))
            {
                Debug.LogErrorFormat("In dictionary {0}, the key {1} is missing.", typeof(TKey).ToString(), enumValue);
            }
            else if (dictionary[enumValue] == null)
            {
                Debug.LogErrorFormat("In dictionary {0}, the value of {1} is null.", typeof(TKey).ToString(), enumValue);
            }
        }
    }
    #endregion
    #endregion
}

