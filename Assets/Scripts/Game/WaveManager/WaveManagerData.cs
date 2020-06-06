using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.WaveSystem
{
    [CreateAssetMenu(menuName = "Tartaros/System/Wave Manager")]
    public class WaveManagerData : ScriptableObject
    {
        [SerializeField, Range(1, 90), Tooltip("In Minutes")] private float _minutesBetweenWaves = 30;

        public float SecondsBetweenWave { get => _minutesBetweenWaves * 60; }
    }
}
