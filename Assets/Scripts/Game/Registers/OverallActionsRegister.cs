using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Registers
{
    public class OverallActionsRegister : Register<RegisterData, OverallAction>
    {
        [EnumNamedArray(typeof(OverallAction))]
        [SerializeField] private RegisterData[] _overallActionsdata;

        protected override RegisterData[] Prefabs { get => _overallActionsdata; set => _overallActionsdata = value; }
    }
}
