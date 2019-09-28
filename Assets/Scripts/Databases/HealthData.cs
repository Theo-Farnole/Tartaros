using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Leonidas Legacy/Entity/Health Data")]
public class HealthData : ScriptableObject
{
    [SerializeField] private int _hp = 10;
    public int Hp { get => _hp; }
}
