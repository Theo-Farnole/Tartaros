using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tartaros/System/Attack Slots")]
public class AttackSlotsData : ScriptableObject
{
    [SerializeField] private float _distanceBetweenSlot = 1.8f;
    public float DistanceBetweenSlot { get => _distanceBetweenSlot; }
}
