using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingDataRegister : Register<EntityData, Building>
{
    [EnumNamedArray(typeof(Building))]
    [SerializeField] private EntityData[] _buildingData;

    protected override EntityData[] Prefabs { get => _buildingData; set => _buildingData = value; }
    protected override int DeltaIndex { get => EntitiesSystem.STARTING_INDEX_BUILDING; }
}
