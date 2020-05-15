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

        [HideInInspector, SerializeField] private Button[] _buildingButtons;
        #endregion

        #region Properties
        #endregion

        #region Methods
        #region Public override
        public override void Initialize<T>(T uiManager)
        {
            base.Initialize(uiManager);

            // bloat code
            // but we can't create construction button on Awake
            // because, MainRegister'll return Null Reference :/
            uiManager.ExecuteAfterFrame(CreateConstructionButtons);
        }

        public override void SubscribeToEvents<T>(T uiManager)
        {
            if (_buildingButtons == null)
                CreateConstructionButtons();

            CheckIfThereIsEnoughtBuildingButtons();

            BrowseThrowBuildingType(
                (BuildingType buildingType, int index) =>
                {
                    if (_buildingButtons[index] != null)
                    {
                        _buildingButtons[index].onClick.AddListener(() => GameManager.Instance.StartBuilding(buildingType));
                    }
                    else
                    {
                        Debug.LogErrorFormat("Panel Construction : button at index {0} is null.", index);
                    }
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
            if (_buildingButtons != null)
            {
                Debug.LogWarningFormat("Panel Construction : Recreate building buttons.");
            }

            // destroy older buttons
            _parentConstructionButton.transform.DestroyImmediateChildren();

            // get building enum length
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

        private void CreateConstructionButton(BuildingType buildingType, int index)
        {
            Button buildingButton = GameObject.Instantiate(_prefabConstructionButton).GetComponent<Button>();

            buildingButton.transform.SetParent(_parentConstructionButton, false);
            TrySetPortraitOnButton(buildingButton, buildingType);
            TrySetHoverPopupDisplay(buildingType, buildingButton);

            _buildingButtons[index] = buildingButton;
        }

        private static void TrySetHoverPopupDisplay(BuildingType buildingType, Button buildingButton)
        {
            // set hoverdisplay popup
            if (buildingButton.gameObject.TryGetComponent(out HoverDisplayPopup hoverDisplayPopup))
            {
                if (MainRegister.Instance == null)
                {
                    Debug.LogErrorFormat("Panel Construction : MainRegister is missing. Can't set hover popup display on construction button.");
                    return;
                }


                if (MainRegister.Instance.TryGetBuildingData(buildingType, out EntityData entityData))
                {
                    hoverDisplayPopup.HoverPopupData = entityData.HoverPopupData;
                }
                else
                {
                    Debug.LogErrorFormat("Panel Construction : Can't find entityData of {0} building for construction button {1}.", buildingType, buildingButton.name);
                }
            }
            else
            {
                Debug.LogErrorFormat("Panel Construction : Construction button '{0}' miss a HoverDisplayPopup component.", buildingButton.name);
            }
        }

        private static void TrySetPortraitOnButton(Button buildingButton, BuildingType buildingType)
        {
            if (MainRegister.Instance == null)
            {
                Debug.LogErrorFormat("Panel Construction : MainRegister is missing. Can't set portrait on construction button.");
                return;
            }

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
