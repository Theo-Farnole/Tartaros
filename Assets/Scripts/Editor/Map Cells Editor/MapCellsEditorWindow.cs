using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace Game.MapCellEditor.Editor
{
    public class MapCellsEditorWindow : EditorWindow
    {
        #region Fields
        private static readonly string debugLogHeader = "<color=cyan>Map Cells editor</color> : ";
        private static readonly int cellButtonSize = 25;
        private static readonly float cellButtonSpacing = 2;
        private static readonly Color selectedBackgroundColor = "30336b".HexToColor();
        private static readonly int TAB_SIZE = 25;

        private CellBrush _currentCellBrush;
        private Shortcut_CellBrushType _shortcutCellBrushType;

        private Vector2 _scrollPosition;
        #endregion

        #region Methods
        [MenuItem("Tartaros/Open map editor")]
        public static void OpenMapEditor()
        {
            // Get existing open window or if none, make a new one:
            MapCellsEditorWindow window = GetWindow<MapCellsEditorWindow>();
            window.Show();
        }


        void OnEnable()
        {
            SceneView.duringSceneGui += OnSceneGUI;

            CreateBrush();
        }

        void OnDisable()
        {
            SceneView.duringSceneGui -= OnSceneGUI;            

            _currentCellBrush?.DisableBrush();
        }

        void OnSceneGUI(SceneView sceneView)
        {
            ProcessShorcutsEvents();
        }

        #region Editor Window Callbacks
        void OnGUI()
        {
            _scrollPosition = GUILayout.BeginScrollView(_scrollPosition);

            OnGUI_DrawCellsButtons();
            OnGUI_DrawBrushSettings();

            GUILayout.EndScrollView();            
        }

        private void ProcessShorcutsEvents()
        {
            if (_shortcutCellBrushType == null)
                _shortcutCellBrushType = new Shortcut_CellBrushType(this);

            _shortcutCellBrushType.ProcessEvent(Event.current);           
        }
        #endregion

        #region Brush creation
        private void CreateBrush()
        {
            if (_currentCellBrush != null)
            {
                Debug.LogWarningFormat("Map Cells : Can't create brush because it already exists.");
                return;
            }

            MapCells mapCells = GetFirstMapCells();
            _currentCellBrush = new CellBrush(mapCells, CellType.Walkable);
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
        #endregion

        #region GUI Methods
        #region GUI Brush Settings Drawer
        void OnGUI_DrawBrushSettings()
        {
            EditorGUILayout.LabelField("Brush Settings", EditorStyles.boldLabel);

            EditorGUI.indentLevel++;

            if (_currentCellBrush == null)
            {
                EditorGUILayout.LabelField("Please, click on a cell before modify the brush.");
            }
            else
            {
                EditorGUILayout.PrefixLabel("Brush Radius");
                _currentCellBrush.Radius = EditorGUILayout.Slider(_currentCellBrush.Radius, 0.1f, 10);
            }

            EditorGUI.indentLevel--;
        }
        #endregion

        #region GUI Buttons Draw
        void OnGUI_DrawCellsButtons()
        {
            EditorGUILayout.LabelField("Cell Selection", EditorStyles.boldLabel);
            EditorGUI.indentLevel++;

            GUILayout.Space(cellButtonSpacing);

            GUILayout.BeginVertical();

            foreach (CellType cellType in Enum.GetValues(typeof(CellType)))
            {
                OnGUI_DrawCellPreset(cellType);
                GUILayout.Space(cellButtonSpacing);
            }

            GUILayout.EndVertical();

            EditorGUI.indentLevel--;
        }

        void OnGUI_DrawCellPreset(CellType cellType)
        {
            GUIStyle cellBackgroundStyle = new GUIStyle();

            if (_currentCellBrush != null && _currentCellBrush.CellType == cellType)
                cellBackgroundStyle.normal.background = GUIHelper.GenerateTexture2D(selectedBackgroundColor);

            GUILayout.BeginHorizontal(cellBackgroundStyle, GUILayout.Height(cellButtonSize));


            GUILayout.Space(EditorGUI.indentLevel * TAB_SIZE);
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
            labelRect.height = cellButtonSize; // height of labelRect as cell size
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
            GUILayout.Box(presetTexture, guiStyle, GUILayout.Width(cellButtonSize), GUILayout.Height(cellButtonSize));

            Rect boxRect = GUILayoutUtility.GetLastRect();
            ProcessPresetEventClick(boxRect, cellType);
        }
        #endregion
        #endregion

        #region Process events
        void ProcessPresetEventClick(Rect presetRect, CellType cellType)
        {
            if (DoUserClickOnRect(presetRect))
            {
                SetBrushCellType(cellType);
                GUI.changed = true;
            }
        }

        public void SetBrushCellType(CellType cellType)
        {
            _currentCellBrush.CellType = cellType;
            this.Repaint();
        }

        private static bool DoUserClickOnRect(Rect presetRect)
        {
            return Event.current != null && Event.current.isMouse && presetRect.Contains(Event.current.mousePosition);
        }
        #endregion
        #endregion
    }
}
