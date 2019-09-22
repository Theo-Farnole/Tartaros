using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class Group
{
    public EntityType type;

    public List<SelectableEntity> selectableEntities = new List<SelectableEntity>();

    public Group(EntityType type, SelectableEntity selectableEntity) : this(type, new List<SelectableEntity>() { selectableEntity })
    { }

    public Group(EntityType type, List<SelectableEntity> selectableEntities)
    {
        this.type = type;

        this.selectableEntities = selectableEntities;
    }
}

public class SelectionManager : Singleton<SelectionManager>
{
    #region Fields
    [SerializeField] private bool _displaySelectionOnUI = true;

    private List<Group> _selectedGroups = new List<Group>();
    private bool _isSelecting = false;
    private Vector3 _originPositionRect;
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

#if UNITY_EDITOR
        if (_displaySelectionOnUI == false)
            return;

        StringBuilder o = new StringBuilder();
        o.AppendLine("~ Selection ~");

        for (int i = 0; i < _selectedGroups.Count; i++)
        {
            o.AppendLine(_selectedGroups[i].selectableEntities.Count + " " + _selectedGroups[i].type);
        }

        Rect rect2 = new Rect(15, 45, 150, 400);
        GUI.Label(rect2, o.ToString());
#endif
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
                    AddEntity(selectableEntities[i].Type, selectableEntities[i]);
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
        for (int i = 0; i < _selectedGroups.Count; i++)
        {
            for (int j = 0; j < _selectedGroups[i].selectableEntities.Count; j++)
            {
                _selectedGroups[i].selectableEntities[j].OnDeselect();
            }
        }

        _selectedGroups.Clear();
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
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Entity")))
            {
                Transform target = hit.transform;

                for (int i = 0; i < _selectedGroups.Count; i++)
                {
                    for (int j = 0; j < _selectedGroups[i].selectableEntities.Count; j++)
                    {
                        _selectedGroups[i].selectableEntities[j].AttackEntity?.StartAttacking(target);
                    }
                }
            }
            else if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Grid")))
            {
                Vector3 target = hit.point;

                for (int i = 0; i < _selectedGroups.Count; i++)
                {
                    for (int j = 0; j < _selectedGroups[i].selectableEntities.Count; j++)
                    {
                        _selectedGroups[i].selectableEntities[j].AttackEntity?.StopAttack();
                        _selectedGroups[i].selectableEntities[j].MovableEntity?.GoTo(target);
                    }
                }
            }
        }
    }
    #endregion

    #region Selected groups Modifier
    public void AddEntity(EntityType type, SelectableEntity selectableEntity)
    {
        for (int i = 0; i < _selectedGroups.Count; i++)
        {
            // if find a selectedGroup of same unit
            if (_selectedGroups[i].type == type)
            {
                // if gameObject isn't already in gameObjects list
                if (_selectedGroups[i].selectableEntities.Contains(selectableEntity) == false)
                {
                    selectableEntity.OnSelected();
                    _selectedGroups[i].selectableEntities.Add(selectableEntity);
                    UpdatePortrait();
                    return;
                }
                else
                {
                    Debug.Log("Selection Manager # " + selectableEntity + " can't be added because is already selected");
                    return;
                }
            }
        }

        // group with same unit doesn't exist, so we're creating one
        selectableEntity.OnSelected();
        _selectedGroups.Add(new Group(type, selectableEntity));
        UpdatePortrait();
    }

    public void RemoveEntity(EntityType type, SelectableEntity selectableEntity)
    {
        for (int i = 0; i < _selectedGroups.Count; i++)
        {
            // if find a selectedGroup of same unit
            if (_selectedGroups[i].type == type)
            {
                selectableEntity.OnDeselect();
                _selectedGroups[i].selectableEntities.Remove(selectableEntity);

                if (_selectedGroups[i].selectableEntities.Count == 0)
                {
                    _selectedGroups.RemoveAt(i);
                }

                UpdatePortrait();
                return;
            }
        }
    }

    void UpdatePortrait()
    {
        if (_selectedGroups.Count > 0)
        {
            EntityType firstItemType = _selectedGroups[0].type;
            UIManager.Instance.SetSelectedPortrait(firstItemType);
        }
        else
        {
            UIManager.Instance.ResetSelectedPortrait();
        }
    }
    #endregion
    #endregion
}
