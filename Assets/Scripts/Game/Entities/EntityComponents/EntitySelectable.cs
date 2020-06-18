namespace Game.Entities
{
    using Game.Selection;
    using Lortedo.Utilities.Pattern;
    using UnityEngine;

    public delegate void OnSelected(Entity entity);
    public delegate void OnUnselected(Entity entity);

    /// <summary>
    /// Manage display of selection circle.
    /// </summary>
    public class EntitySelectable : AbstractEntityComponent
    {
        #region Fields
        private static int layerMaskTerrain = -1;

        private GameObject _selectionCircle = null;
        private bool _isSelected = false;
        #endregion

        #region Events
        public event OnSelected OnSelected;
        public event OnUnselected OnUnselected;
        #endregion

        #region Properties
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                // REFACTOR NOTE:
                // Properties should only do checks!

                if (value == _isSelected)
                    return;

                bool oldIsSelected = _isSelected;

                _isSelected = value;

                if (_isSelected)
                {
                    OnSelected?.Invoke(Entity);
                    OnSelection();
                }
                else
                {
                    OnUnselected?.Invoke(Entity);
                    OnUnselection();
                }
            }
        }
        #endregion

        #region Methods
        #region Mono Callbacks
        void Awake()
        {
            if (layerMaskTerrain == -1)
                layerMaskTerrain = LayerMask.GetMask("Terrain");
        }

        void OnEnable()
        {
            Entity.GetCharacterComponent<EntityFogCoverable>().OnFogCover += OnFogCover;
        }

        void OnDisable()
        {
            Entity.GetCharacterComponent<EntityFogCoverable>().OnFogCover -= OnFogCover;

            if (SelectionManager.Instance != null)
            {
                SelectionManager.Instance.RemoveEntity(Entity);
            }

            IsSelected = false;
        }
        #endregion

        #region Events handlers
        void OnFogCover(FogOfWar.IFogCoverable fogCoverable)
        {
            SelectionManager.Instance.RemoveEntity(Entity);
            IsSelected = false;
        }
        #endregion

        #region Private methods
        private void OnSelection()
        {
            DisplaySelectionCircle();
        }

        private void OnUnselection()
        {
            HideSelectionCircle();
        }

        private void DisplaySelectionCircle()
        {
            if (_selectionCircle != null)
            {
                Debug.LogWarning("Entity Selectable : Cannot display selection, it's already displayed. But it's okay.");
                return;
            }

            Ray ray = new Ray(
                transform.position + Vector3.up, // transform.position should already touch the terrain. We raise a lit bittle to make the raycast works
                Vector3.down
            );

            Vector3 pos = Physics.Raycast(ray, out RaycastHit hit, layerMaskTerrain) ?
                hit.point + Vector3.up * 0.1f :
                transform.position;
            Quaternion rot = Quaternion.Euler(90, 0, 0);

            _selectionCircle = ObjectPooler.Instance.SpawnFromPool(ObjectPoolingTags.keySelectionCircle, pos, rot);

            if (_selectionCircle == null)
            {
                Debug.LogErrorFormat("Entity Selectable : No selection circle pool in found. Please check your ObjectPooler.");
                return;
            }

            _selectionCircle.transform.parent = transform;

            SelectionCircle selectionCircle = _selectionCircle.GetComponent<SelectionCircle>();
            selectionCircle.SetCircleColor(Entity.Team);
            selectionCircle.SetSize(Entity.Data.GetRadius());
        }

        private void HideSelectionCircle()
        {
            if (_selectionCircle == null)
                return;

            // If we enqueue selection circle on Entity's death, we get this error :
            // "Cannot set the parent of the GameObject while activating or deactivating the parent GameObject."
            if (ObjectPooler.Instance != null && Entity.IsSpawned)
            {
                ObjectPooler.Instance.EnqueueGameObject(ObjectPoolingTags.keySelectionCircle, _selectionCircle);
            }
            else
            {
                Destroy(_selectionCircle);
            }

            _selectionCircle = null;
        }
        #endregion
        #endregion
    }
}
