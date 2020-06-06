using Game.FogOfWar;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Game.Entities
{
    public delegate void OnFog(IFogCoverable fogCoverable);

    public class EntityFogCoverable : EntityComponent, IFogCoverable
    {
        private const string debugLogHeader = "Entity Fog Coverable : ";

        public event OnFog OnFogCover;
        public event OnFog OnFogUncover;

        [SerializeField] private GameObject[] _modelsToHide;
        private Collider _collider;

        private bool _isCover = false;

        public bool IsCover
        {
            get => _isCover;

            set
            {
                // avoid calculation if _isCover value doesn't changes
                if (value == _isCover)
                    return;

                if (value && !_isCover) OnFogCover?.Invoke(this);
                else if (!value && _isCover) OnFogUncover?.Invoke(this);

                _isCover = value;
                UpdateVisibility();
            }
        }

        public Vector3 Position { get => transform.position; }

        private void Awake()
        {
            _collider = GetComponent<Collider>();
        }

        void OnEnable()
        {
            Assert.IsNotNull(FOWManager.Instance, string.Format(debugLogHeader + "FOWManager is missing. Can't add {0} as a coverable", name));

            FOWManager.Instance.AddCoverable(this);
        }

        void OnDisable()
        {
            if (FOWManager.Instance != null)
            {
                FOWManager.Instance.RemoveCoverable(this);
            }
        }

        void UpdateVisibility()
        {
            _collider.enabled = !_isCover;

            foreach (var mesh in _modelsToHide)
                mesh.SetActive(!_isCover);
        }
    }
}
