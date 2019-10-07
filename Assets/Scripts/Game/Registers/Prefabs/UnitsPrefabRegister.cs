using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitsPrefabRegister : Register<GameObject, Unit>
{
    [EnumNamedArray(typeof(Unit))]
    [SerializeField] private GameObject[] _prefabsUnit;

    protected override GameObject[] Prefabs { get => _prefabsUnit; set => _prefabsUnit = value; }
    protected override int DeltaIndex { get => EntitiesSystem.STARTING_INDEX_UNIT; }
}
