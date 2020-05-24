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

        private Button[] _buildingButtons = null;
        #endregion

        #region Properties
        private string[] GameManager_IDsInPanelConstruction => GameManager.Instance.ManagerData.IDsInPanelConstruction;
        #endregion

        #region Methods
        #region Public override
        public override void Initialize<T>(T uiManager)
        {
            base.Initialize(uiManager);

            if (_buildingButtons == null)
                CreateConstructionButtons();
        }

        public override void SubscribeToEvents<T>(T uiManager)
        {
            // make sure there is enought button
            Assert.AreEqual(GameManager_IDsInPanelConstruction.Length, _buildingButtons.Length,
                string.Format("<color=yellow>Panel construction</color> should have {0} building buttons, but there is {1}.", GameManager_IDsInPanelConstruction.Length, _buildingButtons.Length));

            BrowseIDsInPanelConstruction(
                (string id, int index) =>
                {
                    UI_ConstructionButton constructionButton = _buildingButtons[index].GetComponent<UI_ConstructionButton>();
                    Assert.IsNotNull(constructionButton, "Missing a UI_ConstructionButton component.");
                    constructionButton.SubcribeToEvents(id);
                }
            );
        }

        public override void UnsubscribeToEvents<T>(T uiManager)
        {
            BrowseIDsInPanelConstruction(
                (string id, int index) =>
                {
                    if (_buildingButtons != null && _buildingButtons[index] != null)
                    {
                        if (_buildingButtons[index].TryGetComponent(out UI_ConstructionButton constructionButton))
                        {
                            constructionButton.UnsubcribeToEvents(id);
                        }
                    }
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
            _buildingButtons = new Button[GameManager_IDsInPanelConstruction.Length];

            // create a button for each entries in game manager 'buildings in panel construction'
            BrowseIDsInPanelConstruction(CreateConstructionButton);

            // make sure that we have created enought buttons
            Assert.AreEqual(GameManager_IDsInPanelConstruction.Length, _buildingButtons.Length,
                string.Format("<color=yellow>Panel construction</color> should have {0} building buttons, but there is {1}.", GameManager_IDsInPanelConstruction.Length, _buildingButtons.Length));

            Canvas.ForceUpdateCanvases();
        }
        #endregion

        #region Private methods
        private void BrowseIDsInPanelConstruction(Action<string, int> action)
        {
            var array = GameManager.Instance.ManagerData.IDsInPanelConstruction;

            for (int i = 0; i < array.Length; i++)
            {
                string entityID = array[i];

                action?.Invoke(entityID, i);
            }
        }

        private void CreateConstructionButton(string entityID, int index)
        {
            Button buildingButton = GameObject.Instantiate(_prefabConstructionButton).GetComponent<Button>();
            buildingButton.transform.SetParent(_parentConstructionButton, false);

            // set building type on construction button
            UI_ConstructionButton constructionButton = buildingButton.GetComponent<UI_ConstructionButton>();
            Assert.IsNotNull(constructionButton, "Prefab construction prefab misses a UI_ConstructionButton component.");
            constructionButton.SetBuildingType(entityID);

            _buildingButtons[index] = buildingButton;
        }
        #endregion
        #endregion
    }
}
