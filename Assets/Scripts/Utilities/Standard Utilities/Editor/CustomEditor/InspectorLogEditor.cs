using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MonoBehaviour), true)]
public class InspectorLogEditor : Editor
{
    IInspectorLog _inspectorLog;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        DrawLog();
    }

    private void DrawLog()
    {
        if (_inspectorLog == null || string.IsNullOrEmpty(_inspectorLog.Log))
            return;

        GUILayout.Label("Runtime Log", EditorStyles.boldLabel);
        GUILayout.Label(_inspectorLog.Log);
    }

    void OnEnable()
    {
        GetInspectorLog();
    }

    void GetInspectorLog()
    {
        if (_inspectorLog == null)
        {
            _inspectorLog = (target as MonoBehaviour).GetComponent<IInspectorLog>();
        }
    }
}
