using Game.UI.HoverPopup;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace Game.UI
{
    public class UI_ConstructionButton : MonoBehaviour
    {
        [Required, SerializeField] private Button _button;
        [Required, SerializeField] private TextMeshProUGUI _hotkeyLabel;
        [Required, SerializeField] private Image _portrait;
        [Required, SerializeField] private HoverDisplayPopup _hoverDisplayPopup;

        public void SetBuildingType(string buildingID)
        {
            SetContent(buildingID);
        }

        public void SubcribeToEvents(string buildingID)
        {
            _button.onClick.AddListener(() => GameManager.Instance.StartBuilding(buildingID));
        }

        public void UnsubcribeToEvents(string buildingID)
        {
            _button.onClick.RemoveListener(() => GameManager.Instance.StartBuilding(buildingID));
        }

        private void SetContent(string buildingID)
        {
            Assert.IsNotNull(MainRegister.Instance, "MainRegister is missing.");

            if (MainRegister.Instance.TryGetEntityData(buildingID, out EntityData entityData))
            {
                SetContent(entityData);
            }
            else
            {
                throw new System.NotSupportedException("EntityData is null");
            }
        }

        void SetContent(EntityData entityData)
        {
            SetHotkey(entityData.Hotkey);
            SetPortrait(entityData.Portrait);
            SetHoverPopupDisplay(entityData.HoverPopupData);
        }

        void SetHotkey(KeyCode keycode)
        {
            Assert.IsNotNull(_hotkeyLabel);
            _hotkeyLabel.text = keycode != KeyCode.None ? keycode.ToString() : string.Empty;
        }

        void SetPortrait(Sprite portrait)
        {
            Assert.IsNotNull(_portrait);
            _portrait.sprite = portrait;
        }

        void SetHoverPopupDisplay(HoverPopupData hoverPopupData)
        {
            Assert.IsNotNull(_hoverDisplayPopup);
            _hoverDisplayPopup.HoverPopupData = hoverPopupData;
        }
    }
}
