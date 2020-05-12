using Lortedo.Utilities.Inspector;
using Lortedo.Utilities.Managers;
using Registers;
using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;
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
        public override void SubscribeToEvents<T>(T uiManager) 
        {
            CheckIfThereIsEnoughtBuildingButtons();

            for (int i = 0; i < _buildingButtons.Length; i++)
            {
                BuildingType buildingType = (BuildingType)Enum.GetValues(typeof(BuildingType)).GetValue(i);
                _buildingButtons[i].onClick.AddListener(() => GameManager.Instance.StartBuilding(buildingType));
            }
        }

        public override void UnsubscribeToEvents<T>(T uiManager)
        {
            for (int i = 0; i < _buildingButtons.Length; i++)
            {
                BuildingType buildingType = (BuildingType)Enum.GetValues(typeof(BuildingType)).GetValue(i);
                _buildingButtons[i].onClick.RemoveListener(() => GameManager.Instance.StartBuilding(buildingType));
            }
        }

        public void CreateConstructionButtons()
        {                  
            // destroy older buttons
            _parentConstructionButton.transform.DestroyImmediateChildren();

            int buildingEnumLength = Enum.GetValues(typeof(BuildingType)).Length;
            _buildingButtons = new Button[buildingEnumLength];

            for (int i = 0; i < buildingEnumLength; i++)
            {
                BuildingType buildingType = (BuildingType)Enum.GetValues(typeof(BuildingType)).GetValue(i);
                CreateConstructionButton(buildingType, i);
            }

            Canvas.ForceUpdateCanvases();

            CheckIfThereIsEnoughtBuildingButtons();
        }

        private void CreateConstructionButton(BuildingType buildingType, int buttonIndex)
        {            
            Button buildingButton = GameObject.Instantiate(_prefabConstructionButton).GetComponent<Button>();

            buildingButton.GetComponent<Image>().sprite = BuildingsRegister.Instance.GetItem(buildingType).Portrait;
            buildingButton.transform.SetParent(_parentConstructionButton, false);

            _buildingButtons[buttonIndex] = buildingButton;
        }

        
        private void CheckIfThereIsEnoughtBuildingButtons()
        {
            int buildingEnumLength = Enum.GetValues(typeof(BuildingType)).Length;

            Assert.AreEqual(buildingEnumLength, _buildingButtons.Length, 
                string.Format("<color=yellow>Panel construction</color> should have {0} building buttons, but there is only {1}.", buildingEnumLength, _buildingButtons.Length));
        }
        #endregion
    }
}
