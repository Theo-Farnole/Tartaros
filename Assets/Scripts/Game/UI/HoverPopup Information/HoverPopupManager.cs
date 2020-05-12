using Lortedo.Utilities.Pattern;
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
        [SerializeField] private TextMeshProUGUI _titleText;
        [SerializeField] private TextMeshProUGUI _resourcesText;
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
            _titleText.text = hoverPopupData.Title;
            _descriptionText.text = hoverPopupData.Description;

            // add hotkey
            if (hoverPopupData.HotkeyEnabled)
                _titleText.text += " (" + hoverPopupData.Hotkey + ")";

            // resources
            if (hoverPopupData.ResourcesEnabled)
            {
                _resourcesText.gameObject.SetActive(true);
                _resourcesText.text = hoverPopupData.Resources.ToString();
            }
            else
            {
                _resourcesText.gameObject.SetActive(false);
            }

            // creation time
            if (hoverPopupData.CreationTimeEnabled)
            {
                _creationTimeText.gameObject.SetActive(true);
                _creationTimeText.text = hoverPopupData.CreationTime.ToString();
            }
            else
            {
                _creationTimeText.gameObject.SetActive(false);
            }
        }

        private void UpdatePosition(RectTransform hoverDisplayPopup)
        {
            _hoverPopup.position = hoverDisplayPopup.position
                + Vector3.up * hoverDisplayPopup.rect.height / 2
                + Vector3.up * _hoverPopup.rect.height / 2
                + Vector3.up * _heightMargin;
        }
    }
}
