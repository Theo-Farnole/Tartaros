using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionRect : MonoBehaviour
{
    #region Fields
    private bool _isSelecting = false;
    private Vector3 _originPositionRect;
    #endregion

    #region Properties
    public bool IsSelecting { get => _isSelecting; }
    #endregion

    #region Methods
    #region MonoBehaviour callbacks
    void LateUpdate()
    {
        ManageSelectionInput();
    }

    void OnGUI()
    {
        DrawSelectionRect();
    }
    #endregion

    #region Managers
    void ManageSelectionInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _originPositionRect = Input.mousePosition;
        }

        if (Input.GetMouseButton(0))
        {
            ManageSelectionRectActivation();
        }

        if (Input.GetMouseButtonUp(0) && _isSelecting)
        {
            AddEntitiesInRect();
            _isSelecting = false;
        }
    }

    void ManageSelectionRectActivation()
    {
        if (_isSelecting)
            return;

        if (Vector3.Distance(Input.mousePosition, _originPositionRect) >= 50)
        {
            _isSelecting = true;
            
            if (Input.GetKey(KeyCode.LeftShift) == false)
            {
                SelectionManager.Instance.ClearSelection();
            }
        }
    }
    #endregion

    #region SelectionRect drawer
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
    #endregion

    #region On SelectionRect releasing
    void AddEntitiesInRect()
    {
        var selectableEntities = FindObjectsOfType<SelectableEntity>();
        var camera = Camera.main;
        var viewportBounds = Utils.GetViewportBounds(camera, _originPositionRect, Input.mousePosition);

        for (int i = 0; i < selectableEntities.Length; i++)
        {
            if (IsWithinSelectionBounds(camera, viewportBounds, selectableEntities[i].gameObject))
            {
                SelectionManager.Instance.AddEntity(selectableEntities[i]);
            }
        }
    }

    bool IsWithinSelectionBounds(Camera camera, Bounds viewportBounds, GameObject gameObject)
    {
        if (!_isSelecting)
            return false;

        return viewportBounds.Contains(camera.WorldToViewportPoint(gameObject.transform.position));
    }
    #endregion
    #endregion
}
