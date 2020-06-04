using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public delegate void OnWaveTimerUpdate(int waveCount, float remainingTime);
public delegate void OnWaveStart(int waveCount);
public delegate void OnWaveClear(int waveCountCleared);

namespace Game.WaveSystem
{
    /// <summary>
    /// Spawn enemies waves frequently.
    /// </summary>
    public class WaveManager : MonoBehaviour
    {
        #region Fields
        private const string debugLogHeader = "WaveManager : ";

        public static event OnWaveTimerUpdate OnWaveTimerUpdate;
        public static event OnWaveStart OnWaveStart;
        public static event OnWaveClear OnWaveClear;

        [SerializeField] private WaveManagerData _data;

        private int _waveCount = 0;
        private bool _isInUnclearWave = false;

        private float _nextWaveTimer = 0;
        private int _lastFrame_TimerInSeconds;

        private List<Entity> _entitiesFromWave = new List<Entity>();
        #endregion

        #region Properties
        public int WaveCount { get => _waveCount; }
        #endregion

        #region Methods
        #region MonoBehaviour Callbacks
        void Update()
        {
            _nextWaveTimer += Time.deltaTime;

            if (!_isInUnclearWave)
            {
                ManageNotInWaveBehaviour();
            }

            _lastFrame_TimerInSeconds = (int)_nextWaveTimer;
        }

        private void ManageNotInWaveBehaviour()
        {
            if (_nextWaveTimer >= _data.SecondsBetweenWave)
            {
                StartWave();
            }

            // trigger each seconds
            if (_lastFrame_TimerInSeconds < (int)_nextWaveTimer)
            {
                OnWaveTimerUpdate?.Invoke(_waveCount, CalculateRemainingTime());
            }
        }

        void OnEnable()
        {
            UnitSequence.EntitySpawnFromWave += UnitSequence_WaveEntitySpawn;
            Entity.OnDeath += Entity_OnDeath;
        }

        void OnDisable()
        {
            UnitSequence.EntitySpawnFromWave -= UnitSequence_WaveEntitySpawn;
            Entity.OnDeath -= Entity_OnDeath;
        }
        #endregion

        #region Events Handlers
        private void UnitSequence_WaveEntitySpawn(Entity spawnedEntity)
        {
            Assert.IsFalse(_entitiesFromWave.Contains(spawnedEntity));

            _entitiesFromWave.Add(spawnedEntity);
        }

        private void Entity_OnDeath(Entity entity)
        {
            // avoid useless calculations
            if (!_isInUnclearWave)
                return;

            if (_entitiesFromWave.Contains(entity))
            {
                _entitiesFromWave.Remove(entity);

                if (DoAllEntitesFromWaveAreDead())
                {
                    EndWave();
                }
                else
                {
                    Debug.LogFormat("Wave Manager : Remaining {0} entities to kill.", _entitiesFromWave.Count);
                }
            }
        }
        #endregion

        #region Private Methods
        private void StartWave()
        {
            if (_entitiesFromWave.Count == 0) Debug.LogError("Field '_entitiesFromWave' should be clear before calling 'StartWave()'.");

            _entitiesFromWave.Clear();

            _isInUnclearWave = true;
            _waveCount++;

            Debug.LogFormat(debugLogHeader + "Wave {0} starts.", _waveCount);

            OnWaveStart?.Invoke(_waveCount);
        }

        private void EndWave()
        {
            if (_entitiesFromWave.Count != 0) Debug.LogError("Field '_entitiesFromWave' should be clear while calling 'EndWave()'. It's mean that every entities spawned by wave has been killed.");

            _entitiesFromWave.Clear();
            _isInUnclearWave = false;

            _nextWaveTimer = 0;

            Debug.LogFormat(debugLogHeader + "Wave {0} ended.", _waveCount);

            OnWaveClear?.Invoke(_waveCount);
        }

        private float CalculateRemainingTime()
        {
            return _data.SecondsBetweenWave - _nextWaveTimer;
        }

        private bool DoAllEntitesFromWaveAreDead()
        {
            return _entitiesFromWave.Count == 0;
        }
        #endregion
        #endregion
    }
}
