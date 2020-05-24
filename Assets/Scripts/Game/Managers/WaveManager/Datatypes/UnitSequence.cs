using Game.IA.Action;
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


        public IEnumerator StartSequence(Vector3 spawnPosition)
        {
            bool hasFoundEntityData = MainRegister.Instance.TryGetEntityData(_entityID, out EntityData entityData);

            if (!hasFoundEntityData)
            {
                Debug.LogErrorFormat(debugLogHeader + "Cannot find prefab of {0}. Skipping unit spawn sequence.", _entityID);
                yield return null;
            }

            // getting needed variable
            int unitCount = _unitCount; // heap to stack
            GameObject prefab = entityData.Prefab;
            Transform target = Object.FindObjectsOfType<Entity>().Where(x => x.EntityID == "building_temple").FirstOrDefault().transform; // get temple            

            // sequence
            yield return new WaitForSeconds(_waitTimeBeforeSequence);

            if (_spawnUnits)
            {
                for (int i = 0; i < unitCount; i++)
                {
                    SpawnUnit(prefab, spawnPosition, target);
                    yield return new WaitForSeconds(_timeAfterUnitSpawn);
                }
            }

            yield return new WaitForSeconds(_waitTimeAfterSequence);
        }

        void SpawnUnit(GameObject prefab, Vector3 position, Transform templeTarget)
        {
            Assert.IsNotNull(prefab, debugLogHeader + " prefab of " + _entityID + " should be not null. Aborting unit sequence.");

            var instanciatedPrefab = Object.Instantiate(prefab, position, Quaternion.identity);
            var instanciatedPrefabEntity = instanciatedPrefab.GetComponent<Entity>();

            Assert.IsNotNull(instanciatedPrefabEntity,
                string.Format("Unit {0} misses a Entity component", prefab.name));

            instanciatedPrefabEntity.Team = Team.Enemy;

            if (templeTarget != null)
            {
                // make unit go to temple
                Action action = new ActionMoveToPositionAggressively(instanciatedPrefabEntity, templeTarget.transform.position);
                instanciatedPrefabEntity.SetAction(action);
            }
            else
            {
                Debug.LogErrorFormat(debugLogHeader + "No temple found. Can't make Unit move aggressively to it");
            }
        }
    }
}
