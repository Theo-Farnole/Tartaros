using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Leonidas Legacy/System/Wave")]
public class WaveManagerData : ScriptableObject
{
    [SerializeField, Range(1, 90), Tooltip("In Minutes")] private float _minutesBetweenWaves = 30;
    [SerializeField] private float _timeBeforeStartNewWave = 10;

    public float TimerBetweenWavesInSeconds { get => _minutesBetweenWaves * 60; }
    public float TimeBeforeStartNewWave { get => _timeBeforeStartNewWave; }
}
