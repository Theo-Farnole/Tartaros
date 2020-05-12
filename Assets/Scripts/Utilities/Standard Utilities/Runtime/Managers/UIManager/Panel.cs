using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lortedo.Utilities.Managers
{
    [System.Serializable]
    public abstract class Panel
    {
        [SerializeField] private GameObject _root;        

        public GameObject Root { get => _root; }

        public virtual void Initialize<T>(T uiManager) where T : AbstractUIManager
        { }

        public virtual void OnValidate()
        { }

        public virtual void OnStateEnter() { }
        public virtual void OnStateExit() { }
    }
}
