using Game.WaveSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UI
{
    public struct MinimapWaveIndicator
    {
        #region Fields
        private readonly WaveIndicatorUIWorldPosition _waveSpawnPoint;
        private readonly RectTransform _waveIndicatorUI;
        private readonly PanelMiniMap _panelMiniMap;
                
        private readonly MinimapPositionConverter _minimapPositionConverter;
        #endregion

        public MinimapWaveIndicator(WaveIndicatorUIWorldPosition waveSpawnPoint, RectTransform waveIndicator, PanelMiniMap panelMiniMap)
        {
            _waveSpawnPoint = waveSpawnPoint;
            _waveIndicatorUI = waveIndicator;
            _panelMiniMap = panelMiniMap;

            _waveIndicatorUI.anchoredPosition = new Vector2(0, 0);
            _waveIndicatorUI.anchorMax = Vector2.zero;
            _waveIndicatorUI.anchorMin = Vector2.zero;

            _minimapPositionConverter = Object.FindObjectOfType<MinimapPositionConverter>();

            if (_minimapPositionConverter == null)
                Debug.LogErrorFormat("Minimap Wave Indiciator : Missing MinimapPositionConvert.");

            WaveManager waveManager = Object.FindObjectOfType<WaveManager>();
            ManageDisplay(waveManager.WaveCount);
        }

        #region Methods
        #region Public Methods
        public void UpdatePositionAndRotation()
        {
            UpdatePosition();
            LookAtEnemiesTarget();
        }

        public void SubscribeToEvents()
        {
            WaveManager.OnWaveStart += WaveManager_OnWaveStart;
        }

        public void UnsubscribeToEvents()
        {
            WaveManager.OnWaveStart -= WaveManager_OnWaveStart;
        }
        #endregion

        #region Events Handlers
        private void WaveManager_OnWaveStart(int waveCount)
        {
            ManageDisplay(waveCount);
            UpdatePositionAndRotation();
        }
        #endregion

        #region Private methods
        private void ManageDisplay(int waveCount)
        {
            bool displayWaveIndicator = !_waveSpawnPoint.IsWaveEmpty(waveCount);

            // PERFORMANCE:            
            // Should we just disable Image component, or SetActive(false) whole object ?
            _waveIndicatorUI.gameObject.SetActive(displayWaveIndicator);
        }

        private void UpdatePosition()
        {
            Vector3 waveSpawnPointLocation = _waveSpawnPoint.transform.position;
            Vector3 relative = _minimapPositionConverter.WorldPositionToMinimapRelative(waveSpawnPointLocation);

#if UNITY_DEVELOPMENT || UNITY_EDITOR
            if (relative.x < 0 || relative.y < 0 || relative.x > 1 || relative.y > 1)
                Debug.LogErrorFormat("Panel Mini Map : relative position {0} is out of minimap.", relative);
#endif

            Vector3 minimapPosition = _minimapPositionConverter.RelativePositionToAbsolutePositionMinimap(relative);

            _waveIndicatorUI.anchoredPosition = minimapPosition;
        }

        private void LookAtEnemiesTarget()
        {
            Transform enemiesTarget = _waveSpawnPoint.GetWavesAttackTarget();

            if (enemiesTarget == null)
            {
                Debug.LogErrorFormat("Panel Mini Map : Missing entity {0} to look at.", _waveSpawnPoint.GetEntityIDToAttack());
                return;
            }

            Vector3 lookPosition = enemiesTarget.transform.position - _waveSpawnPoint.transform.position;
            lookPosition.y = 0; // only rotation on Y axis

            Quaternion rotation = Quaternion.LookRotation(lookPosition);

            Vector3 eulerAngles = rotation.eulerAngles;

            // swap Z and Y angle
            eulerAngles.z = eulerAngles.y;
            eulerAngles.y = 0;

            _waveIndicatorUI.eulerAngles = eulerAngles;
        }
        #endregion
        #endregion
    }
}