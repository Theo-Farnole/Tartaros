namespace Game.Entities
{
    using Game.FogOfWar;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public class EntityFogVision : AbstractEntityComponent, IFogVision
    {
        #region Fields
        [Header("Viewer Settings")]
        [SerializeField] private SpriteRenderer _fogOfWarVision = null;
        #endregion

        #region Properties
        public float ViewRadius => Entity.Data.ViewRadius;
        public Vector3 Position => transform.position;
        #endregion

        #region Methods
        void Start()
        {
            Transform parent = transform.parent;

            _fogOfWarVision.transform.SetParent(null);
            _fogOfWarVision.transform.localScale = Vector3.one * Entity.Data.ViewRadius * 2;
            _fogOfWarVision.transform.SetParent(null, true);
        }

        void OnEnable()
        {
            _fogOfWarVision.enabled = true;
            FOWManager.Instance.AddViewer(this);
        }

        void OnDisable()
        {
            _fogOfWarVision.enabled = false;
            FOWManager.Instance?.RemoveViewer(this);
        }

        void OnDestroy()
        {
            if (_fogOfWarVision != null)
            {
                Destroy(_fogOfWarVision.gameObject);
            }
        }
        #endregion
    }
}
