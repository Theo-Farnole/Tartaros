using Game.WaveSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UI
{
    public struct MinimapWaveIndicator
    {
        private WaveSpawnPoint _waveSpawnPoint;
        private RectTransform _waveIndicatorUI;
        private PanelMiniMap _panelMiniMap;

        public MinimapWaveIndicator(WaveSpawnPoint waveSpawnPoint, RectTransform waveIndicator, PanelMiniMap panelMiniMap)
        {
            _waveSpawnPoint = waveSpawnPoint;
            _waveIndicatorUI = waveIndicator;
            _panelMiniMap = panelMiniMap;

            _waveIndicatorUI.anchoredPosition = new Vector2(0, 0);
            _waveIndicatorUI.anchorMax = Vector2.zero;
            _waveIndicatorUI.anchorMin = Vector2.zero;

            WaveManager waveManager = Object.FindObjectOfType<WaveManager>();
            ManageDisplay(waveManager.WaveCount);
        }

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

        private void WaveManager_OnWaveStart(int waveCount)
        {
            ManageDisplay(waveCount);
            UpdatePositionAndRotation();
        }

        private void ManageDisplay(int waveCount)
        {
            bool displayWaveIndicator = !_waveSpawnPoint.WavesData.IsWaveEmpty(waveCount);

            // PERFORMANCE:            
            // Should we just disable Image component, or SetActive(false) whole object ?
            _waveIndicatorUI.gameObject.SetActive(displayWaveIndicator);
        }

        private void UpdatePosition()
        {
            Vector3 waveSpawnPointLocation = _waveSpawnPoint.transform.position;
            Vector3 relative = _panelMiniMap.WorldPositionToMinimapRelative(waveSpawnPointLocation);

#if UNITY_DEVELOPMENT || UNITY_EDITOR
            if (relative.x < 0 || relative.y < 0 || relative.x > 1 || relative.y > 1)
                Debug.LogErrorFormat("Panel Mini Map : relative position {0} is out of minimap.", relative);
#endif

            Vector3 minimapPosition = _panelMiniMap.RelativePositionToAbsolutePositionMinimap(relative);

            _waveIndicatorUI.anchoredPosition = minimapPosition;
        }

        private void LookAtEnemiesTarget()
        {
            Transform enemiesTarget = _waveSpawnPoint.WavesData.GetWaveAttackTarget();

            if (enemiesTarget == null)
            {
                Debug.LogErrorFormat("Panel Mini Map : Missing entity {0} to look at.", _waveSpawnPoint.WavesData.EntityIDToAttack);
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
    }
}