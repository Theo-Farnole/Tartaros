using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(UIManager))]
class UIManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Update Construction Buttons"))
        {
            ((UIManager)target).PanelConstruction.UpdateConstructionButtons();
        }
    }
}