using Lortedo.Utilities.Inspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Registers
{
    public class UnitsRegister : Register<EntityData, UnitType>
    {
        [EnumNamedArray(typeof(UnitType))]
        [SerializeField] private EntityData[] _unitsData;

        protected override EntityData[] Prefabs { get => _unitsData; }
    }
}
