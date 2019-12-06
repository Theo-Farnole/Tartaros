using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UnitComponent : MonoBehaviour
{
    private Unit _unitManager;

    public Unit UnitManager
    {
        get => _unitManager;
        set
        {
            if (_unitManager != null)
            {
                Debug.LogWarning("Unit manager is already set. You can't change it value!");
                return;
            }
            else
            {
                _unitManager = value;
            }
        }
    }
}
