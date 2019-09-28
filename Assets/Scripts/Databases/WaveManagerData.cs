using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Leonidas Legacy/System/Wave")]
public class WaveManagerData : ScriptableObject
{
    [SerializeField, Range(1, 90), Tooltip("In Minutes")] private float _minutesBetweenWaves = 30;
    public float TimerBetweenWavesInSeconds { get => _minutesBetweenWaves * 60; }
}
