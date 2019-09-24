using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Register<TPrefab, TEnum> : Singleton<Register<TPrefab, TEnum>> where TEnum : struct, System.Enum where TPrefab : class
{
    #region Fields
    private TPrefab[] _prefabs;
    #endregion

    #region Properties
    protected virtual TPrefab[] Prefabs { get => _prefabs; set => _prefabs = value; }
    protected virtual int DeltaIndex { get => 0; }
    #endregion

    #region Methods   
    // force array to be the size of TEnum
    void OnValidate()
    {
        Prefabs = Utils.ForceArraySize<TPrefab, TEnum>(Prefabs);
    }

    public TPrefab GetItem(TEnum itemEnum)
    {
        int index = (int)(object)itemEnum;
        index -= DeltaIndex;

        return Prefabs[index];
    }
    #endregion
}
