using Game.Entities;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EntityDetection))]
public class EntityDetectionCustomEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (Application.isPlaying)
            DrawNearestEntities();
    }

    void DrawNearestEntities()
    {
        EntityDetection ent = target as EntityDetection;

        if (!ent.Entity.Data.CanDetectEntities)
            return;

        EditorGUILayout.BeginHorizontal();
        {
            EditorGUILayout.PrefixLabel("Nearest Ally");
            EditorGUILayout.LabelField(ent.GetNearestAlly() ? ent.GetNearestAlly().name : "NONE");
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        {
            EditorGUILayout.PrefixLabel("Nearest Opponent");
            EditorGUILayout.LabelField(ent.GetNearestOpponent() ? ent.GetNearestOpponent().name : "NONE");
        }
        EditorGUILayout.EndHorizontal();
    }
}
