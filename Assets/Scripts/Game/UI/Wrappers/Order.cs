using System.Collections;
using System.Collections.Generic;
using TMPro;
using Game.UI.HoverPopup;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class Order : MonoBehaviour
    {
        public Button button;
        [Space]
        public Image backgroundButton;
        public TextMeshProUGUI hotkey;
        [SerializeField] private HoverDisplayPopup _hoverDisplayPopup;

        public HoverDisplayPopup HoverDisplayPopup
        {
            get
            {
                if (_hoverDisplayPopup == null)
                    _hoverDisplayPopup = GetComponent<HoverDisplayPopup>();

                return _hoverDisplayPopup;
            }
        }
    }
}
