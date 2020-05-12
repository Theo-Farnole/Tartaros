using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lortedo.Utilities.Debugging
{
    /// <summary>
    /// Keeps a clean hierarchy in the Editor while the game runs.
    /// Set parents to new instantited objects.
    /// </summary>
    public class DynamicsObjects : Pattern.Singleton<DynamicsObjects>
    {
        #region Fields
        [SerializeField] private string[] _parentsTag;

        private Dictionary<string, Transform> _parents = new Dictionary<string, Transform>();
        #endregion

        #region Methods
        #region MonoBehaviour Callbacks
        void Awake()
        {
            // create parents, modify their name, and set their parent
            for (int i = 0; i < _parentsTag.Length; i++)
            {
                AddParent(_parentsTag[i]);
            }
        }
        #endregion

        /// <summary>
        /// Instantiate parent, then update name with tag.
        /// </summary>
        public void AddParent(string tag)
        {
            Transform obj = new GameObject().transform;
            obj.name = "Parent " + tag;
            obj.parent = transform;

            _parents.Add(tag, obj);
        }

        /// <summary>
        /// Assign the parent of the tag to an object.
        /// </summary>
        /// <param name="obj">The object to whom to assign the parent</param>
        /// <param name="tag">The tag of the parent to assign</param>
        public void SetToParent(Transform obj, string tag)
        {
#if UNITY_EDITOR
            if (!_parents.ContainsKey(tag))
            {
                Debug.LogWarning("Parent type " + tag + " doesn't exists.");
                return;
            }

            obj.parent = _parents[tag];
#else
        obj.parent = null;
#endif
        }
        #endregion
    }
}
