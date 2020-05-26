using Game.IA.Action;
using Lortedo.Utilities.Pattern;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace Game.WaveSystem
{
    [System.Serializable]
    public class UnitSequence
    {
        #region Fields
        private const string headerInspector_SpawnUnit = "Spawn Units";
        private const string headerInspector_AfterSequenceTime = "Before Sequence Wait Time";
        private const string headerInspector_BeforeSequenceTime = "After Sequence Wait Time";
        private const string debugLogHeader = "UnitSequence : ";


        [Header("Wait Fields")]
        [SerializeField] private float _waitTimeBeforeSequence = 0;
        [SerializeField] private float _waitTimeAfterSequence = 1;

        #region Spawn Unit group
        [ToggleGroup(nameof(_spawnUnits), headerInspector_SpawnUnit)]
        [SerializeField] private bool _spawnUnits = true;

        [ToggleGroup(nameof(_spawnUnits), headerInspector_SpawnUnit)]
        [SerializeField] private string _entityID;

        [ToggleGroup(nameof(_spawnUnits), headerInspector_SpawnUnit)]
        [SerializeField] private int _unitCount = 1;

        [ToggleGroup(nameof(_spawnUnits), headerInspector_SpawnUnit)]
        [SerializeField] private float _timeAfterUnitSpawn = 0.05f;
        #endregion
        #endregion


        public IEnumerator StartSequence(Vector3 spawnPosition, Transform attackTarget)
        {
            EntityData entityData = MainRegister.Instance.GetEntityData(_entityID);

            if (entityData == null)
            {
                Debug.LogErrorFormat(debugLogHeader + "Cannot find prefab of {0}. Skipping unit spawn sequence.", _entityID);
                yield return null;
            }

            // getting needed variable
            int unitCount = _unitCount; // heap to stack
            GameObject prefab = entityData.Prefab;

            // sequence
            yield return new WaitForSeconds(_waitTimeBeforeSequence);

            if (_spawnUnits)
            {
                for (int i = 0; i < unitCount; i++)
                {
                    SpawnUnit(prefab, spawnPosition, attackTarget);
                    yield return new WaitForSeconds(_timeAfterUnitSpawn);
                }
            }

            yield return new WaitForSeconds(_waitTimeAfterSequence);
        }

        void SpawnUnit(GameObject prefab, Vector3 position, Transform attackTarget)
        {
            Assert.IsNotNull(prefab, debugLogHeader + " prefab of " + _entityID + " should be not null. Aborting unit sequence.");

            var instanciatedPrefab = ObjectPooler.Instance.SpawnFromPool(prefab, position, Quaternion.identity, true);
            var instanciatedPrefabEntity = instanciatedPrefab.GetComponent<Entity>();

            Assert.IsNotNull(instanciatedPrefabEntity,
                string.Format("Unit {0} misses the Entity component", prefab.name));

            instanciatedPrefabEntity.Team = Team.Enemy;

            if (attackTarget != null)
            {
                // make unit go to temple
                Action action = new ActionMoveToPositionAggressively(instanciatedPrefabEntity, attackTarget.position);
                instanciatedPrefabEntity.SetAction(action);
            }
            else
            {
                Debug.LogErrorFormat(debugLogHeader + "No temple found. Can't make Unit move aggressively to it");
            }
        }
    }
}
