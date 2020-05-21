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

        public void SetBuildingType(BuildingType buildingType)
        {
            SetContent(buildingType);
            AddListeners(buildingType);
        }

        void AddListeners(BuildingType buildingType)
        {
            _button.onClick.AddListener(() => GameManager.Instance.StartBuilding(buildingType));
        }

        private void SetContent(BuildingType buildingType)
        {
            Assert.IsNotNull(MainRegister.Instance, "MainRegister is missing.");

            MainRegister.Instance.TryGetBuildingData(buildingType, out EntityData entityData);

            Assert.IsNotNull(entityData,
                string.Format("Missing data for entity {0}", buildingType));

            SetContent(entityData);
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
