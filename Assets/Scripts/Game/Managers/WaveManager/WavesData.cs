using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace Game.WaveSystem
{
    [CreateAssetMenu(menuName = "Tartaros/System/Waves Content")]
    public class WavesData : ScriptableObject
    {
        [SerializeField] private Wave[] _waves = new Wave[0];
        [SerializeField] private string _entityIDToAttack = "building_temple";

        public string EntityIDToAttack { get => _entityIDToAttack; }

        /// <summary>
        /// Don't pass the wave index (starts from '0'), but the actual wave count (starts from '1)
        /// </summary>
        /// <param name="waveCount"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public IEnumerator WaveSequence(int waveCount, Vector3 position)
        {
            Assert.IsTrue(waveCount >= 1, "Wave count should start at 1. Don't pass the wave index (starts from '0'), but the actual wave count (starts from '1)");

            int waveIndex = waveCount - 1;

            Assert.IsTrue(waveIndex >= 0, "Wave index must be greater or equals to zero.");
            Assert.IsTrue(waveIndex < _waves.Length, string.Format("There is no wave setted at wave {0} for object {1}.", waveCount, name));

            yield return _waves[waveIndex].WaveSequence(position, GetWaveAttackTarget());
        }


        public Transform GetWaveAttackTarget()
        {
            var foundedEntity = FindObjectsOfType<Entity>()
                    .Where(x => x.EntityID == _entityIDToAttack)
                    .FirstOrDefault();

            return foundedEntity != null ? foundedEntity.transform : null;
        }

        public bool IsWaveEmpty(int index)
        {
            return _waves.IsIndexInsideBounds(index) 
                ? _waves[index].UnitSequencesEmpty 
                : true;
        }
    }
}
