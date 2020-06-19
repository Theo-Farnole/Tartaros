namespace Game.FogOfWar
{
    using UnityEngine;

    public class FogCoveredObject : MonoBehaviour, IFogCoverable
    {
        #region Fields
        [SerializeField] private MeshRenderer[] _meshRenderers = null;
        [SerializeField] private bool _dontHideOnRecover = true;

        private bool _isCover = false;
        private int _coveredCount = 0;
        #endregion        

        #region IFogCoverable Interfaces
        bool IFogCoverable.IsCover
        {
            get => _isCover;

            set
            {                
                if (value && value != _isCover)                
                    _coveredCount++;                

                _isCover = value;     

                UpdateMeshRenderers();
            }
        }

        Vector3 IFogCoverable.Position => transform.position;
        #endregion

        #region Private Methods
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

        void UpdateMeshRenderers()
        {
            if (_dontHideOnRecover && IsRecover())            
                return;            

            for (int i = 0; i < _meshRenderers.Length; i++)
            {
                _meshRenderers[i].enabled = !_isCover;
            }
        }

        bool IsRecover() => _isCover && _coveredCount > 1;
        #endregion
    }
}
