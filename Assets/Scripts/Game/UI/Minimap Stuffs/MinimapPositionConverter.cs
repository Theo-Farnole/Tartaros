using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.UI
{
    public class MinimapPositionConverter : MonoBehaviour
    {
        [SerializeField] private Camera _miniMapCamera; // used to get minimap size
        [SerializeField] private RectTransform _minimapRoot; // used to get minimap UI width

        public void Initialize(Camera miniMapCamera, RectTransform minimapRoot)
        {
            _miniMapCamera = miniMapCamera;
            _minimapRoot = minimapRoot;
        }

        /// <summary>
        /// Returns a Vector3 as a percent. If in bounds, returns between 0.0f and 1.0f.
        /// </summary>
        public Vector3 WorldPositionToMinimapRelative(Vector3 position)
        {
            Bounds cameraBounds = new Bounds(_miniMapCamera.transform.position, _miniMapCamera.orthographicSize * 2 * Vector3.one);

            Vector3 min = cameraBounds.min;
            Vector3 max = cameraBounds.max;

            Vector3 relativePosition = (position - min);
            Vector3 relativeSize = (max - min);

            // operator Vector3 / Vector3 doesn't exist            
            return new Vector3(relativePosition.x / relativeSize.x, relativePosition.z / relativeSize.z);
        }

        public Vector3 RelativePositionToAbsolutePositionMinimap(Vector3 relative)
        {
            Vector3 output = new Vector3(relative.x * _minimapRoot.rect.width, relative.y * _minimapRoot.rect.height);

            return output;
        }

        public Vector3 WorldPositionToMinimap(Vector3 position)
            => RelativePositionToAbsolutePositionMinimap(WorldPositionToMinimap(position));


        public Vector3 ScreenToWorldPoint(Vector2 minimapPosition)
        {
            if (_minimapRoot.anchorMin != Vector2.zero || _minimapRoot.anchorMax != Vector2.zero)
            {
                throw new System.NotImplementedException("Calculation with anchors !=  Vector2 is not implemented.");
            }

            Vector2 min = _minimapRoot.rect.min;
            Vector2 max = _minimapRoot.rect.max;

            // between .0f & 1.0f
            Vector2 relativePosition = (minimapPosition - min) / _minimapRoot.rect.width; 

            // between -1.0f & 1.0f
            relativePosition = new Vector2(relativePosition.x - 0.5f, relativePosition.y - 0.5f) * 2;

            Vector2 cameraOffset = relativePosition * _miniMapCamera.orthographicSize;
            Vector3 worldPosition = _miniMapCamera.transform.position + cameraOffset.ToXZ();

            return worldPosition;
        }
    }
}
