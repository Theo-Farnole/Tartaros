using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FogOfWar
{
    [System.Serializable]
    public class FOWCoverable
    {
        [SerializeField] private Collider _collider;
        [SerializeField] private Renderer _renderer;

        private bool _isCover = false;

        public bool IsCover
        {
            get => _isCover;

            set
            {
                _isCover = value;

                _collider.enabled = !_isCover;
                _renderer.enabled = !_isCover;
            }
        }
    }
}
