using Lortedo.Utilities.Pattern;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Assertions;

namespace Game.Inputs.Editor
{
    [CustomEditor(typeof(HotkeyActionListener))]
    public class HotkeyActionListenerEditor : UnityEditor.Editor
    {
        private const BindingFlags commandsBindingFlags = BindingFlags.NonPublic | BindingFlags.Instance;

        private HotkeyActionListener HotkeyActionListener => target as HotkeyActionListener;

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            DrawCommandsContent();
        }

        void DrawCommandsContent()
        {
            // prevent commands content non playing
            if (!Application.isPlaying)
                return;

            EditorGUILayout.LabelField("Hotkeys", EditorStyles.boldLabel);

            System.Type type = typeof(HotkeyActionListener);
            FieldInfo field = type.GetField("_commands", commandsBindingFlags);

            var commands = field.GetValue(HotkeyActionListener) as Dictionary<KeyCode, Action>;

            Assert.IsNotNull(commands, "Can't find field _commands in HotkeyActionListner");

            if (commands.Count == 0)
            {
                GUILayout.Label("No hotkey listened.");
            }
            else
            {
                foreach (var command in commands)
                {
                    string label = command.Key + " => " + command.Value;

                    GUILayout.Label(label);
                }
            }
        }
    }
}
