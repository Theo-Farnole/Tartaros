using Game.Entities;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Game.UI.HoverPopup
{
    [CreateAssetMenu(menuName = "Tartaros/UI/Hover Popup")]
    public class HoverPopupData : ScriptableObject
    {
        [SerializeField] private EntityData _entityData;

        [DisableIf(nameof(_entityData))]
        [SerializeField] private string _title;

        [SerializeField, TextArea(3, 10)] private string _description;

        [Header("Optional settings")]

        [DisableIf(nameof(_entityData))]
        [SerializeField] private bool _displayHotkey;

        [EnableIf(nameof(_displayHotkey))]
        [DisableIf(nameof(_entityData))]
        [SerializeField] private KeyCode _hotkey;

        [Space]
        [DisableIf(nameof(_entityData))]
        [SerializeField] private bool _displayCreationTime;

        [EnableIf(nameof(_displayCreationTime))]
        [DisableIf(nameof(_entityData))]
        [SerializeField] private float _creationTime;

        [DisableIf(nameof(_entityData))]
        [Space]
        [SerializeField] private bool _displayResources;

        [EnableIf(nameof(_displayResources))]
        [DisableIf(nameof(_entityData))]
        [SerializeField] private ResourcesWrapper _resources;

        #region Properties
        public bool HasTitle { get => _title != string.Empty; }
        public string Title { get => _title; }

        public bool HasDescription { get => _description != string.Empty; }
        public string Description { get => _description; }

        public bool HotkeyEnabled { get => _displayHotkey; }
        public KeyCode Hotkey { get => _hotkey; }

        public bool ResourcesEnabled { get => _displayResources; }
        public ResourcesWrapper Resources { get => _resources; }

        public bool CreationTimeEnabled { get => _displayCreationTime; }
        public float CreationTime { get => _creationTime; }
        #endregion

        void OnValidate()
        {
            UpdateContent();
        }

        /// <summary>
        /// Update content with _entityData field.
        /// </summary>
        public void UpdateContent()
        {
            if (_entityData != null)
                SetContent(_entityData);
        }

        private void SetContent(EntityData entityData)
        {
            Assert.IsNotNull(entityData);

            _title = entityData.EntityName;
            _displayHotkey = entityData.Hotkey != KeyCode.None;
            _hotkey = entityData.Hotkey;

            _displayResources = true;
            _resources = entityData.SpawningCost;

            _displayCreationTime = entityData.CreationDuration != 0;
            _creationTime = entityData.CreationDuration;
        }

        public override int GetHashCode()
        {
            var hashCode = -1301703148;
            hashCode += hashCode * -1521134295 + _title.GetHashCode();
            hashCode += hashCode * -1521134295 + _displayHotkey.GetHashCode();
            hashCode += hashCode * -1521134295 + _hotkey.GetHashCode();
            hashCode += hashCode * -1521134295 + _displayResources.GetHashCode();
            hashCode += hashCode * -1521134295 + _resources.GetHashCode();
            hashCode += hashCode * -1521134295 + _displayCreationTime.GetHashCode();
            hashCode += hashCode * -1521134295 + _creationTime.GetHashCode();
            return hashCode;
        }
    }
}
