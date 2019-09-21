using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Entity
{
    Alexios,
    Kassandra
}

public class PortraitRegister : Register<Sprite, Entity>
{
    [EnumNamedArray(typeof(Entity))]
    [SerializeField] private Sprite[] _prefabsSprites;

    protected override Sprite[] Prefabs { get => _prefabsSprites; set => _prefabsSprites = value; }
}
