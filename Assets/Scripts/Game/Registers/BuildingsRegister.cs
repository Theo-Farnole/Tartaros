using Lortedo.Utilities.Inspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Registers
{
    public class BuildingsRegister : Register<RegisterData, BuildingType>
    {
        [EnumNamedArray(typeof(BuildingType))]
        [SerializeField] private RegisterData[] _buildingsData;

        protected override RegisterData[] Prefabs { get => _buildingsData; set => _buildingsData = value; }
        protected override int DeltaIndex { get => EntitiesSystem.STARTING_INDEX_BUILDING; }
    }
}
