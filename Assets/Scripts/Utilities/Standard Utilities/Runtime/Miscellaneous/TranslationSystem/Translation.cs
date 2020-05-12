using UnityEngine;
using System.Collections.Generic;
using System;
using System.IO;

/// <author>
/// http://www.demonixis.net/ajout-du-multilingue-dans-votre-jeux-avec-unity-3d/
/// </author>

namespace Lortedo.Utilities
{
    public sealed class Translation : MonoBehaviour
    {
        public static readonly SystemLanguage[] Languages = { SystemLanguage.English, SystemLanguage.French };
        private static Dictionary<string, string> Translations = null;

        public static void ResetTranslations()
        {
            Translations = null;
        }

        private static void CheckInstance()
        {
            // It's already initialized.
            if (Translations != null)
                return;

            Translations = new Dictionary<string, string>();

            var lang = Application.systemLanguage;

            // Check if the current language is supported.
            // Otherwise use the first language as default.
            if (Array.IndexOf<SystemLanguage>(Languages, lang) == -1)
                lang = Languages[0];

            // Load and parse the translation file from the Resources folder.
            var data = Resources.Load<TextAsset>($"Translations/{lang}");

            if (data != null)
                ParseFile(data.text);
        }
        // Returns the translation for this key.
        public static string Get(string key)
        {
            CheckInstance();

            if (Translations.ContainsKey(key))
                return Translations[key];

#if UNITY_EDITOR
            Debug.Log($"The key {key} is missing");
#endif
            return key;
        }

        public static void ParseFile(string data)
        {
            using (var stream = new StringReader(data))
            {
                var line = stream.ReadLine();
                var temp = new string[2];
                var key = string.Empty;
                var value = string.Empty;
                while (line != null)
                {
                    if (line.StartsWith(";") || line.StartsWith("["))
                    {
                        line = stream.ReadLine();
                        continue;
                    }
                    temp = line.Split('=');
                    if (temp.Length == 2)
                    {
                        key = temp[0].Trim();
                        value = temp[1].Trim();
                        if (value == string.Empty)
                            continue;
                        if (Translations.ContainsKey(key))
                            Translations[key] = value;
                        else
                            Translations.Add(key, value);
                    }
                    line = stream.ReadLine();
                }
            }
        }
    }
}