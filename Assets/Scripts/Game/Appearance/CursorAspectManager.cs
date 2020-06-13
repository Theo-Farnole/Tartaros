namespace Game.Appearance.Cursor
{
    using Game.Entities;
    using Game.Inputs;
    using Game.Selection;
    using System;
    using UnityEngine;
    using UnityEngine.EventSystems;

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
        [SerializeField] private Vector2 _cursorOffset;

        private CursorState _cursorState = CursorState.Default;

        // cache variable
        private Camera _mainCamera;
        private int _layerMaskTerrain;
        private int _layerMaskEntity;
        #endregion

        #region Properties
        public CursorState CurrentCursorState
        {
            get => _cursorState;
            private set
            {
                var oldValue = _cursorState;
                _cursorState = value;

                if (oldValue != _cursorState)
                {
                    SetCursorSprite();
                }
            }
        }
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
        }

        void OnEnable()
        {
            SetCursorSprite();
        }

        void OnDisable()
        {
            SetCursorSpriteToDefault();
        }

        void OnMouseExit()
        {
            SetCursorSpriteToDefault();
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

            // is over UI
            if (EventSystem.current.IsPointerOverGameObject())
            {
                SetCursorSprite(CursorState.Default);
            }

            if (MouseInput.IsMouseOver(out RaycastHit hit, MouseLayer.Entity))
            {
                SetCursorState_OverEntity(hit.transform.GetComponent<Entity>());
            }
            else if (MouseInput.IsMouseOver(out hit, MouseLayer.Terrain))
            {
                SetCursorState_OverTerrain();
            }
            else
            {
                CurrentCursorState = CursorState.Default;
            }
        }

        private void SetCursorState_OverTerrain()
        {
            if (SelectionManager.Instance.HasSelection)
                CurrentCursorState = CursorState.OrderMove;
            else
                CurrentCursorState = CursorState.Default;
        }

        private void SetCursorState_OverEntity(Entity clickedEntity)
        {
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
        }

        private void SetCursorSprite()
        {
            Cursor.SetCursor(_cursorTextures[(int)CurrentCursorState], _cursorOffset, _cursorMode);
        }

        private void SetCursorSprite(CursorState cursorState)
        {
            Cursor.SetCursor(_cursorTextures[(int)cursorState], _cursorOffset, _cursorMode);
        }

        private void SetCursorSpriteToDefault()
        {
            Cursor.SetCursor(null, Vector2.zero, _cursorMode);
        }
        #endregion
    }
}
