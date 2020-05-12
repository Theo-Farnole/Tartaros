using Lortedo.Utilities.Pattern;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lortedo.Utilities.Pattern
{
    /// <summary>
    /// Enqueue game object after Particle System ended
    /// </summary>
    [RequireComponent(typeof(ParticleSystem))]
    public class PooledParticleSystem : MonoBehaviour, IPooledObject
    {
        private ParticleSystem _particleSystem;
        private string _objectTag;

        public string ObjectTag { get; set; }

        void Awake()
        {
            _particleSystem = GetComponent<ParticleSystem>();
        }

        void Update()
        {
            if (_particleSystem.isStopped)
            {
                ObjectPooler.Instance.EnqueueGameObject(ObjectTag, gameObject);
            }
        }

        void IPooledObject.OnObjectSpawn()
        {
            _particleSystem.Play();
        }
    }
}
