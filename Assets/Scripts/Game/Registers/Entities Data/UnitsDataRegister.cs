using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitsDataRegister : Register<EntityData, Unit>
{
    [EnumNamedArray(typeof(Unit))]
    [SerializeField] private EntityData[] _unitsData;

    protected override EntityData[] Prefabs { get => _unitsData; set => _unitsData = value; }
}
