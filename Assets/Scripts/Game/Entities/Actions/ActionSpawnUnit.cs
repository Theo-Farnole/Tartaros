using Game.Cheats;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Game.Entities.Actions
{
    public class ActionSpawnUnit : Action
    {
        private readonly float _creationDuration;
        private readonly string _entityIDToSpawn;

        private float _startTime;
        private float _spawnTime;

        private bool _successfulSpawnUnit = false; // allow returning true of CanExecuteAction()
        private bool _isInToPendingCreation = false;
        private bool _doPendingCreationFailed = false;

        public ActionSpawnUnit(Entity owner, float creationDuration, string entityIDToSpawn) : base(owner)
        {
            this._entityIDToSpawn = entityIDToSpawn;

            // overwrite 'creationDuration' is cheat is enabled
            if (TartarosCheatsManager.IsCreationTimeToZeroActive())
                creationDuration = 1f;

            _creationDuration = creationDuration;

            if (CanExecuteAction())
            {
                _doPendingCreationFailed = false;
                SetInPendingCreation();
            }
            else
            {
                _doPendingCreationFailed = true;
                _isInToPendingCreation = false;
            }
        }

        ~ActionSpawnUnit()
        {
            Refund();
        }

        #region Methods
        #region Public Override Methods
        public override void OnStateEnter()
        {
            base.OnStateEnter();

            _startTime = Time.time;
            _spawnTime = Time.time + _creationDuration;
        }

        public override void Tick()
        {
            if (Time.time >= _spawnTime)
            {
                SpawnUnit();
            }
        }

        public override bool CanExecuteAction()
        {
            return !_doPendingCreationFailed
                && (
                    _isInToPendingCreation
                    || _successfulSpawnUnit
                    || _owner.GetCharacterComponent<EntityUnitSpawner>().CanSpawnEntity(_entityIDToSpawn, true)
                );
        }

        public override string ToString()
        {
            return string.Format("{0} spawns {1} in {2}.", _owner.name, _entityIDToSpawn, RemainingTimeToString());
        }
        #endregion

        #region Private Methods
        private void Refund()
        {
            if (!_isInToPendingCreation)
                Debug.LogWarningFormat("{0} : Can't refund because not in pending creation list.", GetType());

            GameManager.Instance.RemovePendingCreationEntity(_entityIDToSpawn);
            _isInToPendingCreation = false;
        }

        private void SetInPendingCreation()
        {
            if (_isInToPendingCreation)
                return;

            GameManager.Instance.AddPendingCreationEntity(_entityIDToSpawn);
            _isInToPendingCreation = true;
        }

        private void SpawnUnit()
        {
            Assert.IsFalse(_doPendingCreationFailed);

            // We need to called 'Refund'
            // Otherwise, the resources for unit creation are taken twice.
            Refund();
            _owner.GetCharacterComponent<EntityUnitSpawner>().SpawnUnit(_entityIDToSpawn);

            _successfulSpawnUnit = true;

            LeaveAction();
        }
        #endregion

        #region Getter Methods
        public float GetCompletion()
        {
            return (Time.time - _startTime) / (_spawnTime - _startTime);
        }

        public float GetRemainingTime()
        {
            return _spawnTime - Time.time;
        }

        public string RemainingTimeToString()
        {
            float remainingTime = GetRemainingTime();

            if (remainingTime >= 0)
            {
                return string.Format("{0:N0}", GetRemainingTime());
            }
            else
            {
                return _creationDuration.ToString();
            }
        }
        #endregion
        #endregion
    }
}
