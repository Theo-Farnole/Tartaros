using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FogOfWar
{
    [RequireComponent(typeof(Entity))]
    public class FOWEntity : MonoBehaviour
    {
        #region Fields
        [Header("Viewer Settings")]
        [SerializeField] private float _viewRadius = 3;
        [SerializeField] private SpriteRenderer _fogOfWarVision = null;
        [Header("Coverable Settings")]
        [SerializeField] private Coverable _coverable;

        // cache variable
        private Entity _entity;
        #endregion

        #region Fields
        public float ViewRadius { get => _viewRadius; }
        public bool IsCover
        {
            get
            {
                if (_entity.Owner == Owner.Sparta)
                {
                    return false;
                }
                else
                {
                    Debug.Log("return _cov.IsCover = " + _coverable.IsCovered);
                    return _coverable.IsCovered;
                }
            }
        }
        #endregion

        #region Methods
        void Awake()
        {
            _entity = GetComponent<Entity>();
        }

        void Start()
        {
            if (_entity.Owner == Owner.Sparta)
            {
                FOWManager.Instance.AddViewer(this);
                _fogOfWarVision.gameObject.SetActive(true);
                _fogOfWarVision.transform.localScale = Vector3.one * _viewRadius * 2;
            }
            else
            {
                FOWManager.Instance.AddCoverable(_coverable);
                _fogOfWarVision.enabled = false;
            }
        }
        #endregion
    }
}
