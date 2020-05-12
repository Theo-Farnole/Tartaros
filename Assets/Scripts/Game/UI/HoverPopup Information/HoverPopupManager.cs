﻿using Lortedo.Utilities.Pattern;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Game.HoverPopup
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
        [SerializeField] private TextMeshProUGUI _goldResourcesText;
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
            _descriptionText.text = hoverPopupData.Description;
        }

        void SetTitle(HoverPopupData hoverPopupData)
        {
            _titleText.text = hoverPopupData.Title;
        }

        void SetHotkey(HoverPopupData hoverPopupData)
        {
            if (hoverPopupData.HotkeyEnabled)
                _titleText.text += " (" + hoverPopupData.Hotkey + ")";
        }

        void SetTimeCreationContent(HoverPopupData hoverPopupData)
        {            
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
            if (hoverPopupData.ResourcesEnabled)
            {
                _resourcesWrapper.gameObject.SetActive(true);

                _foodResourcesText.text = hoverPopupData.Resources.food.ToString();
                _woodResourcesText.text = hoverPopupData.Resources.wood.ToString();
                _goldResourcesText.text = hoverPopupData.Resources.gold.ToString();
            }
            else
            {
                _resourcesWrapper.gameObject.SetActive(false);
            }
        }

        private void UpdatePosition(RectTransform hoverDisplayPopup)
        {
            _hoverPopup.position = hoverDisplayPopup.position
                + Vector3.up * _hoverPopup.rect.height / 4; // center vertically
        }
    }
}