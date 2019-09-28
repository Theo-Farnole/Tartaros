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
    private Dictionary<SelectionKey, List<SelectableEntity>> _selectedGroups = new Dictionary<SelectionKey, List<SelectableEntity>>();
    private bool _isSelecting = false;
    private Vector3 _originPositionRect;
    #endregion

    #region Properties
    public KeyValuePair<SelectionKey, List<SelectableEntity>>[] SelectedGroups { get => _selectedGroups.ToArray(); }
    public KeyValuePair<SelectionKey, List<SelectableEntity>>[] SpartanGroups { get => (from x in _selectedGroups where x.Key.owner == Owner.Sparta select x).ToArray(); }
    public 
    #endregion

    #region Methods
    #region MonoBehaviour Callback
    void Update()
    {
        ManageSelectionInput();
        ManageCommandsExecuter();
    }

    void OnGUI()
    {
        DrawSelectionRect();
    }
    #endregion

    #region Selection Rect
    /// <summary>
    /// Must be called in OnGUI()
    /// </summary>
    void DrawSelectionRect()
    {
        if (_isSelecting)
        {
            // Create a rect from both mouse positions
            Rect rect = Utils.GetScreenRect(_originPositionRect, Input.mousePosition);
            Utils.DrawScreenRect(rect, new Color(0.8f, 0.8f, 0.95f, 0.25f));
            Utils.DrawScreenRectBorder(rect, 2, new Color(0.8f, 0.8f, 0.95f));
        }
    }

    /// <summary>
    /// Must be called in Update()
    /// </summary>
    void ManageSelectionInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (Input.GetKey(KeyCode.LeftShift) == false)
            {
                ClearSelection();
            }

            _isSelecting = true;
            _originPositionRect = Input.mousePosition;
        }

        if (Input.GetMouseButtonUp(0))
        {
            var selectableEntities = FindObjectsOfType<SelectableEntity>();

            for (int i = 0; i < selectableEntities.Length; i++)
            {
                if (IsWithinSelectionBounds(selectableEntities[i].gameObject))
                {
                    AddEntity(selectableEntities[i]);
                }
            }

            _isSelecting = false;
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ClearSelection();
        }
    }

    bool IsWithinSelectionBounds(GameObject gameObject)
    {
        if (!_isSelecting)
            return false;

        var camera = Camera.main;
        var viewportBounds = Utils.GetViewportBounds(camera, _originPositionRect, Input.mousePosition);

        return viewportBounds.Contains(camera.WorldToViewportPoint(gameObject.transform.position));
    }

    void ClearSelection()
    {
        foreach (var item in _selectedGroups)
        {
            for (int j = 0; j < item.Value.Count; j++)
            {
                item.Value[j].OnDeselect();
            }
        }

        _selectedGroups.Clear();
        UIManager.Instance.UpdateSelectedGroups(_selectedGroups.ToArray());
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

    #region Selected groups Modifier
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
        if (_selectedGroups[selectionKey].Count == 0) _selectedGroups.Remove(selectionKey);

        UIManager.Instance.UpdateSelectedGroups(_selectedGroups.ToArray());
    }
    #endregion
    #endregion
}
