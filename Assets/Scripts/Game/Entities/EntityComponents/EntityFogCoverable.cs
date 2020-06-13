namespace Game.Entities
{
    using Game.FogOfWar;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Assertions;

    public delegate void OnFog(IFogCoverable fogCoverable);

    public class EntityFogCoverable : AbstractEntityComponent, IFogCoverable
    {
        #region Fields
        private const string debugLogHeader = "Entity Fog Coverable : ";

        [SerializeField] private GameObject[] _modelsToHide;

        private Collider _collider;
        private bool _isCover = false;
        #endregion

        #region Events
        public event OnFog OnFogCover;
        public event OnFog OnFogUncover;
        #endregion

        #region Properties
        public bool IsCover
        {
            get => _isCover;

            set
            {
                // REFACTOR NOTE:
                // We should only do checks in properties!

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
        #endregion

        #region Methods
        void Awake()
        {
            _collider = GetComponent<Collider>();
        }

        void OnEnable()
        {
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
        #endregion
    }
}
