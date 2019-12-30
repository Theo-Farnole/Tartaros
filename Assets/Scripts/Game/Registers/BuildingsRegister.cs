using Lortedo.Utilities.Inspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Registers
{
    public class BuildingsRegister : Register<EntityData, BuildingType>
    {
        [EnumNamedArray(typeof(BuildingType))]
        [SerializeField] private EntityData[] _buildingsData;

        protected override EntityData[] Prefabs { get => _buildingsData; set => _buildingsData = value; }
        protected override int DeltaIndex { get => EntitiesSystem.STARTING_INDEX_BUILDING; }
    }
}
