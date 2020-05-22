using Lortedo.Utilities.Pattern;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Game.UI;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;

namespace Game.Selection
{

    public delegate void OnSelectionUpdated(SelectionManager.SelectionGroup[] selectedGroups, int highlightGroupIndex);

    /// <summary>
    /// Manage the player's selection.
    /// </summary>
    public class SelectionManager : Singleton<SelectionManager>
    {
        #region Struct
        public class SelectionGroup
        {
            public EntityType entityType;
            public Team owner;
            public List<Entity> unitsSelected = new List<Entity>();

            public SelectionGroup(EntityType entityType, Team owner)
            {
                this.entityType = entityType;
                this.owner = owner;
            }
        }
        #endregion

        #region Fields
        public static event OnSelectionUpdated OnSelectionUpdated;

        [SerializeField] private SelectionRectangle _selectionRectangle;

        private List<SelectionGroup> _selectedGroups = new List<SelectionGroup>();
        private int _highlightGroupIndex = -1;

        private bool _selectionEnable = true;
        #endregion

        #region Properties
        public bool HasSelection { get => _selectedGroups.Count > 0; }
        public SelectionGroup[] SpartanGroups { get => (from x in _selectedGroups where x.owner == Team.Player select x).ToArray(); }
        public List<SelectionGroup> SelectedGroups { get => _selectedGroups; }
        #endregion

        #region Methods
        #region MonoBehaviour Callback
        void Update()
        {
            Assert.AreEqual(_selectionRectangle.enabled, _selectionEnable, 
                "Selection rectangle should be deactivate if _selectionEnable = false.");

            HandleInput_SwitchHighlightGroup();
            HandleInput_ClickOnEntity();

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                ClearSelection();
            }
        }

        void OnEnable()
        {
            GameManager.OnStartBuild += GameManager_OnStartBuild;
            GameManager.OnStopBuild += GameManager_OnStopBuild;
        }

        void OnDisable()
        {
            GameManager.OnStartBuild -= GameManager_OnStartBuild;
            GameManager.OnStopBuild -= GameManager_OnStopBuild;            
        }
        #endregion

        #region Events handlers
        private void GameManager_OnStartBuild(GameManager gameManager)
        {
            _selectionEnable = false;
            _selectionRectangle.enabled = _selectionEnable;
            ClearSelection();
        }

        private void GameManager_OnStopBuild(GameManager gameManager)
        {
            _selectionEnable = true;
            _selectionRectangle.enabled = _selectionEnable;
        }
        #endregion

        #region Private methods
        /// <summary>
        /// Handle input when player click on non entity to CLEAR SELECTION.
        /// </summary>
        private void HandleInput_ClickOnEntity()
        {
            if (Input.GetMouseButtonUp(0))
            {
                // if player is selecting, don't clear selection
                if (_selectionRectangle.IsSelecting)
                    return;

                // if player click on UI, don't clear selection
                if (EventSystem.current.IsPointerOverGameObject(-1))
                    return;

                if (Input.GetKey(KeyCode.LeftShift) == false)
                {
                    ClearSelection();
                }

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Entity")))
                {
                    Entity hitEntity = hit.transform.GetComponent<Entity>();
                    SwitchEntity(hitEntity);
                }
            }
        }

        private void HandleInput_SwitchHighlightGroup()
        {
            if (Input.GetKeyDown(KeyCode.Tab))
            {
                _highlightGroupIndex++;
                if (_highlightGroupIndex >= _selectedGroups.Count) _highlightGroupIndex = 0;

                OnSelectionUpdated?.Invoke(_selectedGroups.ToArray(), _highlightGroupIndex);
            }
        }
        #endregion

        #region Public methods
        public void AddEntity(Entity selectableEntity)
        {
            if (!_selectionEnable)
                return;

            // prevent covered entity by fog to be selected
            if (selectableEntity.GetCharacterComponent<EntityFogCoverable>().IsCover)
                return;

            SelectionGroup groupOfSameEntity = _selectedGroups.FirstOrDefault(x => x.entityType == selectableEntity.Type);

            // create group if no groud of the same entity exist
            if (groupOfSameEntity == null)
            {
                groupOfSameEntity = new SelectionGroup(selectableEntity.Type, selectableEntity.Team);
                _selectedGroups.Add(groupOfSameEntity);

                if (_highlightGroupIndex == -1) _highlightGroupIndex = 0;
            }

            // don't add entity if it already selected
            if (groupOfSameEntity.unitsSelected.Contains(selectableEntity))
            {
                Debug.LogWarning("Selection Manager # " + selectableEntity + " can't be added because is already selected");
                return;
            }

            groupOfSameEntity.unitsSelected.Add(selectableEntity);
            selectableEntity.GetCharacterComponent<EntitySelectable>().OnSelected();

            OnSelectionUpdated?.Invoke(_selectedGroups.ToArray(), _highlightGroupIndex);
        }

        public void RemoveEntity(Entity selectableEntity)
        {
            if (!_selectionEnable)
                return;

            SelectionGroup groupWithSameType = _selectedGroups.FirstOrDefault(x => x.entityType == selectableEntity.Type);

            // don't remove unselected unit
            if (groupWithSameType == null ||
               (groupWithSameType != null && groupWithSameType.unitsSelected.Contains(selectableEntity) == false))
            {
                return;
            }

            selectableEntity.GetCharacterComponent<EntitySelectable>().OnDeselect();
            groupWithSameType.unitsSelected.Remove(selectableEntity);

            // delete group if empty
            if (groupWithSameType.unitsSelected.Count == 0)
            {
                int removeGroupIndex = _selectedGroups.IndexOf(groupWithSameType);

                _selectedGroups.Remove(groupWithSameType);

                // update HighlightGroupIndex
                if (removeGroupIndex < _highlightGroupIndex)
                {
                    _highlightGroupIndex--;
                    if (_highlightGroupIndex < 0) _highlightGroupIndex = 0;
                }
            }

            if (_selectedGroups.Count == 0) _highlightGroupIndex = -1;

            OnSelectionUpdated?.Invoke(_selectedGroups.ToArray(), _highlightGroupIndex);
        }

        /// <summary>
        /// Remove SelectableEntity if it's selected. And add it, if it's not selected.
        /// </summary>
        public void SwitchEntity(Entity toSwitchUnit)
        {
            bool isEntitySelected = _selectedGroups.Exists(x => x.unitsSelected.Contains(toSwitchUnit));

            if (isEntitySelected)
            {
                RemoveEntity(toSwitchUnit);
            }
            else
            {
                AddEntity(toSwitchUnit);
            }
        }

        public void ClearSelection()
        {
            foreach (var item in _selectedGroups)
            {
                for (int j = 0; j < item.unitsSelected.Count; j++)
                {
                    item.unitsSelected[j].GetCharacterComponent<EntitySelectable>().OnDeselect();
                }
            }

            _selectedGroups.Clear();
            _highlightGroupIndex = -1;

            OnSelectionUpdated?.Invoke(_selectedGroups.ToArray(), _highlightGroupIndex);
        }
        #endregion
        #endregion
    }
}

