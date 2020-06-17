namespace Game.Entities
{
    using UnityEngine;

    [CreateAssetMenu(menuName = "Tartaros/System/Collision Scaler")]
    public class EntityDynamicSizeData : ScriptableObject
    {
        [SerializeField, Range(0, 1), Tooltip("In Percent")] private float _collisionScaleDownPercent = 0.25f;
        [Space]
        [SerializeField, Tooltip("In Seconds")] private float _reduceTime = 1f;

        [SerializeField] private bool _useDifferentIncreaseTime = true;
        [SerializeField, DrawIf("_useDifferentIncreaseTime", true, ComparisonType.Equals, DisablingType.ReadOnly)] private float _increaseTime = 1f;


        public float ReduceTime { get => _reduceTime; }
        public float CollisionScaleDownPercent { get => _collisionScaleDownPercent; }
        public float IncreaseTime { get => _useDifferentIncreaseTime ? _increaseTime : _reduceTime; }
    }
}
