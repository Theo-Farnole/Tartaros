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
        [SerializeField] private TextMeshProUGUI _timeUntilWaveLabel;

        private int _cachedFinalWave;

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
            WaveManager.OnWaveTimerUpdate += WaveManager_OnWaveTimerUpdate;
        }

        void OnDisable()
        {
            WaveManager.OnWaveStart -= UpdateWaveLabel;
            WaveManager.OnWaveTimerUpdate -= WaveManager_OnWaveTimerUpdate;
        }
        #endregion

        #region Events Handlers
        private void WaveManager_OnWaveTimerUpdate(int waveCount, float remainingTime)
        {
            int minutes = Mathf.FloorToInt(remainingTime / 60);

            UpdateTimeLeft(minutes);
        }
        #endregion

        #region Private Methods
        private void UpdateWaveLabel(int currentWave) => UpdateWaveLabel(currentWave, _cachedFinalWave);

        private void UpdateWaveLabel(int currentWave, int finalWave)
        {
            _waveLabel.text = string.Format("{0}/{1}", currentWave, finalWave);
        }

        private void UpdateTimeLeft(int minutes)
        {
            _timeUntilWaveLabel.text = string.Format("{0}h", minutes.ToString());
        }

        private int GetFinalWave()
        {
            return FindObjectOfType<WaveManager>().FinalWave;
        }
        #endregion
        #endregion
    }
}
