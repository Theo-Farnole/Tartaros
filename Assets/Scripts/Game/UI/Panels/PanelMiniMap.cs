using Game.WaveSystem;
using Lortedo.Utilities.Managers;
using Lortedo.Utilities.Pattern;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace Game.UI
{
    public struct WaveIndicator
    {
        private WaveSpawnPoint _waveSpawnPoint;
        private RectTransform _waveIndicatorUI;
        private PanelMiniMap _panelMiniMap;

        public WaveIndicator(WaveSpawnPoint waveSpawnPoint, RectTransform waveIndicator, PanelMiniMap panelMiniMap)
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

    public class PanelMiniMap : MonoBehaviour
    {
        #region Fields
        [SerializeField] private Camera _miniMapCamera; // used to get minimap size
        [SerializeField] private RectTransform _minimapRoot; // used to get minimap UI width
        [SerializeField] private GameObject _prefabSpriteWaveIndicator;

        private WaveIndicator[] _wavesIndicators = null;
        #endregion

        #region Methods
        #region MonoBehaviour Callbacks
        void Start()
        {
            // We set 'CreateWaveIndicators' in Start, and not Awake:
            // In Awake, FindGameObject used in this method would find no WaveSpawnPoint.
            CreateWaveIndicators();
        }

        void OnEnable()
        {
            // OnEnable called before Start,
            // So CreateWaveIndicators has not be called
            if (_wavesIndicators != null)
            {
                foreach (var waveIndicator in _wavesIndicators)
                    waveIndicator.SubscribeToEvents();
            }
        }

        void OnDisable()
        {
            foreach (var waveIndicator in _wavesIndicators)
                waveIndicator.UnsubscribeToEvents();
        }
        #endregion

        #region Private methods
        private void CreateWaveIndicators()
        {
            Assert.IsNull(_wavesIndicators, "You shouldn't call CreateWaveIndicators() if _wavesIndicators is already initialized.");

            var waveSpawnPoints = FindObjectsOfType<WaveSpawnPoint>();

            if (waveSpawnPoints.Length == 0)
                Debug.LogWarningFormat("Panel Mini Map: Found 0 WaveSpawnPoint on scene. There might be problems on the scene.");

            _wavesIndicators = new WaveIndicator[waveSpawnPoints.Length];

            // for each WaveSpawnPoint
            for (int i = 0; i < waveSpawnPoints.Length; i++)
            {
                // instanciate a wave indicator
                WaveIndicator waveIndicator = new WaveIndicator(waveSpawnPoints[i], InstanciateWaveIndicator().GetComponent<RectTransform>(), this);
                waveIndicator.SubscribeToEvents();
                waveIndicator.UpdatePositionAndRotation();

                _wavesIndicators[i] = waveIndicator;
            }
        }

        private GameObject InstanciateWaveIndicator()
        {
            var instanciatedWaveIndicator = Instantiate(_prefabSpriteWaveIndicator, _minimapRoot);

#if DEVELOPMENT_BUILD || UNITY_EDITOR            
            if (instanciatedWaveIndicator.GetComponent<Image>() == null)
                Debug.LogError("Panel Mini Map : Wave Indicator miss a Image component on it.");
#endif

            return instanciatedWaveIndicator;
        }

        #region Calculation
        /// <summary>
        /// Returns a Vector3 as a percent. If in bounds, returns between 0.0f and 1.0f.
        /// </summary>
        public Vector3 WorldPositionToMinimapRelative(Vector3 position)
        {
            Bounds cameraBounds = new Bounds(_miniMapCamera.transform.position, _miniMapCamera.orthographicSize * 2 * Vector3.one);

            Vector3 min = cameraBounds.min;
            Vector3 max = cameraBounds.max;

            Vector3 relativePosition = (position - min);
            Vector3 relativeSize = (max - min);

            // operator Vector3 / Vector3 doesn't exist            
            return new Vector3(relativePosition.x / relativeSize.x, relativePosition.z / relativeSize.z);
        }

        public Vector3 RelativePositionToAbsolutePositionMinimap(Vector3 relative)
        {
            Vector3 output = new Vector3(relative.x * _minimapRoot.rect.width, relative.y * _minimapRoot.rect.height);

            return output;
        }

        public Vector3 WorldPositionToMinimap(Vector3 position)
            => RelativePositionToAbsolutePositionMinimap(WorldPositionToMinimap(position));


        public Vector3 MinimapToWorldPosition(Vector3 position)
        {
            // TODO
            // used to move camera or unit on minimap click
            throw new System.NotImplementedException();
        }
        #endregion
        #endregion
        #endregion
    }
}
