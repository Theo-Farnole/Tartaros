using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Entities
{
    [CreateAssetMenu(menuName = "Tartaros/Entities/Shift", fileName = "DB_Entity_Shift")]
    public class EntityShiftData : ScriptableObject
    {
        [SerializeField] private float _shiftCollisionRadius = 3;
        [SerializeField] private float _shiftLength = 3;

        public float ShiftLength { get => _shiftLength; }
        public float ShiftCollisionRadius { get => _shiftCollisionRadius; }        
    }
}
