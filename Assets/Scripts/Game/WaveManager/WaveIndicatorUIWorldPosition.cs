namespace Game.WaveSystem
{
    using TF.Assertations;
    using UnityEngine;

    public partial class WaveIndicatorUIWorldPosition : MonoBehaviour
    {
        [SerializeField] private WaveSpawnPoint waveSpawnPoint;

        public bool IsWaveEmpty(int waveCount) => waveSpawnPoint.WavesData.IsWaveEmpty(waveCount);
        public Transform GetWavesAttackTarget() => waveSpawnPoint.WavesData.GetWaveAttackTarget();
        public string GetEntityIDToAttack() => waveSpawnPoint.WavesData.EntityIDToAttack;
    }

#if UNITY_EDITOR
    public partial class WaveIndicatorUIWorldPosition : MonoBehaviour
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
