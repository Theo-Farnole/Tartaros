using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.MapCellEditor.Editor
{
    public class Shortcut_CellBrushType
    {
        private MapCellsEditorWindow _window;

        public Shortcut_CellBrushType(MapCellsEditorWindow window)
        {
            _window = window;
        }

        public void ProcessEvent(Event e)
        {
            if (e.type == EventType.KeyDown && e.shift)
            {
                if (e.keyCode == KeyCode.Alpha1) _window.SetBrushCellType(CellType.Walkable);
                if (e.keyCode == KeyCode.Alpha2) _window.SetBrushCellType(CellType.Sea);
                if (e.keyCode == KeyCode.Alpha3) _window.SetBrushCellType(CellType.Mountains);
                if (e.keyCode == KeyCode.Alpha4) _window.SetBrushCellType(CellType.Farm_LowFertility);
                if (e.keyCode == KeyCode.Alpha5) _window.SetBrushCellType(CellType.Farm_MedFertility);
                if (e.keyCode == KeyCode.Alpha6) _window.SetBrushCellType(CellType.Farm_HighFertility);
                if (e.keyCode == KeyCode.Alpha7) _window.SetBrushCellType(CellType.Stone);
                if (e.keyCode == KeyCode.Alpha8) _window.SetBrushCellType(CellType.Forest);

                e.Use();
            }
        }
    }
}
