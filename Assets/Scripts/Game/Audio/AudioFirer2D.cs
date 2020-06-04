using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Audio
{
    /// <summary>
    /// This script listen to game events to firer audio clip to AudioManager2D.    
    /// </summary>
    public class AudioFirer2D : MonoBehaviour
    {
        private AudioManager2D _audioManager;

        void Awake()
        {
            // we use 'FindObjectOfType' to avoid using a Singleton
            _audioManager = FindObjectOfType<AudioManager2D>();
        }

        void OnEnable()
        {
            WaveManager.OnWaveStart += WaveManager_OnWaveStart;
            WaveManager.OnWaveClear += WaveManager_OnWaveClear;
        }

        private void WaveManager_OnWaveClear(int waveCountCleared)
        {
            _audioManager.PlayRandomClip(Sound2D.WaveEnd);
        }

        private void WaveManager_OnWaveStart(int waveCount)
        {
            _audioManager.PlayRandomClip(Sound2D.WaveStart);
        }
    }
}
