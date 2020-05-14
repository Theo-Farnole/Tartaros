using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace LeonidasLegacy.MapCellEditor.Editor
{
    public class MapCellsEditorWindow : EditorWindow
    {
        private static readonly string debugLogHeader = "<color=cyan>Map Cells editor</color> : ";
        private static readonly int cellPresetSize = 50;
        private static readonly float spacing = 2;
        private static readonly Color selectedBackgroundColor = "30336b".HexToColor();

        private CellBrush _cellBrush;

        private Vector2 _scrollPosition;

        [MenuItem("Tartaros/Open map editor")]
        public static void OpenMapEditor()
        {
            // Get existing open window or if none, make a new one:
            MapCellsEditorWindow window = GetWindow<MapCellsEditorWindow>();
            window.Show();
        }

        void OnGUI()
        {
            OnGUI_DrawCellsPreset();
        }

        void OnEnable()
        {
            CreateBrush();
        }

        private void CreateBrush()
        {
            if (_cellBrush != null)
            {
                Debug.LogWarningFormat("Map Cells : Can't create brush because it already exists.");
                return;
            }

            MapCells mapCells = GetFirstMapCells();
            _cellBrush = new CellBrush(mapCells, CellType.Walkable);
        }

        void OnDisable()
        {

        }

        public MapCells GetFirstMapCells()
        {
            string[] mapCellsGUID = AssetDatabase.FindAssets("t:" + typeof(MapCells));

            if (mapCellsGUID.Length == 0)
            {
                Debug.LogErrorFormat("Map Cells : No MapCells found in project.");
                return null;
            }

            string firstMapCellGUID = mapCellsGUID[0];
            string firstCellPath = AssetDatabase.GUIDToAssetPath(firstMapCellGUID);
            MapCells mapCells = AssetDatabase.LoadAssetAtPath<MapCells>(firstCellPath);

            if (mapCellsGUID.Length > 1)
            {
                Debug.LogWarningFormat("Map Cells : " + "There is more than one MapCells. We are editing " + mapCells.name + ".");
            }

            return mapCells;
        }

        #region GUI Methods
        void OnGUI_DrawCellsPreset()
        {
            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition);
            GUILayout.Space(spacing);

            GUILayout.BeginVertical();

            foreach (CellType cellType in Enum.GetValues(typeof(CellType)))
            {
                OnGUI_DrawCellPreset(cellType);
                GUILayout.Space(spacing);
            }

            GUILayout.EndVertical();
            GUILayout.EndScrollView();
        }

        void OnGUI_DrawCellPreset(CellType cellType)
        {
            GUIStyle cellBackgroundStyle = new GUIStyle();

            if (_cellBrush != null && _cellBrush.CellType == cellType)
                cellBackgroundStyle.normal.background = GUIHelper.GenerateTexture2D(selectedBackgroundColor);

            GUILayout.BeginHorizontal(cellBackgroundStyle, GUILayout.Height(cellPresetSize));

            GUI_DrawCell_Color(cellType);
            GUI_DrawCell_Name(cellType);

            GUILayout.EndHorizontal();
        }

        private void GUI_DrawCell_Name(CellType cellType)
        {
            // start of vertical center
            GUILayout.BeginVertical();
            GUILayout.FlexibleSpace();

            // draw name of cell
            GUILayout.Label(cellType.ToString());

            Rect labelRect = GUILayoutUtility.GetLastRect();
            labelRect.height = cellPresetSize; // height of labelRect as cell size
            ProcessPresetEventClick(labelRect, cellType);

            // end of vertical center
            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
        }

        private void GUI_DrawCell_Color(CellType cellType)
        {
            Texture2D presetTexture = GUIHelper.GenerateTexture2D(cellType.GetColor());

            GUIStyle guiStyle = new GUIStyle();
            guiStyle.normal.background = presetTexture;

            // draw color of cell
            GUILayout.Box(presetTexture, guiStyle, GUILayout.Width(cellPresetSize), GUILayout.Height(cellPresetSize));

            Rect boxRect = GUILayoutUtility.GetLastRect();
            ProcessPresetEventClick(boxRect, cellType);
        }
        #endregion

        #region Process events
        void ProcessPresetEventClick(Rect presetRect, CellType cellType)
        {
            if (DoUserClickOnRect(presetRect))
            {
                _cellBrush.CellType = cellType;
                GUI.changed = true;
            }
        }

        private static bool DoUserClickOnRect(Rect presetRect)
        {
            return Event.current != null && Event.current.isMouse && presetRect.Contains(Event.current.mousePosition);
        }
        #endregion
    }
}
