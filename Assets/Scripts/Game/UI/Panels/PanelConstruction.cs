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
    public class PanelConstruction : AbstractPanel
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
        void Start()
        {
            if (_orderButtons == null)
                CreateConstructionButtons();
        }

        void OnEnable()
        {
            foreach (var orderButton in _orderButtons)
            {
                orderButton.EnableButtonInteraction();
            }
        }

        void OnDisable()
        {
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
