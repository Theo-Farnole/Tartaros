using System.Collections;
using System.Collections.Generic;
using TMPro;
using Game.UI.HoverPopup;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Assertions;

namespace Game.UI
{
    public class UI_OrderWrapper : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [Space]
        [SerializeField] private Image _backgroundButton;
        [SerializeField] private TextMeshProUGUI _hotkey;
        [SerializeField] private HoverDisplayPopup _hoverDisplayPopup;

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

        public void SetContent(OrderContent orderContent)
        {
            Assert.IsNotNull(orderContent);

            Assert.IsNotNull(_hotkey, "Please assign _hotkey in " + name);
            Assert.IsNotNull(_backgroundButton, "Please assign _backgroundButton in " + name);
            Assert.IsNotNull(_hoverDisplayPopup, "Please assign _hoverDisplayPopup in " + name);

            Assert.IsNotNull(orderContent.OnClick, "Order content on click is null in " + name);

            _hotkey.text = orderContent.Hotkey.ToString();
            _backgroundButton.sprite = orderContent.Portrait;
            HoverDisplayPopup.HoverPopupData = orderContent.HoverPopupData;

            _button.onClick.RemoveAllListeners();
            _button.onClick.AddListener(() => orderContent.OnClick());
        }

        public void EnableButtonInteraction()
        {
            _button.interactable = true;
        }

        public void DisableButtonInteraction()
        {
            _button.interactable = false;
        }
    }
}
