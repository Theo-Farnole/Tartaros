using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Owner
{
    Sparta,
    Persian,
    Nature
}

public class OwnedEntity : Entity
{
    [Space]
    [SerializeField] private Owner _owner;

    public Owner Owner { get => _owner;}
}
