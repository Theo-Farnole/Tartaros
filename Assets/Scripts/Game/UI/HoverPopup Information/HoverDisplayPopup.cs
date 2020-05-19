using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Game.UI.HoverPopup
{
    public class HoverDisplayPopup : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] HoverPopupData _hoverPopupData;

        private RectTransform _rectTransform;

        public RectTransform RectTransform
        {

            get
            {
                if (_rectTransform == null)
                    _rectTransform = GetComponent<RectTransform>();

                return _rectTransform;
            }
        }

        public HoverPopupData HoverPopupData { get => _hoverPopupData; set => _hoverPopupData = value; }

        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            HoverPopupManager.Instance.DisplayHoverPopup(RectTransform, _hoverPopupData);
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            HoverPopupManager.Instance.HidePopUp();
        }
    }
}
