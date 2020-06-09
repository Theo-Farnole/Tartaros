using Game.WaveSystem;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Game.UI
{
    public class PanelWavesInformation : AbstractPanel
    {
        [SerializeField] private TextMeshProUGUI _waveLabel;

        private int _cachedFinalWave;

        // TODO: Current Wave
        // TODO: Max wave
        // TODO: Time next wave

        #region Methods
        #region MonoBehaviour Callbcks
        void Start()
        {
            _cachedFinalWave = GetFinalWave();
            UpdateWaveLabel(1, _cachedFinalWave);    
        }

        void OnEnable()
        {
            WaveManager.OnWaveStart += UpdateWaveLabel;   
        }
        #endregion

        #region Private Methods
        private void UpdateWaveLabel(int currentWave) => UpdateWaveLabel(currentWave, _cachedFinalWave);

        private void UpdateWaveLabel(int currentWave, int finalWave)
        {
            _waveLabel.text = string.Format("{0}/{1}", currentWave, finalWave);
        }

        private int GetFinalWave()
        {
            return FindObjectOfType<WaveManager>().FinalWave;
        }
        #endregion
        #endregion
    }
}
