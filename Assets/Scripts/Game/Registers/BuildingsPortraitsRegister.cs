using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingsPortraitsRegister : Register<Sprite, Building>
{
    [EnumNamedArray(typeof(Building))]
    [SerializeField] private Sprite[] _buildingsPortraits;

    protected override Sprite[] Prefabs { get => _buildingsPortraits; set => _buildingsPortraits = value; }
}
