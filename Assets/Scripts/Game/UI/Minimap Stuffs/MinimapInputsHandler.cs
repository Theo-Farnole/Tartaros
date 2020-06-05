using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Game.UI
{
    /// <summary>
    /// This script must be set on the minimap UI image.
    /// It's manage allow the player to move camera through the minimap.
    /// </summary>
    [RequireComponent(typeof(RawImage))]
    public class MinimapInputsHandler : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
    {
        [SerializeField] private Vector3 _cameraOffsetOnCameraDrag;

        bool _isMovingCamera = false;

        Transform _mainCamera;
        MinimapPositionConverter _minimapPositionConverter;

        void Start()
        {
            _mainCamera = Camera.main.transform;
            _minimapPositionConverter = FindObjectOfType<MinimapPositionConverter>();

            if (_minimapPositionConverter == null)
                Debug.LogError("Minimap Inputs Handler : Missing MinimapPositionConverter in scene. Can't handle inputs in minimap.");
        }

        void Update()
        {
            if (_isMovingCamera)
            {
                Vector3 worldPosition = _minimapPositionConverter.ScreenToWorldPoint(Input.mousePosition);
                _mainCamera.position = worldPosition + _cameraOffsetOnCameraDrag;
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _isMovingCamera = true;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _isMovingCamera = false;
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            // even if pointer down
            // set mouse as down
            _isMovingCamera = false;
        }
    }
}
