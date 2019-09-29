using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class SelectionManager : Singleton<SelectionManager>
{
    #region Struct
    public class Group
    {
        public EntityType entityType;
        public Owner owner;
        public List<SelectableEntity> selectedEntities = new List<SelectableEntity>();

        public Group(EntityType entityType, Owner owner)
        {
            this.entityType = entityType;
            this.owner = owner;
        }
    }
    #endregion

    #region Fields
    [SerializeField] private SelectionRect _selectionRect;

    private List<Group> _selectedGroups = new List<Group>();
    private int _highlightGroupIndex = -1;
    #endregion

    #region Properties
    public Group[] SpartanGroups { get => (from x in _selectedGroups where x.owner == Owner.Sparta select x).ToArray(); }
    public List<Group> SelectedGroups { get => _selectedGroups; }
    #endregion

    #region Methods
    #region MonoBehaviour Callback
    void Update()
    {
        ManageCommandsExecuter();
        ManageHighlightGroupInput();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ClearSelection();
        }

        if (EventSystem.current.IsPointerOverGameObject(-1))
            return;

        if (Input.GetMouseButtonUp(0) && !_selectionRect.IsSelecting)
        {
            SwitchEntityUnderMouse();
        }
    }
    #endregion

    #region Mouse Click
    public void SwitchEntityUnderMouse()
    {
        if (Input.GetKey(KeyCode.LeftShift) == false)
        {
            ClearSelection();
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        // check if entities should ATTACK
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Entity")))
        {
            SelectableEntity hittedSelectableEntity = hit.transform.GetComponent<SelectableEntity>();

            if (hittedSelectableEntity)
            {
                SwitchEntity(hittedSelectableEntity);
            }
        }
    }
    #endregion

    #region Highlight Group Manager
    void ManageHighlightGroupInput()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            _highlightGroupIndex++;
            if (_highlightGroupIndex >= _selectedGroups.Count) _highlightGroupIndex = 0;

            UIManager.Instance.UpdateSelectedGroups(_selectedGroups.ToArray(), _highlightGroupIndex);
        }
    }
    #endregion

    #region Commands Executer
    /// <summary>
    /// If right click pressed, order attack or movement to Spartan selected groups.
    /// </summary>
    void ManageCommandsExecuter()
    {
        if (Input.GetMouseButton(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Entity")))
            {
                OrderAttack(hit.transform);
            }
            else if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Grid")))
            {
                OrderMovement(hit.point);
            }
        }
    }

    /// <summary>
    /// Order attack to Spartan selected groups, except if target is also Spartan.
    /// </summary>
    /// <param name="target">Transform of the attack's target.</param>
    void OrderAttack(Transform target)
    {
        if (target.GetComponent<OwnedEntity>().Owner == Owner.Sparta)
            return;

        // order attack
        foreach (Group group in SpartanGroups)
        {
            for (int i = 0; i < group.selectedEntities.Count; i++)
            {
                group.selectedEntities[i].CommandReceiverEntity.Attack(target);
            }
        }
    }

    /// <summary>
    /// Order movement to Spartan selected groups.
    /// </summary>
    /// <param name="destination">Position of the wanted destination</param>
    void OrderMovement(Vector3 destination)
    {
        foreach (Group group in SpartanGroups)
        {
            if (group.owner != Owner.Sparta)
                continue;

            for (int j = 0; j < group.selectedEntities.Count; j++)
            {
                group.selectedEntities[j].CommandReceiverEntity.Move(destination);
            }
        }
    }
    #endregion

    #region SelectedGroups Manager
    public void AddEntity(SelectableEntity selectableEntity)
    {
        Group groupWithSameType = _selectedGroups.FirstOrDefault(x => x.entityType == selectableEntity.Type);

        // create group
        if (groupWithSameType == null)
        {
            groupWithSameType = new Group(selectableEntity.Type, selectableEntity.Owner);
            _selectedGroups.Add(groupWithSameType);

            if (_highlightGroupIndex == -1) _highlightGroupIndex = 0;
        }

        // check if entity isn't selected
        if (groupWithSameType.selectedEntities.Contains(selectableEntity))
        {
            Debug.Log("Selection Manager # " + selectableEntity + " can't be added because is already selected");
            return;
        }

        // add to selectGroup and trigger OnSelect
        groupWithSameType.selectedEntities.Add(selectableEntity);
        selectableEntity.OnSelected();

        UIManager.Instance.UpdateSelectedGroups(_selectedGroups.ToArray(), _highlightGroupIndex);
    }

    public void RemoveEntity(SelectableEntity selectableEntity)
    {
        Group groupWithSameType = _selectedGroups.FirstOrDefault(x => x.entityType == selectableEntity.Type);

        if (groupWithSameType == null ||
           (groupWithSameType != null && groupWithSameType.selectedEntities.Contains(selectableEntity) == false))
        {
            Debug.LogWarning("Can't remove selectableEntity that's not selected!");
            return;
        }

        selectableEntity.OnDeselect();
        groupWithSameType.selectedEntities.Remove(selectableEntity);

        // delete group if empty
        if (groupWithSameType.selectedEntities.Count == 0)
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

        UIManager.Instance.UpdateSelectedGroups(_selectedGroups.ToArray(), _highlightGroupIndex);
    }

    /// <summary>
    /// Remove SelectableEntity if it's selected. And add it, if it's not selected.
    /// </summary>
    public void SwitchEntity(SelectableEntity selectableEntity)
    {
        bool isEntitySelected = _selectedGroups.Exists(x => x.selectedEntities.Contains(selectableEntity));

        if (isEntitySelected)
        {
            RemoveEntity(selectableEntity);
        }
        else
        {
            AddEntity(selectableEntity);
        }
    }

    public void ClearSelection()
    {
        foreach (var item in _selectedGroups)
        {
            for (int j = 0; j < item.selectedEntities.Count; j++)
            {
                item.selectedEntities[j].OnDeselect();
            }
        }

        _selectedGroups.Clear();
        _highlightGroupIndex = -1;

        UIManager.Instance.UpdateSelectedGroups(_selectedGroups.ToArray(), _highlightGroupIndex);
    }
    #endregion
    #endregion
}
