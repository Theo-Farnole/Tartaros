using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.WaveSystem
{
    [System.Serializable]
    public class Wave
    {
        [SerializeField] private UnitSequence[] _unitSequences = new UnitSequence[0];

        public UnitSequence[] UnitSequences { get => _unitSequences; }
        public bool UnitSequencesEmpty { get => _unitSequences.Length == 0; }

        public IEnumerator WaveSequence(Vector3 spawnPosition, Transform attackTarget)
        {
            foreach (UnitSequence unitSequence in _unitSequences)
            {
                yield return unitSequence.StartSequence(spawnPosition, attackTarget);
            }
        }
    }
}