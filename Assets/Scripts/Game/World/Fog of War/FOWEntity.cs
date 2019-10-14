using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FogOfWar
{
    [System.Serializable]
    public class FOWEntity : MonoBehaviour
    {
        #region Enum
        public enum Type
        {
            Viewer = 0,
            Coverable = 1
        }
        #endregion

        #region Fields
        [Header("Viewer Settings")]
        [SerializeField] private SpriteRenderer _fogOfWarVision = null;
        [Header("Coverable Settings")]
        [SerializeField] private FOWCoverable _coverable = new FOWCoverable();

        private Entity _owner;
        private Type _type;
        #endregion

        #region Fields
        public bool IsCover { get => _type == Type.Viewer ? false : _coverable.IsCover; }
        public float ViewRadius { get => _owner.Data.VisionRange; }
        public FOWCoverable Coverable { get => _coverable; }
        #endregion

        #region Methods
        #region MonoBehaviour Callbacks
        void Start()
        {
            _owner = GetComponent<Entity>();            

            SetTypeField();
            RegisterToManager();
        }

        void OnDestroy()
        {
            RemoveFromFOWManager();
        }
        #endregion

        void RemoveFromFOWManager()
        {
            switch (_type)
            {
                case Type.Viewer:
                    FOWManager.Instance?.RemoveViewer(this);
                    break;

                case Type.Coverable:
                    FOWManager.Instance?.RemoveCoverable(this);
                    break;
            }
        }

        void SetTypeField()
        {
            if (_owner.Owner == Owner.Sparta)
            {
                _type = Type.Viewer;
            }
            else
            {
                _type = Type.Coverable;
            }
        }

        void RegisterToManager()
        {
            switch (_type)
            {
                case Type.Viewer:
                    FOWManager.Instance.AddViewer(this);

                    _fogOfWarVision.gameObject.SetActive(true);
                    _fogOfWarVision.transform.localScale = Vector3.one * ViewRadius * 2;
                    break;

                case Type.Coverable:
                    FOWManager.Instance.AddCoverable(this);

                    _fogOfWarVision.gameObject.SetActive(false);
                    break;
            }
        }
    }
    #endregion
}
