using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Leonidas Legacy/Entity Attack Data")]
public class AttackerEntityDatabase : ScriptableObject
{
    [SerializeField] private float _attackRange = 3f;
    public float AttackRange { get => _attackRange; }

    [SerializeField, Tooltip("Time between each attack")] private float _attackSpeed = 1f;
    public float AttackSpeed { get => _attackSpeed; }

    [SerializeField] private int _damage = 5;
    public int Damage { get => _damage; }
}
