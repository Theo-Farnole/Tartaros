using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FogOfWar
{
    [System.Serializable]
    public class FOWCoverable
    {
        [SerializeField] private GameObject _renderer;
        [HideInInspector] public Collider collider;

        private bool _isCover = false;

        public bool IsCover
        {
            get => _isCover;

            set
            {
                _isCover = value;

                collider.enabled = !_isCover;
                _renderer.SetActive(!_isCover);
            }
        }
    }
}
