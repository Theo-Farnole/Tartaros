using Lortedo.Utilities.Inspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Registers
{
    public class UnitsRegister : Register<RegisterData, Unit>
    {
        [EnumNamedArray(typeof(Unit))]
        [SerializeField] private RegisterData[] _unitsData;

        protected override RegisterData[] Prefabs { get => _unitsData; set => _unitsData = value; }
    }
}
