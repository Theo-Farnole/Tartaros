using System.Collections;
using System.Collections.Generic;
using TMPro;
using Game.UI.HoverPopup;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;
using Game.Entities;

namespace Game.UI
{
    public class OrderButton : MonoBehaviour
    {
        #region Fields
        [SerializeField] private Button _button;
        [Space]
        [SerializeField] private Image _backgroundButton;
        [SerializeField] private TextMeshProUGUI _hotkey;
        [SerializeField] private HoverDisplayPopup _hoverDisplayPopup;

        private OrderContent _orderContent;
        #endregion

        #region Properties
        public Button Button { get => _button; }

        private HoverDisplayPopup HoverDisplayPopup
        {
            get
            {
                if (_hoverDisplayPopup == null)
                    _hoverDisplayPopup = GetComponent<HoverDisplayPopup>();

                return _hoverDisplayPopup;
            }
        }
        #endregion

        #region Methods
        #region Public Methods
        public void SetContent(OrderContent orderContent)
        {
            Assert.IsNotNull(orderContent);

            Assert.IsNotNull(_hotkey, "Please assign _hotkey in " + name);
            Assert.IsNotNull(_backgroundButton, "Please assign _backgroundButton in " + name);
            Assert.IsNotNull(_hoverDisplayPopup, "Please assign _hoverDisplayPopup in " + name);

            _orderContent = orderContent;

            _hotkey.text = orderContent.Hotkey.ToString();
            _backgroundButton.sprite = orderContent.Portrait;
            HoverDisplayPopup.HoverPopupData = orderContent.HoverPopupData;

            _button.onClick.RemoveAllListeners();

            if (orderContent.OnClick != null)
            {
                _button.onClick.AddListener(() => orderContent.OnClick());
            }

            if (orderContent.Enabled)
            {
                EnableButtonInteraction();
            }
            else
            {
                DisableButtonInteraction();
            }
        }

        public void EnableButtonInteraction()
        {
            _button.interactable = true;
            _hotkey.enabled = true;

            _backgroundButton.color = Color.white;
        }

        public void DisableButtonInteraction()
        {
            // we check for null here because DisableButtonInteraction is called when application is closing.
            // To avoid errors logs for missing reference, we check for null.
            if (_button != null)
                _button.interactable = false;

            if (_hotkey != null)
                _hotkey.enabled = false;

            if (_backgroundButton != null)
                _backgroundButton.color = Color.grey;
        }
        #endregion

        #region Private Methods
        void UpdateCurrentButtonInteraction()
        {
            if (_orderContent == null)
                return;

            if (_orderContent.Enabled) EnableButtonInteraction();
            else DisableButtonInteraction();
        }
        #endregion
        #endregion
    }
}
