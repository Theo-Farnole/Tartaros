using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class BuildingsPrefabRegister : Register<GameObject, Building>
{
    [EnumNamedArray(typeof(Building))]
    [SerializeField] private GameObject[] _prefabsBuildings;

    protected override GameObject[] Prefabs { get => _prefabsBuildings; set => _prefabsBuildings = value; }
    protected override int DeltaIndex { get => EntitiesSystem.STARTING_INDEX_BUILDING; }
}
