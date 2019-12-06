using Lortedo.Utilities.Inspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Change cursor aspect in fonction of mosue over game object.
/// </summary>
public class CursorAspectManager : MonoBehaviour
{
    public enum CursorState
    {
        Default = 0,
        OverEntity = 1, // over entity without selection
        OrderMove = 2,
        OrderAttack = 3
    }

    #region Fields
    [SerializeField] CursorMode _cursorMode = CursorMode.Auto;
    [SerializeField, EnumNamedArray(typeof(CursorState))] private Texture2D[] _cursorTextures;

    private CursorState _cursorState = CursorState.Default;
    #endregion

    #region Methods
    #region MonoBehaviour Callbacks
    void Update()
    {
        UpdateCursorState();

        Cursor.SetCursor(_cursorTextures[(int)_cursorState], Vector2.zero, _cursorMode);
    }

    void OnValidate()
    {
        Array.Resize(ref _cursorTextures, Enum.GetValues(typeof(CursorState)).Length);
    }
    #endregion

    void UpdateCursorState()
    {
        if (SelectionManager.Instance.IsPlayerSelection)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Entity")))
            {
                if (hit.transform.GetComponent<Unit>().Team != Team.Sparta)
                {
                    _cursorState = CursorState.OrderAttack;
                }
                else
                {
                    _cursorState = CursorState.OverEntity;
                }
            }
            else if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Terrain")))
            {
                _cursorState = CursorState.OrderMove;
            }
            else
            {
                _cursorState = CursorState.Default;
            }
        }
        else
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Entity")))
            {
                _cursorState = CursorState.OverEntity;
            }
            else
            {
                _cursorState = CursorState.Default;
            }
        }
    }
    #endregion
}
