using Lortedo.Utilities.Pattern;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecondClickListener : Singleton<SecondClickListener>
{
    private bool _listenToClick = false;
    private System.Action<RaycastHit> _actionOnClick = null;
    private CursorAspectManager.CursorState _cursorOverride;

    public bool ListenToClick { get => _listenToClick; }
    public CursorAspectManager.CursorState CursorOverride { get => _cursorOverride; }

    private int _raycastLayer;

    void Awake()
    {
        _raycastLayer = LayerMask.GetMask("Entity", "Terrain");
    }

    void LateUpdate()
    {
        if (_listenToClick && Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, _raycastLayer))
            {
                _actionOnClick(hit);
                StopListening();
            }
            else
            {
                StopListening();
            }
        }
    }

    public void ListenToAttack()
    {
        _listenToClick = true;
        _cursorOverride = CursorAspectManager.CursorState.OrderAttack;

        _actionOnClick = (RaycastHit hit) =>
        {
            Debug.Log("Second click");

            // do we hit terrain ?
            if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Terrain"))
            {
                Debug.Log("hit point = " + hit.point);
                SelectedGroupsActionsCaller.OrderMoveAggressively(hit.point);
            }
            else if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Entity"))
            {
                Entity clickedEntity = hit.transform.GetComponent<Entity>();

                // do we hit entity ?
                if (clickedEntity)
                {
                    SelectedGroupsActionsCaller.OrderAttackUnit(clickedEntity);
                }
            }
        };
    }

    public void ListenToMove()
    {
        _listenToClick = true;
        _cursorOverride = CursorAspectManager.CursorState.OrderMove;

        _actionOnClick = (RaycastHit hit) =>
        {
            Debug.Log("Move to pos");
            SelectedGroupsActionsCaller.OrderMoveToPosition(hit.point);
        };
    }

    public void StopListening()
    {
        _listenToClick = false;
        _actionOnClick = null;
    }
}
