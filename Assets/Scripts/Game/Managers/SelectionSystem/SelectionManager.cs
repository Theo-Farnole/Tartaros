using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SelectionManager : Singleton<SelectionManager>
{
    #region Struct
    public struct SelectionKey
    {
        public EntityType entityType;
        public Owner owner;

        public SelectionKey(EntityType type, Owner owner)
        {
            this.entityType = type;
            this.owner = owner;
        }

        public SelectionKey(SelectableEntity selectableEntity)
        {
            entityType = selectableEntity.Type;
            owner = selectableEntity.Owner;
        }
    }
    #endregion

    #region Fields
    [SerializeField] private SelectionRect _selectionRect;

    private Dictionary<SelectionKey, List<SelectableEntity>> _selectedGroups = new Dictionary<SelectionKey, List<SelectableEntity>>();
    private int _highlightGroupIndex = -1;
    #endregion

    #region Properties
    public KeyValuePair<SelectionKey, List<SelectableEntity>>[] SelectedGroups { get => _selectedGroups.ToArray(); }
    public KeyValuePair<SelectionKey, List<SelectableEntity>>[] SpartanGroups { get => (from x in _selectedGroups where x.Key.owner == Owner.Sparta select x).ToArray(); }
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

            if (_highlightGroupIndex >= _selectedGroups.Count)
            {
                _highlightGroupIndex = 0;
            }

            UIManager.Instance.UpdateHighlightGroup(_highlightGroupIndex);
        }
    }
    #endregion

    #region Commands Executer
    /// <summary>
    /// Must be called in Update()
    /// </summary>
    void ManageCommandsExecuter()
    {
        if (Input.GetMouseButton(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            // check if entities should ATTACK
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Entity")))
            {
                Transform target = hit.transform;

                if (target.GetComponent<OwnedEntity>().Owner == Owner.Sparta)
                    return;

                // order attack
                foreach (var group in SpartanGroups)
                {
                    for (int i = 0; i < group.Value.Count; i++)
                    {
                        group.Value[i].CommandReceiverEntity.Attack(target);
                    }
                }
            }
            // check if entities should MOVE
            else if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Grid")))
            {
                Vector3 target = hit.point;

                foreach (var item in _selectedGroups)
                {
                    if (item.Key.owner != Owner.Sparta)
                        continue;

                    for (int j = 0; j < item.Value.Count; j++)
                    {
                        item.Value[j].CommandReceiverEntity.Move(target);
                    }
                }
            }
        }
    }
    #endregion

    #region SelectedGroups Manager
    public void AddEntity(SelectableEntity selectableEntity)
    {
        SelectionKey selectionKey = new SelectionKey(selectableEntity);

        // create entry in the dictionary
        if (_selectedGroups.ContainsKey(selectionKey) == false)
        {
            _selectedGroups.Add(selectionKey, new List<SelectableEntity>());
        }

        if (_selectedGroups[selectionKey].Contains(selectableEntity))
        {
            Debug.Log("Selection Manager # " + selectableEntity + " can't be added because is already selected");
            return;
        }

        selectableEntity.OnSelected();
        _selectedGroups[selectionKey].Add(selectableEntity);

        UIManager.Instance.UpdateHighlightGroup(_highlightGroupIndex);
        UIManager.Instance.UpdateSelectedGroups(_selectedGroups.ToArray());
    }

    public void RemoveEntity(SelectableEntity selectableEntity)
    {
        SelectionKey selectionKey = new SelectionKey(selectableEntity);

        if (_selectedGroups.ContainsKey(selectionKey) == false)
        {
            Debug.LogWarning("Can't delete " + selectionKey + " because it isn't selected");
            return;
        }

        selectableEntity.OnDeselect();

        _selectedGroups[selectionKey].Remove(selectableEntity);
        if (_selectedGroups[selectionKey].Count == 0)
        {
            _selectedGroups.Remove(selectionKey);
        }

        UIManager.Instance.UpdateHighlightGroup(_highlightGroupIndex);
        UIManager.Instance.UpdateSelectedGroups(_selectedGroups.ToArray());
    }

    /// <summary>
    /// Remove SelectableEntity if it's selected. And add it, if it's not selected.
    /// </summary>
    public void SwitchEntity(SelectableEntity selectableEntity)
    {
        SelectionKey selectionKey = new SelectionKey(selectableEntity);

        if (_selectedGroups.ContainsKey(selectionKey))
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
            for (int j = 0; j < item.Value.Count; j++)
            {
                item.Value[j].OnDeselect();
            }
        }

        _selectedGroups.Clear();
        _highlightGroupIndex = 0;

        UIManager.Instance.UpdateHighlightGroup(_highlightGroupIndex);
        UIManager.Instance.UpdateSelectedGroups(_selectedGroups.ToArray());
    }
    #endregion
    #endregion
}
