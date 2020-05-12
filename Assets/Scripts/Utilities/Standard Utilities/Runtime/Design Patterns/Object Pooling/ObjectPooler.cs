using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Lortedo.Utilities.Pattern
{
    /// <summary>
    /// Manage Object Pooling.
    /// </summary>
    public class ObjectPooler : Singleton<ObjectPooler>
    {
        #region Fields
        [System.Serializable]
        public class Pool
        {
            public string tag;
            public GameObject prefab;
        }

        [SerializeField] private Pool[] _poolsUserInput;

        private Dictionary<string, Queue<GameObject>> _pools = new Dictionary<string, Queue<GameObject>>();
        #endregion

        #region Methods
        #region Mono Callbacks
        void Awake()
        {
            // create Dictionnary from Pools Array (because Unity doesn't Dictionnary)
            for (int i = 0; i < _poolsUserInput.Length; i++)
            {
                Debugging.DynamicsObjects.Instance?.AddParent(_poolsUserInput[i].tag + "_pool");
                _pools.Add(_poolsUserInput[i].tag, new Queue<GameObject>());
            }
        }
        #endregion

        #region Public methods
        public GameObject SpawnFromPool(GameObject prefab, Vector3 position, Quaternion rotation)
        {
            string tag = _poolsUserInput.Where(x => x.prefab == prefab).First().tag;

            return SpawnFromPool(tag, position, rotation);
        }

        public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
        {
            if (!_pools.ContainsKey(tag))
            {
                Debug.LogWarning("Pool with tag " + tag + " doesn't exist.");
                return null;
            }

            if (_pools[tag].Count == 0) InstantiateOneItem(tag);

            GameObject objectToSpawn = _pools[tag].Dequeue();
            objectToSpawn.transform.position = position;
            objectToSpawn.transform.rotation = rotation;

            objectToSpawn.SetActive(true);

            foreach (IPooledObject pooledObject in objectToSpawn.GetComponents<IPooledObject>())
            {
                pooledObject.OnObjectSpawn();
                pooledObject.ObjectTag = tag;
            }

            return objectToSpawn;
        }

        public void EnqueueGameObject(GameObject prefab, GameObject toEnqueue)
        {
            string tag = _poolsUserInput.Where(x => x.prefab == prefab).First().tag;

            EnqueueGameObject(tag, toEnqueue);
        }

        public void EnqueueGameObject(string tag, GameObject toEnqueue)
        {
            if (_pools[tag].Contains(toEnqueue))
                return;

            toEnqueue.SetActive(false);
            _pools[tag].Enqueue(toEnqueue);

#if UNITY_EDITOR
            if (Debugging.DynamicsObjects.Instance != null)
                Debugging.DynamicsObjects.Instance.SetToParent(toEnqueue.transform, tag + "_pool");
            else
#endif
                toEnqueue.transform.parent = null;
        }
        #endregion

        #region Private methods
        /// <summary>
        /// If pool empty, instantiate a prefab.
        /// </summary>
        private void InstantiateOneItem(string tag)
        {
            if (!_pools.ContainsKey(tag))
            {
                Debug.LogWarning("Pool with tag " + tag + " doesn't exists.");
                return;
            }

            GameObject prefab = Instantiate(_poolsUserInput.First(x => x.tag == tag).prefab);

            if (prefab == null) return;

            EnqueueGameObject(tag, prefab);
        }
        #endregion
        #endregion
    }
}
