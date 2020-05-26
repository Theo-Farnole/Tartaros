using Lortedo.Utilities.Pattern;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace Game.WaveSystem
{
    [RequireComponent(typeof(WaveSpawnPoint))]
    public class WaveIndicator : MonoBehaviour
    {
        private GameObject _waveIndicator;

        private WaveSpawnPoint waveSpawnPoint;

        private WaveSpawnPoint WaveSpawnPoint
        {
            get
            {
                if (waveSpawnPoint == null)
                    waveSpawnPoint = GetComponent<WaveSpawnPoint>();

                return waveSpawnPoint;
            }
        }

        void Start()
        {
            WaveManager waveManager = FindObjectOfType<WaveManager>();
            ManageWaveIndicatorDisplay(waveManager.WaveCount);
        }

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
            ManageWaveIndicatorDisplay(waveCount);
        }

        #region Wave Indicator
        void InstanciateWaveIndicator()
        {
            _waveIndicator = ObjectPooler.Instance.SpawnFromPool(ObjectPoolingTags.keyUIWavesIndicators, transform.position, transform.rotation);

            _waveIndicator.transform.parent = transform;
            LookAtEnemiesTarget();

            Assert.IsTrue(LayerMask.LayerToName(_waveIndicator.gameObject.layer) == "MiniMap Additive", "Set your wave indicator's layer to 'MiniMap'.");
        }

        private void LookAtEnemiesTarget()
        {
            string enemiesTargetID = WaveSpawnPoint.WavesData.EntityIDToAttack;
            Entity enemiesTarget = FindObjectsOfType<Entity>()
                    .Where(x => x.EntityID == enemiesTargetID)
                    .FirstOrDefault();

            Assert.IsNotNull(enemiesTarget, string.Format("Missing entity {0} to look at.", enemiesTargetID));

            var lookPos = enemiesTarget.transform.position - _waveIndicator.transform.position;
            lookPos.y = 0;
            var rotation = Quaternion.LookRotation(lookPos);

            _waveIndicator.transform.rotation = rotation;
        }

        void ManageWaveIndicatorDisplay(int waveCount)
        {
            if (_waveIndicator == null)
                InstanciateWaveIndicator();

            
            bool waveIndicatorDisplayed = !WaveSpawnPoint.WavesData.IsWaveEmpty(waveCount);
            _waveIndicator.SetActive(waveIndicatorDisplayed);
        }
        #endregion
    }
}
