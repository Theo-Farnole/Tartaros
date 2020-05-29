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

        private UI_OrderWrapper[] _orderButtons = null;
        #endregion

        #region Methods
        #region Public override
        public override void Initialize<T>(T uiManager)
        {
            base.Initialize(uiManager);

            if (_orderButtons == null)
                CreateConstructionButtons();
        }

        public override void SubscribeToEvents<T>(T uiManager)
        {
            base.SubscribeToEvents(uiManager);

            foreach (var orderButton in _orderButtons)
            {
                orderButton.EnableButtonInteraction();
            }
        }

        public override void UnsubscribeToEvents<T>(T uiManager)
        {
            base.UnsubscribeToEvents(uiManager);

            foreach (var orderButton in _orderButtons)
            {
                orderButton.DisableButtonInteraction();
            }
        }
        #endregion

        #region Private methods
        private void CreateConstructionButtons()
        {
            if (_orderButtons != null)
            {
                Debug.LogWarningFormat("Panel Construction : Recreate building buttons.");
            }

            // destroy older buttons
            _parentConstructionButton.transform.DestroyChildren();

            // create a button for each entries in game manager 'buildings in panel construction'
            var constructionOrders = GameManager.Instance.ManagerData.GetConstructionOrders();

            _orderButtons = new UI_OrderWrapper[constructionOrders.Length];

            for (int i = 0; i < constructionOrders.Length; i++)
            {
                OrderContent order = constructionOrders[i];
                CreateConstructionButton(order, i);
            }

            Canvas.ForceUpdateCanvases();
        }

        private void CreateConstructionButton(OrderContent orderContent, int index)
        {
            // instanciate button
            var instanciatedButton = UnityEngine.Object.Instantiate(_prefabConstructionButton);
            instanciatedButton.transform.SetParent(_parentConstructionButton, false);

            // set building type on construction button
            UI_OrderWrapper orderWrapper = instanciatedButton.GetComponent<UI_OrderWrapper>();
            orderWrapper.SetContent(orderContent);

            Assert.IsNotNull(orderWrapper, "Prefab construction prefab misses a UI_ConstructionButton component.");

            _orderButtons[index] = orderWrapper;
        }
        #endregion
        #endregion
    }
}
