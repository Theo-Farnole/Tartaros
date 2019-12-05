using Lortedo.Utilities.Inspector;
using Lortedo.Utilities.Managers;
using Registers;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Game
{
    [Serializable]
    public class PanelConstruction : Panel
    {
        #region Fields
        [Space(order = 0)]
        [Header("Construction Button", order = 1)]
        [SerializeField] private GameObject _prefabConstructionButton;
        [SerializeField] private Transform _parentConstructionButton;

        [HideInInspector, SerializeField] private Button[] _buildingButtons = new Button[0];
        #endregion

        #region Properties
        #endregion

        #region Methods
        public override void Initialize()
        {
            // add StartBuilding listener to buttons
            int buildingEnumLength = Enum.GetValues(typeof(Building)).Length;

            for (int i = 0; i < buildingEnumLength; i++)
            {
                Building buildingType = (Building)Enum.GetValues(typeof(Building)).GetValue(i);

                _buildingButtons[i].onClick.AddListener(() => GameManager.Instance.StartBuilding(buildingType));                
            }
        }

        public void CreateConstructionButtons()
        {                  
            // destroy older buttons
            _parentConstructionButton.transform.DestroyImmediateChildren();

            int buildingEnumLength = Enum.GetValues(typeof(Building)).Length;
            _buildingButtons = new Button[buildingEnumLength];

            for (int i = 0; i < buildingEnumLength; i++)
            {
                Building buildingType = (Building) Enum.GetValues(typeof(Building)).GetValue(i);

                Button buildingButton = GameObject.Instantiate(_prefabConstructionButton).GetComponent<Button>();
                
                buildingButton.GetComponent<Image>().sprite = BuildingsRegister.Instance.GetItem(buildingType).Portrait;
                buildingButton.transform.SetParent(_parentConstructionButton, false);

                _buildingButtons[i] = buildingButton;
            }

            Canvas.ForceUpdateCanvases();
        }
        #endregion
    }
}
