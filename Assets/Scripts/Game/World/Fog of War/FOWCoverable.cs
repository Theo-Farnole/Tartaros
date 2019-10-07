using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FogOfWar
{
    [System.Serializable]
    public class FOWCoverable
    {
        [SerializeField] private Transform _transform;
        [Space]
        [SerializeField] private Collider _collider;
        [SerializeField] private Renderer _renderer;

        private bool _isCovered = false;

        public bool IsCovered { get => _isCovered; }
        public Transform Transform { get => _transform; }

        public void Cover(bool isCovered)
        {
            _isCovered = isCovered;

            _collider.enabled = !_isCovered;
            _renderer.enabled = !_isCovered;
        }
    }
}
