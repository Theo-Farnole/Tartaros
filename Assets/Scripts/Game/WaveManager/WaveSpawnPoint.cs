namespace Game.WaveSystem
{
    using Sirenix.OdinInspector;
    using UnityEngine;
    using UnityEngine.Assertions;

    /// <summary>
    /// This script start wave sequence when order received from WaveManager.
    /// It spawn every mob from its position.
    /// </summary>
    public partial class WaveSpawnPoint : MonoBehaviour
    {
        [Required]
        [SerializeField] private WavesData _wavesData;        
        
        private Coroutine _currentCoroutine;

        public WavesData WavesData { get => _wavesData; set => _wavesData = value; }

        void OnEnable()
        {
            WaveManager.OnWaveStart += WaveManager_OnWaveStart;
        }

        void OnDisable()
        {
            WaveManager.OnWaveStart -= WaveManager_OnWaveStart;
        }

        void WaveManager_OnWaveStart(int waveCount)
        {            
            StartWave(waveCount);
        }

        private void StartWave(int waveCount)
        {
            Assert.IsNotNull(_wavesData, string.Format("Please assign a WavesData to {0}.", name));
            _currentCoroutine = StartCoroutine(_wavesData.WaveSequence(waveCount, transform.position));
        }
    }

#if UNITY_EDITOR
    public partial class WaveSpawnPoint : MonoBehaviour
    {
        [Header("DEBUGS")]
        [SerializeField] private Color _gizmosColor = Color.red;
        [SerializeField] private float _gizmosWireSphereRadius = 1.5f;

        void OnDrawGizmos()
        {
            Gizmos.color = _gizmosColor;
            Gizmos.DrawWireSphere(transform.position, _gizmosWireSphereRadius);
        }
    }
#endif
}
