namespace Game.FogOfWar
{
    using UnityEngine;

    public class FogCoveredObject : MonoBehaviour, IFogCoverable
    {
        [SerializeField] private MeshRenderer[] _meshRenderers = null;
        [SerializeField] private bool _dontHideOnRecover = true;

        private bool _isCover = false;
        private int _coveredCount = 0;

        public bool IsCover
        {
            get => _isCover;

            set
            {
                _isCover = value;
                
                if (_isCover == true)
                {
                    _coveredCount++;
                }

                UpdateMeshRenderers();
            }
        }

        public Vector3 Position => transform.position;

        void OnEnable()
        {
            FOWManager.Instance.AddCoverable(this);
        }

        void OnDisable()
        {
            FOWManager.Instance.RemoveCoverable(this);    
        }

        void UpdateMeshRenderers()
        {
            if (_dontHideOnRecover && _isCover && _coveredCount > 1)            
                return;            

            for (int i = 0; i < _meshRenderers.Length; i++)
            {
                _meshRenderers[i].enabled = !_isCover;
            }
        }
    }
}
