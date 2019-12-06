using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Unit))]
public class UnitCustomEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        DrawUnitAction();
    }

    private void DrawUnitAction()
    {
        // get action
        Unit unit = (Unit)target;
        string action = unit.CurrentAction != null ? unit.CurrentAction.ToString() : "NONE"; // display action or "NONE"

        GUILayout.Space(EditorGUIUtility.singleLineHeight);

        // display it
        EditorGUILayout.BeginHorizontal();

        GUILayout.Label("Current action is ");
        GUILayout.Label(action);

        EditorGUILayout.EndHorizontal();
    }
}
