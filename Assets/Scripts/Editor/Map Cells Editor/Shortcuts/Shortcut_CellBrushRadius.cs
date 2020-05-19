using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Game.MapCellEditor.Editor
{
    public class Shortcut_CellBrushRadius
    {
        private static readonly float pixelToReach1Unit = 25;

        private KeyCode _keyCode = KeyCode.B;
        private CellBrush _cellBrush;

        private float _radiusOnStart;
        private bool _shortcutPressed = false;
        private float _mousePositionStartX;

        public Shortcut_CellBrushRadius(CellBrush cellBrush, KeyCode keyCode)
        {
            _cellBrush = cellBrush;
            _keyCode = keyCode;
        }

        public void ProcessEvent(Event e)
        {
            if (e.type == EventType.KeyDown && e.keyCode == _keyCode && !_shortcutPressed)
            {
                _shortcutPressed = true;
                _cellBrush.Lock();

                _radiusOnStart = _cellBrush.Radius;
                _mousePositionStartX = e.mousePosition.x + (_cellBrush.Radius * pixelToReach1Unit);
            }

            if (e.type == EventType.KeyUp && e.keyCode == _keyCode && _shortcutPressed)
            {
                _cellBrush.Unlock();
                _shortcutPressed = false;                
            }

            if (_shortcutPressed)
            {
                float distance = Mathf.Abs(e.mousePosition.x - _mousePositionStartX);
                float newRadius = (distance / pixelToReach1Unit);

                _cellBrush.Radius = newRadius;
            }
        }
    }
}
