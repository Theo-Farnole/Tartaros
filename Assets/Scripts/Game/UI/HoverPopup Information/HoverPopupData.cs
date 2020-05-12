using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI.Game.HoverPopup
{
    [CreateAssetMenu(menuName = "Tartaros/UI/Hover Popup")]
    public class HoverPopupData : ScriptableObject
    {
        [SerializeField] private string _title;
        [SerializeField, TextArea(3, 10)] private string _description;

        [Header("Optional settings")]

        [SerializeField] private bool _displayHotkey;
        [EnableIf(nameof(_displayHotkey))]
        [SerializeField] private KeyCode _hotkey;

        [Space]
        [SerializeField] private bool _displayCreationTime;
        [EnableIf(nameof(_displayCreationTime))]
        [SerializeField] private float _creationTime;

        [Space]
        [SerializeField] private bool _displayResources;
        [EnableIf(nameof(_displayResources))]
        [SerializeField] private ResourcesWrapper _resources;

        #region Properties
        public string Title { get => _title; }
        public string Description { get => _description;  }

        public bool HotkeyEnabled { get => _displayHotkey; }
        public KeyCode Hotkey { get => _hotkey; }

        public bool ResourcesEnabled { get => _displayResources; }
        public ResourcesWrapper Resources { get => _resources; }

        public bool CreationTimeEnabled { get => _displayCreationTime; }
        public float CreationTime { get => _creationTime; }
        #endregion
    }
}
