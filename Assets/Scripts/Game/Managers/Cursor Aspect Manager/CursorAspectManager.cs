using Game.Selection;
using Lortedo.Utilities.Inspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// Change cursor aspect in fonction of mosue over game object.
/// </summary>
public class CursorAspectManager : MonoBehaviour
{
    public enum CursorState
    {
        Default = 0,
        OverAlly = 1, // over entity without selection
        OrderMove = 2,
        OrderAttack = 3,
        OverEnemy = 4
    }

    #region Fields
    [SerializeField] CursorMode _cursorMode = CursorMode.Auto;
    [SerializeField] private Texture2D[] _cursorTextures;

    private CursorState _cursorState = CursorState.Default;
    private Camera _mainCamera;
    #endregion

    #region Methods
    #region MonoBehaviour Callbacks
    void Start()
    {
        _mainCamera = Camera.main;
    }

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
        if (SecondClickListener.Instance.ListenToClick)
        {
            _cursorState = SecondClickListener.Instance.CursorOverride;
            return;
        }

        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Entity")))
        {
            Assert.IsNotNull(GetComponent<Entity>(), 
                string.Format("Cursor Aspect Manager : {0} is on layer 'Entity' doesn't have a 'Entity' component. Maybe, you want to remove colliders on model.", name));

            // order cursor
            if (SelectionManager.Instance.HasSelection)
            {
                if (hit.transform.GetComponent<Entity>().Team == Team.Sparta)                
                    _cursorState = CursorState.OrderMove;
                
                else
                    _cursorState = CursorState.OrderAttack;
            }
            // over cursor
            else
            {
                if (hit.transform.GetComponent<Entity>().Team == Team.Sparta)
                    _cursorState = CursorState.OverAlly;

                else
                    _cursorState = CursorState.OverEnemy;
            }
        }
        else if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Terrain")))
        {
            if (SelectionManager.Instance.HasSelection)
                _cursorState = CursorState.OrderMove;

            else
                _cursorState = CursorState.Default;
        }
        else
        {
            _cursorState = CursorState.Default;
        }
    }
    #endregion
}
