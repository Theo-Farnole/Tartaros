using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Obsolete("Previously used to change entities' collision at runtime. This feature has been killed.")]
[CreateAssetMenu(menuName = "Tartaros/System/Collision Scaler")]
public class CollisionScalerData : ScriptableObject
{
    [SerializeField, Range(0, 100), Tooltip("In Percent")] private int _collisionScaleDownPercent = 50;
    public float CollisionScaleDownPercent { get => (float)_collisionScaleDownPercent / 100f; }

    [SerializeField, Tooltip("In Seconds")] private float _reduceTime = 1f;
    public float ReduceTime { get => _reduceTime; }

    [SerializeField] private bool _useDifferentIncreaseTime = true;

    [SerializeField, DrawIf("_useDifferentIncreaseTime", true, ComparisonType.Equals, DisablingType.ReadOnly)] private float _increaseTime = 1f;
    public float IncreaseTime
    {
        get
        {
            if (_useDifferentIncreaseTime)
                return _increaseTime;
            else
                return _reduceTime;
        }
    }
}
