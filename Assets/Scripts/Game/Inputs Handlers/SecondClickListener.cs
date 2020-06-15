using Game.Appearance.Cursor;
using Game.Entities;
using Game.Selection;
using Lortedo.Utilities.Pattern;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Inputs
{
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

        void Start()
        {
            SelectionManager.OnSelectionUpdated += (SelectionManager.SelectionGroup[] selectedGroups, int highlightGroupIndex) => StopListening();
        }

        void LateUpdate()
        {
            if (_listenToClick && Input.GetMouseButtonDown(1))
            {
                // over UI
                if (EventSystem.current.IsPointerOverGameObject())
                {
                    StopListening();
                    return;
                }

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
                // do we hit terrain ?
                if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Terrain"))
                {
                    SelectedGroupsActionsCaller.OrderMoveAggressively(hit.point);
                }
                else if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Entity"))
                {
                    // do we hit entity ?
                    if (hit.transform.TryGetComponent(out Entity clickedEntity))
                    {
                        SelectedGroupsActionsCaller.OrderAttackUnit(clickedEntity);
                    }
                }
            };
        }

        public void ListenToMoveAggresively()
        {
            _listenToClick = true;
            _cursorOverride = CursorAspectManager.CursorState.OrderAttack;

            _actionOnClick = (RaycastHit hit) =>
            {
                // do we hit terrain ?
                if (hit.transform.gameObject.layer == LayerMask.NameToLayer("Terrain"))
                {
                    SelectedGroupsActionsCaller.OrderMoveAggressively(hit.point);
                }
            };
        }

        public void ListenToMove()
        {
            _listenToClick = true;
            _cursorOverride = CursorAspectManager.CursorState.OrderMove;

            _actionOnClick = (RaycastHit hit) =>
            {
                SelectedGroupsActionsCaller.OrderMoveToPosition(hit.point);
            };
        }

        public void ListenToPatrol()
        {
            _listenToClick = true;
            _cursorOverride = CursorAspectManager.CursorState.OverAlly;

            _actionOnClick = (RaycastHit hit) =>
            {
                SelectedGroupsActionsCaller.OrderPatrol(hit.point);
            };
        }

        public void StopListening()
        {
            _listenToClick = false;
            _actionOnClick = null;
        }
    }
}
