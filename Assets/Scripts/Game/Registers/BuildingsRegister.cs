using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum Building
{
    House,
    Barracks
}

public class BuildingsRegister : Register<GameObject, Building>
{
    [EnumNamedArray(typeof(Building))]
    [SerializeField] private GameObject[] _prefabsBuildings;

    protected override GameObject[] Prefabs { get => _prefabsBuildings; set => _prefabsBuildings = value; }
}
