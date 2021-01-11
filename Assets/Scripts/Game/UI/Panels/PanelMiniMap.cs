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
    public class PanelMiniMap : AbstractPanel
    {
        #region Fields        
        [SerializeField] private GameObject _prefabSpriteWaveIndicator;
        [SerializeField] private Camera _miniMapCamera; // used to get minimap size
        [SerializeField] private RectTransform _minimapRoot; // used to get minimap UI width

        private MinimapWaveIndicator[] _wavesIndicators = null;
        #endregion

        #region Methods
        #region MonoBehaviour Callbacks
        protected override void Awake()
        {
            var minimapPositionConverter = this.GetOrAddComponent<MinimapPositionConverter>();
            minimapPositionConverter.Initialize(_miniMapCamera, _minimapRoot);
        }
        
        protected override void Start()
		{
            base.Start();

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

            var waveSpawnPoints = FindObjectsOfType<WaveIndicatorUIWorldPosition>();

            if (waveSpawnPoints.Length == 0)
                Debug.LogWarningFormat("Panel Mini Map: Found 0 WaveSpawnPoint on scene. There might be problems on the scene.");

            _wavesIndicators = new MinimapWaveIndicator[waveSpawnPoints.Length];

            // for each WaveSpawnPoint
            for (int i = 0; i < waveSpawnPoints.Length; i++)
            {
                // instanciate a wave indicator
                MinimapWaveIndicator waveIndicator = new MinimapWaveIndicator(waveSpawnPoints[i], InstanciateWaveIndicator().GetComponent<RectTransform>(), this);
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
        #endregion
        #endregion
    }
}
