using Lortedo.Utilities.Inspector;
using Lortedo.Utilities.Managers;
using System;
using TMPro;
using UnityEngine;

namespace UI.Game
{
    [System.Serializable]
    public class PanelGameInformation : Panel
    {
        #region Fields
        private const string TIME_FORMAT = "{0}:{1:00}";
        private const string WAVE_FORMAT = "Wave # {0} in {1} minutes";

        [Space(order = 0)]
        [Header("Content", order = 1)]
        [SerializeField] private TextMeshProUGUI _waveIndicator;
        [SerializeField, EnumNamedArray(typeof(Resource))] private TextMeshProUGUI[] _resourcesLabel;
        #endregion

        #region Properties
        public TextMeshProUGUI[] ResourcesLabel { get => _resourcesLabel; }
        #endregion

        #region Methods
        public override void Initialize<T>(T uiManager)
        {
            _waveIndicator.gameObject.SetActive(false);

            GameManager.OnGameResourcesUpdate += UpdateResourcesLabel;
            WaveManager.OnWaveTimerUpdate += SetWaveText;

            UpdateResourcesLabel(GameManager.Instance.Resources);
        }

        public override void OnValidate()
        {
            if (_resourcesLabel.Length != Enum.GetValues(typeof(Resource)).Length)
            {
                Array.Resize(ref _resourcesLabel, Enum.GetValues(typeof(Resource)).Length);
            }
        }

        public void UpdateResourcesLabel(ResourcesWrapper currentResources)
        {
            _resourcesLabel[(int)Resource.Food].text = "food " + currentResources.food;
            _resourcesLabel[(int)Resource.Wood].text = "wood " + currentResources.wood;
            _resourcesLabel[(int)Resource.Gold].text = "gold " + currentResources.gold;
        }

        public void SetWaveText(int waveCount, float remainingTime)
        {
            int remainingMinutes = Mathf.FloorToInt(remainingTime / 60);
            int remainingSeconds = Mathf.FloorToInt(remainingTime % 60);

            string stringTime = string.Format(TIME_FORMAT, remainingMinutes, remainingSeconds);            
            
            _waveIndicator.text = string.Format(WAVE_FORMAT, waveCount, stringTime);
        }
        #endregion
    }
}
