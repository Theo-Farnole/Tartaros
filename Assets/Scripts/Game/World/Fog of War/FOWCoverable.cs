using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FogOfWar
{
    [System.Serializable]
    public class Coverable
    {
        public Transform transform;
        [Space]
        [SerializeField] Collider _collider;
        [SerializeField] Renderer _renderer;

        private bool _isCovered;

        public bool IsCovered { get => _isCovered; }

        public void Cover(bool isCovered)
        {
            Debug.Log("Set cover to " + isCovered + ".");
            _isCovered = isCovered;

            _collider.enabled = !_isCovered;
            _renderer.enabled = !_isCovered;
        }
    }
}
