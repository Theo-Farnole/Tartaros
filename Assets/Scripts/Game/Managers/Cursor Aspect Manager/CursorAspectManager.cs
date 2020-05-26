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
        OverEnemy = 4,
        OverAlly_WithSelection = 5
    }

    #region Fields
    [SerializeField] CursorMode _cursorMode = CursorMode.Auto;
    [SerializeField] private Texture2D[] _cursorTextures;

    private CursorState _cursorState = CursorState.Default;

    // cache variable
    private Camera _mainCamera;
    private int _layerMaskTerrain;
    private int _layerMaskEntity;
    #endregion

    #region Properties
    public CursorState CurrentCursorState { get => _cursorState;
        private set
        {
            var oldValue = _cursorState;
            _cursorState = value;

            if (oldValue != _cursorState)
            {
                SetCursor();
            }
        }
    }
    #endregion

    #region Methods
    #region MonoBehaviour Callbacks
    void Start()
    {
        _mainCamera = Camera.main;

        _layerMaskEntity = LayerMask.NameToLayer("Entity");
        _layerMaskTerrain = LayerMask.NameToLayer("Terrain");
    }

    void Update()
    {
        UpdateCursorState();        
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
            CurrentCursorState = SecondClickListener.Instance.CursorOverride;
            return;
        }

        Ray ray = _mainCamera.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _layerMaskEntity))
        {
            Entity clickedEntity = hit.transform.GetComponent<Entity>();

            Assert.IsNotNull(clickedEntity, string.Format("Cursor Aspect Manager : {0} is on layer 'Entity' doesn't have a 'Entity' component. Maybe, you want to remove colliders on model.", hit.transform.name));

            // order cursor
            if (SelectionManager.Instance.HasSelection)
            {
                if (clickedEntity.Team == Team.Player)                
                    CurrentCursorState = CursorState.OverAlly_WithSelection;
                else if (clickedEntity.Team != Team.Player)
                    CurrentCursorState = CursorState.OrderAttack;         
            }
            // over cursor
            else
            {
                if (clickedEntity.Team == Team.Player)
                    CurrentCursorState = CursorState.OverAlly;

                else
                    CurrentCursorState = CursorState.OverEnemy;
            }

            Debug.LogFormat("Over {0}", transform.name);
        }
        else if (Physics.Raycast(ray, out hit, Mathf.Infinity, _layerMaskTerrain))
        {
            if (SelectionManager.Instance.HasSelection)
                CurrentCursorState = CursorState.OrderMove;

            else
                CurrentCursorState = CursorState.Default;
        }
        else
        {
            CurrentCursorState = CursorState.Default;
        }
    }

    private void SetCursor()
    {
        Cursor.SetCursor(_cursorTextures[(int)CurrentCursorState], Vector2.zero, _cursorMode);
    }
    #endregion
}
