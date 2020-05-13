using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Game
{
    public class UISelectionGroup : MonoBehaviour
    {
        public Image portrait;
        public TextMeshProUGUI unitsCount;

        [SerializeField] private Image _highlightFrame;

        void Awake()
        {
            SetHighlight(false);
        }

        public void SetHighlight(bool isHighlight)
        {
            _highlightFrame.gameObject.SetActive(isHighlight);
        }
    }
}
