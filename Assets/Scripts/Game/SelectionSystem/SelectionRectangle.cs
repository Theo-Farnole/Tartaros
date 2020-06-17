namespace Game.Selection
{
    using Game.Entities;
    using Lortedo.Utilities;
    using System.Linq;
    using UnityEngine;
    using UnityEngine.EventSystems;

    /// <summary>
    /// It handle the input, the drawing and adding the entities to the selection manager.
    /// </summary>
    public class SelectionRectangle : MonoBehaviour
    {
        public enum State
        {
            None,
            TryStartSelection,
            InSelection
        }

        #region Fields
        [SerializeField] private int _pixelsToStartSelection = 50;

        private State _state;
        private Vector3 _originPositionRect;
        #endregion

        #region Properties
        public bool IsSelecting { get => _state == State.InSelection; }
        #endregion

        #region Methods
        #region MonoBehaviour callbacks
        void LateUpdate()
        {
            CurrentStateBehaviour();
        }

        void OnGUI()
        {
            DrawSelectionRect();
        }
        #endregion

        #region Managers
        void CurrentStateBehaviour()
        {
            switch (_state)
            {
                case State.None:
                    // mouse down and not on UI
                    if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
                    {
                        _originPositionRect = Input.mousePosition;
                        _state = State.TryStartSelection;
                    }
                    break;

                case State.TryStartSelection:
                    if (Input.GetMouseButton(0))
                    {
                        if (EventSystem.current.IsPointerOverGameObject())
                        {
                            _state = State.None;
                            break;
                        }

                        // we wait the player to drag, before start selection
                        if (Vector3.Distance(Input.mousePosition, _originPositionRect) >= _pixelsToStartSelection)
                        {
                            if (Input.GetKey(KeyCode.LeftShift) == false)
                            {
                                SelectionManager.Instance.ClearSelection();
                            }

                            _state = State.InSelection;
                        }
                    }

                    if (Input.GetMouseButtonUp(0))
                    {
                        _state = State.None;
                    }

                    break;

                case State.InSelection:
                    if (Input.GetMouseButtonUp(0))
                    {
                        AddEntitiesInSelectionRect();
                        _state = State.None;
                    }
                    break;

                default:
                    throw new System.NotImplementedException();
            }
        }


        #endregion

        #region SelectionRect drawer
        void DrawSelectionRect()
        {
            if (_state == State.InSelection)
            {
                // Create a rect from both mouse positions
                Rect rect = GUIRectDrawer.GetScreenRect(_originPositionRect, Input.mousePosition);
                GUIRectDrawer.DrawScreenRect(rect, new Color(0.8f, 0.8f, 0.95f, 0.25f));
                GUIRectDrawer.DrawScreenRectBorder(rect, 2, new Color(0.8f, 0.8f, 0.95f));
            }
        }
        #endregion

        void AddEntitiesInSelectionRect()
        {
            Entity[] entities = EntitiesGetter.GetUnitsInCameraViewport(_originPositionRect, Input.mousePosition);
            SelectionManager.Instance.AddEntities(entities);
        }
        #endregion
    }
}
