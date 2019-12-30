using Lortedo.Utilities.Inspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Registers
{
    public class OverallActionsRegister : Register<OverallActionData, OverallAction>
    {
        [EnumNamedArray(typeof(OverallAction))]
        [SerializeField] private OverallActionData[] _overallActionsdata;

        protected override OverallActionData[] Prefabs { get => _overallActionsdata;}
    }
}
