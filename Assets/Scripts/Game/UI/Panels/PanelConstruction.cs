using Lortedo.Utilities.Inspector;
using Lortedo.Utilities.Managers;
using System;
using System.Collections.Generic;
using TMPro;
using Game.UI.HoverPopup;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;

namespace Game.UI
{
    [Serializable]
    public class PanelConstruction : Panel
    {
        #region Fields
        private const string debugLogHeader = "Panel Construction : ";

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
            CreateConstructionButtons();
        }

        public override void SubscribeToEvents<T>(T uiManager)
        {
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

            Array buildingTypeValues = Enum.GetValues(typeof(BuildingType));
            foreach (BuildingType buildingType in buildingTypeValues)
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

            if (buildingButton.TryGetComponent(out UI_ConstructionButton constructionButton))
            {
                constructionButton.SetBuildingType(buildingType);
            }
            else
            {
                Debug.Log(debugLogHeader + "Prefab construction button miss a UI_ConstructionButton component.");
            }

            _buildingButtons[index] = buildingButton;
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
