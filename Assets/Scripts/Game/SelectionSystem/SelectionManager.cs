namespace Game.Selection
{
    using Lortedo.Utilities.Pattern;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using UnityEngine.Assertions;
    using UnityEngine.EventSystems;
    using Sirenix.OdinInspector;
    using Lortedo.Utilities;
    using Game.Entities;
    using Game.GameManagers;

    public delegate void OnSelectionUpdated(SelectionManager.SelectionGroup[] selectedGroups, int highlightGroupIndex);

    // REFACTOR NOTE:
    // Split 'input' logic outside this class

    /// <summary>
    /// Manage the player's selection.
    /// </summary>
    public class SelectionManager : Singleton<SelectionManager>
    {
        #region Struct
        public class SelectionGroup
        {
            public string entityID;
            public Team owner;
            public List<Entity> unitsSelected = new List<Entity>();

            public SelectionGroup(string entityID, Team owner)
            {
                this.entityID = entityID;
                this.owner = owner;
            }
        }
        #endregion

        #region Fields
        public static event OnSelectionUpdated OnSelectionUpdated;

        [Header("SETTINGS")]
        [SerializeField] private bool _forceSelectionToHaveOneType = false;
        [EnableIf(nameof(_forceSelectionToHaveOneType))]
        [SerializeField] private EntityType _selectionPriority = EntityType.Unit;
        [Header("COMPONENTS LINKING")]
        [SerializeField] private SelectionRectangle _selectionRectangle;
        [Header("INPUTS")]
        [SerializeField] private KeyCode _keepSelectionKey = KeyCode.LeftShift;

        private List<SelectionGroup> _selectedGroups = new List<SelectionGroup>();
        private int _highlightGroupIndex = -1;

        private bool _selectionEnable = true;
        private EntityType _selectionType;

        private bool _ignoreNextMouseButtonUpInput = false;
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
            Assert.AreEqual(_selectionRectangle.enabled, _selectionEnable, "Selection rectangle should be deactivate if _selectionEnable = false.");

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

        #region Public methods        
        public void IgnoreNextMouseButtonUpInput() => _ignoreNextMouseButtonUpInput = true;

        public void AddEntities(Entity[] selectedEntities)
        {
            if (_forceSelectionToHaveOneType)
            {
                TrimEntities(ref selectedEntities);
            }

            foreach (Entity entity in selectedEntities)
            {
                AddEntity(entity);
            }
        }

        public void AddEntity(Entity selectedEntity)
        {
            if (!_selectionEnable)
                return;

            // prevent covered entity by fog to be selected
            if (selectedEntity.GetCharacterComponent<EntityFogCoverable>().IsCover)
                return;

            // on start selection,
            // set selectionType
            if (_selectedGroups.Count == 0)
            {
                _selectionType = selectedEntity.Data.EntityType;
            }

            if (_forceSelectionToHaveOneType && _selectionType != selectedEntity.Data.EntityType)
            {
                Debug.LogWarningFormat("Selection Manager : " + "Trying to add {0}, but current selection type is {1}.", selectedEntity.name, _selectionType);
                return;
            }

            SelectionGroup groupOfSameEntity = _selectedGroups.FirstOrDefault(x => x.entityID == selectedEntity.EntityID);

            // create group if no group of the same entity exist
            if (groupOfSameEntity == null)
            {
                groupOfSameEntity = new SelectionGroup(selectedEntity.EntityID, selectedEntity.Team);
                _selectedGroups.Add(groupOfSameEntity);

                if (_highlightGroupIndex == -1) _highlightGroupIndex = 0;
            }

            // don't add entity if it already selected
            if (groupOfSameEntity.unitsSelected.Contains(selectedEntity))
            {
                Debug.LogWarning("Selection Manager # " + selectedEntity + " can't be added because is already selected");
                return;
            }

            groupOfSameEntity.unitsSelected.Add(selectedEntity);
            selectedEntity.GetCharacterComponent<EntitySelectable>().IsSelected = true;

            OnSelectionUpdated?.Invoke(_selectedGroups.ToArray(), _highlightGroupIndex);
        }

        public void RemoveEntity(Entity selectableEntity)
        {
            if (!_selectionEnable)
                return;

            SelectionGroup groupWithSameType = _selectedGroups.FirstOrDefault(x => x.entityID == selectableEntity.EntityID);

            // don't remove unselected unit
            if (groupWithSameType == null ||
               (groupWithSameType != null && groupWithSameType.unitsSelected.Contains(selectableEntity) == false))
            {
                return;
            }

            selectableEntity.GetCharacterComponent<EntitySelectable>().IsSelected = false;
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
                    item.unitsSelected[j].GetCharacterComponent<EntitySelectable>().IsSelected = false;
                }
            }

            _selectedGroups.Clear();
            _highlightGroupIndex = -1;

            OnSelectionUpdated?.Invoke(_selectedGroups.ToArray(), _highlightGroupIndex);
        }

        public Vector3[] GetSelectedEntitiesPositions()
        {
            int pointsCount = _selectedGroups.Sum(x => x.unitsSelected.Count);

            // get position in each entity of each group.
            Vector3[] points = new Vector3[pointsCount];

            int index = 0;
            foreach (SelectionGroup group in SelectedGroups)
            {
                foreach (Entity entity in group.unitsSelected)
                {
                    points[index] = entity.transform.position;
                    index++;
                }
            }

            return points;
        }

        public Vector3 GetSelectedEntitesCentroid()
        {
            return Math.GetCentroid(GetSelectedEntitiesPositions());
        }
        #endregion

        #region Private methods        
        /// <summary>
        /// Handle input when player click on non entity to CLEAR SELECTION.
        /// </summary>
        private void HandleInput_ClickOnEntity()
        {
            if (Input.GetMouseButtonDown(0))
            {
                if (_ignoreNextMouseButtonUpInput)
                {
                    _ignoreNextMouseButtonUpInput = false;
                    return;
                }

                // if player is selecting, don't clear selection
                if (_selectionRectangle.IsSelecting)
                    return;

                // if player click on UI, don't clear selection
                if (EventSystem.current.IsPointerOverGameObject(-1))
                    return;

                if (Input.GetKey(_keepSelectionKey) == false)
                {
                    ClearSelection();
                }

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Entity")))
                {
                    if (hit.transform.TryGetComponent(out Entity hitEntity))
                    {
                        SwitchEntity(hitEntity);
                    }
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

        /// <summary>
        /// Remove entities that haven't the same type
        /// </summary>
        private void TrimEntities(ref Entity[] selectedEntities)
        {
            bool selectedEntitiesHasUnit = selectedEntities
                   .Where(x => x.Data.EntityType == _selectionPriority)
                   .FirstOrDefault() != null;

            // remove non EntityType.Unit
            if (selectedEntitiesHasUnit)
            {
                selectedEntities = selectedEntities
                    .Where(x => x.Data.EntityType == _selectionPriority)
                    .ToArray();
            }
            else
            {
                // keep non EntityType.Unit
            }
        }
        #endregion
        #endregion
    }
}

