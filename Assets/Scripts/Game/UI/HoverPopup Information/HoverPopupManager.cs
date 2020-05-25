using Lortedo.Utilities.Pattern;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Game.UI.HoverPopup
{
    public class HoverPopupManager : Singleton<HoverPopupManager>
    {
        [SerializeField] private GameObject _hoverPopupCanvas;
        [SerializeField] private RectTransform _hoverPopup;
        [Space]
        [Header("WRAPPERS")]
        [SerializeField] private RectTransform _creationTimeWrapper;
        [SerializeField] private RectTransform _resourcesWrapper;
        [Header("TEXTS")]
        [SerializeField] private TextMeshProUGUI _titleText;
        [SerializeField] private TextMeshProUGUI _foodResourcesText;
        [SerializeField] private TextMeshProUGUI _woodResourcesText;
        [FormerlySerializedAs("_goldResourcesText")] [SerializeField] private TextMeshProUGUI _stoneResourcesText;
        [SerializeField] private TextMeshProUGUI _creationTimeText;
        [SerializeField] private TextMeshProUGUI _descriptionText;
        [Space]
        [SerializeField, Tooltip("In pixels")] private int _heightMargin = 25;

        void Start()
        {
            HidePopUp();    
        }

        public void DisplayHoverPopup(RectTransform hoverDisplayPopup, HoverPopupData hoverPopupData)
        {
            if (hoverPopupData == null)
            {
                Debug.LogErrorFormat("Can't display hover popup with null 'hoverPopupData'. Abortinging display of hover popup.");
                return;
            }

            _hoverPopupCanvas.SetActive(true);

            UpdateContent(hoverPopupData);
            UpdatePosition(hoverDisplayPopup);
        }

        public void HidePopUp()
        {
            _hoverPopupCanvas.SetActive(false);
        }

        private void UpdateContent(HoverPopupData hoverPopupData)
        {
            SetTitle(hoverPopupData);
            SetDescription(hoverPopupData);

            SetHotkey(hoverPopupData);
            SetResourceContent(hoverPopupData);
            SetTimeCreationContent(hoverPopupData);
        }

        void SetDescription(HoverPopupData hoverPopupData)
        {
            Assert.IsNotNull(hoverPopupData);

            _descriptionText.gameObject.SetActive(hoverPopupData.HasDescription);
            _descriptionText.text = hoverPopupData.Description;
        }

        void SetTitle(HoverPopupData hoverPopupData)
        {
            Assert.IsNotNull(hoverPopupData);

            _titleText.gameObject.SetActive(hoverPopupData.HasTitle);
            _titleText.text = hoverPopupData.Title;
        }

        void SetHotkey(HoverPopupData hoverPopupData)
        {
            Assert.IsNotNull(hoverPopupData);

            if (hoverPopupData.HotkeyEnabled)
                _titleText.text += " (" + hoverPopupData.Hotkey + ")";
        }

        void SetTimeCreationContent(HoverPopupData hoverPopupData)
        {
            Assert.IsNotNull(hoverPopupData);

            if (hoverPopupData.CreationTimeEnabled)
            {
                _creationTimeWrapper.gameObject.SetActive(true);
                _creationTimeText.text = hoverPopupData.CreationTime.ToString();
            }
            else
            {
                _creationTimeWrapper.gameObject.SetActive(false);
            }
        }

        void SetResourceContent(HoverPopupData hoverPopupData)
        {
            Assert.IsNotNull(hoverPopupData);

            if (hoverPopupData.ResourcesEnabled)
            {
                _resourcesWrapper.gameObject.SetActive(true);

                _foodResourcesText.text = hoverPopupData.Resources.food.ToString();
                _woodResourcesText.text = hoverPopupData.Resources.wood.ToString();
                _stoneResourcesText.text = hoverPopupData.Resources.stone.ToString();
            }
            else
            {
                _resourcesWrapper.gameObject.SetActive(false);
            }
        }

        private void UpdatePosition(RectTransform hoverDisplayPopup)
        {
            Assert.IsNotNull(hoverDisplayPopup, "The display popup passed in arg is null. Can't update popup position");

            _hoverPopup.position = hoverDisplayPopup.position
                + Vector3.up * _hoverPopup.rect.height / 4; // center vertically
        }
    }
}
