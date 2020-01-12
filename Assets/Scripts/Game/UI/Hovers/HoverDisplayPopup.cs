using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace UI.Game.HoverPopup
{
    public class HoverDisplayPopup : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        public HoverPopupInfo hoverPopupData;

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


        void IPointerEnterHandler.OnPointerEnter(PointerEventData eventData)
        {
            HoverPopupManager.Instance.DisplayHoverPopup(RectTransform, hoverPopupData);
        }

        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            HoverPopupManager.Instance.HidePopUp();
        }
    }
}
