using Lortedo.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.Selection
{

    /// <summary>
    /// It handle the input, the drawing and adding the entities to the selection manager.
    /// </summary>
    public class SelectionRectangle : MonoBehaviour
    {
        #region Fields
        public readonly static int pixelsToStartSelection = 50;

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
        /// <summary>
        /// Need to be set in LateUpdate. Else, SelectionManager while override selection on MouseButtonUp.
        /// </summary>
        void ManageSelectionInput()
        {
            // if pointer over UI, don't start selection
            if (EventSystem.current.IsPointerOverGameObject(-1) == false)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    _originPositionRect = Input.mousePosition;
                }

                if (Input.GetMouseButton(0))
                {
                    CheckForSelectionRectStart();
                }
            }

            if (Input.GetMouseButtonUp(0) && _isSelecting)
            {
                AddEntitiesInSelectionRect();
                _isSelecting = false;
            }
        }

        /// <summary>
        /// If is mouse button down, check if the player is dragging
        /// </summary>
        void CheckForSelectionRectStart()
        {
            // if it selecting, don't check to start again
            if (_isSelecting)
                return;

            if (Vector3.Distance(Input.mousePosition, _originPositionRect) >= pixelsToStartSelection)
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
                Rect rect = GUIRectDrawer.GetScreenRect(_originPositionRect, Input.mousePosition);
                GUIRectDrawer.DrawScreenRect(rect, new Color(0.8f, 0.8f, 0.95f, 0.25f));
                GUIRectDrawer.DrawScreenRectBorder(rect, 2, new Color(0.8f, 0.8f, 0.95f));
            }
        }
        #endregion

        #region On SelectionRect Releasing
        void AddEntitiesInSelectionRect()
        {
            var unitsSelectable = FindObjectsOfType<EntitySelectable>();
            var camera = Camera.main;
            var viewportBounds = GUIRectDrawer.GetViewportBounds(camera, _originPositionRect, Input.mousePosition);

            for (int i = 0; i < unitsSelectable.Length; i++)
            {
                if (IsWithinSelectionBounds(camera, viewportBounds, unitsSelectable[i].gameObject))
                {
                    SelectionManager.Instance.AddEntity(unitsSelectable[i].GetComponent<Entity>());
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
}
