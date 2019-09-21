using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Unit
{
    Alexios,
    Kassandra
}

public class UnitsPortraitsRegister : Register<Sprite, Unit>
{
    [EnumNamedArray(typeof(Unit))]
    [SerializeField] private Sprite[] _unitsPortraits;

    protected override Sprite[] Prefabs { get => _unitsPortraits; set => _unitsPortraits = value; }
}
