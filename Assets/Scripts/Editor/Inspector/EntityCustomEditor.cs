using Game.Entities;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Game.Entities.Editor
{
    [CustomEditor(typeof(Entity))]
    public class EntityCustomEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            DrawUnitAction();
        }

        private void DrawUnitAction()
        {
            // get action
            Entity unit = (Entity)target;
            string action = unit.CurrentAction != null ? unit.CurrentAction.ToString() : "NONE"; // display action or "NONE"

            GUILayout.Space(EditorGUIUtility.singleLineHeight);

            // display it
            EditorGUILayout.BeginHorizontal();

            GUILayout.Label("Current action is ");
            GUILayout.Label(action);

            EditorGUILayout.EndHorizontal();
        }
    }
}
