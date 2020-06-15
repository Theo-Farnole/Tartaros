using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.UI
{
    public class UISelectionGroupPortrait : MonoBehaviour
    {
        // REFACTOR NOTE
        // Set field as private, and use property
        public Image portrait;
        public TextMeshProUGUI unitsCount;

        void Awake()
        {
            SetHighlight(false);
        }

        [Obsolete]
        public void SetHighlight(bool isHighlight)
        {
            // REFACTOR NOTE:
            // Remove this method. 
        }
    }
}
