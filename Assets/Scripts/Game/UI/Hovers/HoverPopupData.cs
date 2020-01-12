using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI.Game.HoverPopup
{
    [System.Serializable]
    public class HoverPopupInfo
    {
        [SerializeField] private string _title;
        [SerializeField, TextArea(3, 10)] private string _description;

        [Header("Optional settings")]

        [SerializeField] private bool _hotkeyEnabled;
        [SerializeField] private KeyCode _hotkey;

        [Space]
        [SerializeField] private bool _creationTimeEnabled;
        [SerializeField] private float _creationTime;

        [Space]
        [SerializeField] private bool _resourcesEnabled;
        [SerializeField] private ResourcesWrapper _resources;

        #region Properties
        public string Title { get => _title; }
        public string Description { get => _description;  }

        public bool HotkeyEnabled { get => _hotkeyEnabled; }
        public KeyCode Hotkey { get => _hotkey; }

        public bool ResourcesEnabled { get => _resourcesEnabled; }
        public ResourcesWrapper Resources { get => _resources; }

        public bool CreationTimeEnabled { get => _creationTimeEnabled; }
        public float CreationTime { get => _creationTime; }
        #endregion
    }
}
