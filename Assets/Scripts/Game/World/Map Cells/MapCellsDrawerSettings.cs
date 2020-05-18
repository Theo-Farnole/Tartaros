using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
public static class MapCellsDrawerSettings
{

    private static readonly string keyMaxVisibleCell = "MaxVisibleCell";
    private static readonly string keyCellOpacity = "MapCell_CellOpacity";

    public static int Gizmos_MaxVisibleCells
    {
        get => EditorPrefs.GetInt(keyMaxVisibleCell, 300);
        private set => EditorPrefs.SetInt(keyMaxVisibleCell, value);
    }

    public static float Gizmos_CellOpacity
    {
        get => EditorPrefs.GetFloat(keyCellOpacity, 0.8f);
        private set => EditorPrefs.SetFloat(keyCellOpacity, value);
    }


    [SettingsProvider()]
    public static SettingsProvider PreferencesGUI()
    {
        // First parameter is the path in the Settings window.
        // Second parameter is the scope of this setting: it only appears in the Settings window for the Project scope.
        var provider = new SettingsProvider("Project/Tartaros settings", SettingsScope.User)
        {
            label = "Tartaros settings",

            guiHandler = (string str) => OnGUI(),

            // Populate the search keywords to enable smart search filtering and label highlighting:
            keywords = new HashSet<string>(new[] { "Tartaros", "Map Cells" })
        };

        return provider;
    }

    private static void OnGUI()
    {
        GUILayout.BeginHorizontal();

        EditorGUILayout.PrefixLabel("Max Visible Cells");                        
        Gizmos_MaxVisibleCells = EditorGUILayout.IntSlider(Gizmos_MaxVisibleCells, 10, 15000);

        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();

        EditorGUILayout.PrefixLabel("Cell Opacity");
        Gizmos_CellOpacity = EditorGUILayout.Slider(Gizmos_CellOpacity, 0, 1);

        GUILayout.EndHorizontal();
    }
}
#endif