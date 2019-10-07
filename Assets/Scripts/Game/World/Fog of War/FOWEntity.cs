using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FogOfWar
{
    [System.Serializable]
    public class FOWEntity
    {
        #region Fields
        [Header("Viewer Settings")]
        [SerializeField] private SpriteRenderer _fogOfWarVision = null;
        [Header("Coverable Settings")]
        [SerializeField] private FOWCoverable _coverable = new FOWCoverable();

        private Entity _owner;
        #endregion

        #region Fields
        public float ViewRadius { get => _owner.Data.VisionRange; }
        public Transform Transform { get => _owner.transform; }
        public bool IsCover { get => _owner.Owner == Owner.Sparta ? false : _coverable.IsCovered; }
        #endregion

        #region Methods
        public void Init(Entity owner)
        {
            _owner = owner;

            if (_owner.Owner == Owner.Sparta)
            {
                FOWManager.Instance.AddViewer(this);
                _fogOfWarVision.gameObject.SetActive(true);
                _fogOfWarVision.transform.localScale = Vector3.one * ViewRadius * 2;
            }
            else
            {
                FOWManager.Instance.AddCoverable(_coverable);
                _fogOfWarVision.enabled = false;
            }
        }

        public void RemoveFromFOWManager()
        {
            if (_owner.Owner == Owner.Sparta)
            {
                FOWManager.Instance?.RemoveViewer(this);
            }
            else
            {
                FOWManager.Instance?.RemoveCoverable(_coverable);
            }
        }
    }
    #endregion
}
