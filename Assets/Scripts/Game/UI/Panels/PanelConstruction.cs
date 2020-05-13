using Lortedo.Utilities.Inspector;
using Lortedo.Utilities.Managers;
using System;
using System.Collections.Generic;
using TMPro;
using UI.Game.HoverPopup;
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
        #region Public override
        public override void Initialize<T>(T uiManager)
        {
            base.Initialize(uiManager);

            CreateConstructionButtons();
        }

        public override void SubscribeToEvents<T>(T uiManager)
        {
            CheckIfThereIsEnoughtBuildingButtons();

            BrowseThrowBuildingType(
                (BuildingType buildingType, int index) =>
                {
                    _buildingButtons[index].onClick.AddListener(() => GameManager.Instance.StartBuilding(buildingType));
                }
            );
        }

        public override void UnsubscribeToEvents<T>(T uiManager)
        {
            BrowseThrowBuildingType(
                (BuildingType buildingType, int index) =>
                {
                    _buildingButtons[index].onClick.RemoveListener(() => GameManager.Instance.StartBuilding(buildingType));
                }
            );
        }
        #endregion

        #region Public methods
        public void CreateConstructionButtons()
        {
            // destroy older buttons
            _parentConstructionButton.transform.DestroyImmediateChildren();

            int buildingEnumLength = Enum.GetValues(typeof(BuildingType)).Length;
            buildingEnumLength--; // we remove 'BuildingType.None'
            _buildingButtons = new Button[buildingEnumLength];

            BrowseThrowBuildingType(
                (BuildingType buildingType, int index) =>
                {
                    CreateConstructionButton(buildingType, index);
                }
            );

            CheckIfThereIsEnoughtBuildingButtons();

            Canvas.ForceUpdateCanvases();
        }
        #endregion

        #region Private methods
        private void BrowseThrowBuildingType(Action<BuildingType, int> action)
        {
            CheckIfThereIsEnoughtBuildingButtons();

            int index = 0;

            foreach (BuildingType buildingType in Enum.GetValues(typeof(BuildingType)))
            {
                if (buildingType == BuildingType.None)
                    continue;

                action?.Invoke(buildingType, index);
                index++;
            }
        }

        private void CreateConstructionButton(BuildingType buildingType, int buttonIndex)
        {
            Button buildingButton = GameObject.Instantiate(_prefabConstructionButton).GetComponent<Button>();
            buildingButton.transform.SetParent(_parentConstructionButton, false);
            TrySetPortraitOnButton(buildingButton, buildingType);

            _buildingButtons[buttonIndex] = buildingButton;
        }

        private static void TrySetPortraitOnButton(Button buildingButton, BuildingType buildingType)
        {
            if (MainRegister.Instance.TryGetBuildingData(buildingType, out EntityData buildingData))
            {
                Sprite portrait = buildingData.Portrait;
                buildingButton.GetComponent<Image>().sprite = portrait;
            }
            else
            {
                Debug.LogErrorFormat("Panel Construction: Couldn't set a portrait of building {0} on button {1}", buildingType, buildingButton);
            }
        }

        private void CheckIfThereIsEnoughtBuildingButtons()
        {
            int buildingEnumLength = Enum.GetValues(typeof(BuildingType)).Length;
            buildingEnumLength--; // we remove 'BuildingType.None'

            Assert.AreEqual(buildingEnumLength, _buildingButtons.Length,
                string.Format("<color=yellow>Panel construction</color> should have {0} building buttons, but there is only {1}.", buildingEnumLength, _buildingButtons.Length));
        }
        #endregion
        #endregion
    }
}
