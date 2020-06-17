namespace Game.UI
{
    using System;
    using UnityEngine;
    using UnityEngine.Assertions;
    using Game.Selection;
    using Game.Entities;
    using Sirenix.OdinInspector;
    using Game.GameManagers;

    public class UI_OrdersManager : MonoBehaviour
    {
        [SerializeField] private RectTransform[] _ordersLineParent;

        // REFACTOR NOTE
        // We could use a flag, but refactoring OverallAction as a flag could lead to bugs.
        [SerializeField, EnumToggleButtons] private OverallAction _overallActionToIgnore = OverallAction.None;

        private OrderButton[][] _orders = null;
        private Entity _selectedEntity;

        #region Methods
        #region MonoBehaviour Callbacks
        void Awake()
        {
            InitializeOrdersField();
            HideOrders();
        }

        void OnEnable()
        {
            SelectionManager.OnSelectionUpdated += SelectionManager_OnSelectionUpdated;
            GameManager.PendingCreationEntityAdded += GameManager_PendingCreationEntityAdded;
            GameManager.PendingCreationEntityRemoved += GameManager_PendingCreationEntityRemoved;
            Entity.OnSpawn += Entity_OnSpawn;
            Entity.OnDeath += Entity_OnDeath;
        }

        void OnDisable()
        {
            SelectionManager.OnSelectionUpdated -= SelectionManager_OnSelectionUpdated;
            GameManager.PendingCreationEntityAdded -= GameManager_PendingCreationEntityAdded;
            GameManager.PendingCreationEntityRemoved -= GameManager_PendingCreationEntityRemoved;
            Entity.OnSpawn -= Entity_OnSpawn;
            Entity.OnDeath -= Entity_OnDeath;
        }
        #endregion

        #region Events Handlers
        private void SelectionManager_OnSelectionUpdated(SelectionManager.SelectionGroup[] selectedGroups, int highlightGroupIndex)
        {
			Assert.IsTrue(selectedGroups.IsIndexInsideBounds(highlightGroupIndex), "Highlight group index is out of bounds of selectedGroups");

			_selectedEntity = selectedGroups[highlightGroupIndex].unitsSelected[0];
			UpdateOrders(_selectedEntity);            
        }

        private void GameManager_PendingCreationEntityAdded(string id) => UpdateOrdersWithSelectedEntity();

        private void GameManager_PendingCreationEntityRemoved(string id) => UpdateOrdersWithSelectedEntity();

        private void Entity_OnSpawn(Entity entity) => UpdateOrdersWithSelectedEntity();

        private void Entity_OnDeath(Entity entity) => UpdateOrdersWithSelectedEntity();
        #endregion

        #region Private Methods
        /// <summary>
        /// Hide all orders, than set orders' content.
        /// </summary>
        /// <param name="entity"></param>
        private void UpdateOrders(Entity entity)
        {
            HideOrders();

            if (entity == null)
                return;

            SetAllOrders(entity);
        }

        private void HideOrders()
        {
            foreach (var line in _orders)
            {
                foreach (var lineElement in line)
                {
                    lineElement.gameObject.SetActive(false);
                }
            }
        }

        private void SetAllOrders(Entity entity)
        {
            OrderContent[] orders = entity.Data.GetAvailableOrders(_overallActionToIgnore);

            foreach (OrderContent order in orders)
            {
                SetOrderContent(order);
            }
        }

        private void SetOrderContent(OrderContent order)
        {
            Assert.IsNotNull(order);

            var orderWrapper = GetOrderWrapperFromOrder(order);

            Assert.IsNotNull(orderWrapper);

            orderWrapper.gameObject.SetActive(true);
            orderWrapper.SetContent(order);
        }

        private void UpdateOrdersWithSelectedEntity()
        {
            if (_selectedEntity == null)
                return;

            UpdateOrders(_selectedEntity);
        }

        /// <summary>
        /// Find each UI_OrderWrapper inside _ordersLineParent.
        /// </summary>
        private void InitializeOrdersField()
        {
            Assert.IsNull(_orders, "_orders field is not null. Can't initialize it.");

            _orders = new OrderButton[_ordersLineParent.Length][];

            for (int i = 0; i < _ordersLineParent.Length; i++)
            {
                int childCount = _ordersLineParent[i].childCount;

                _orders[i] = new OrderButton[childCount];

                for (int j = 0; j < childCount; j++)
                {
                    _orders[i][j] = _ordersLineParent[i].GetChild(j).GetComponent<OrderButton>();
                }
            }

        }

        private OrderButton GetOrderWrapperFromOrder(OrderContent order)
        {
            var orderWrappers = GetLineOrders(order.LinePosition);

            return GetFirstInactiveOrderWrapper(orderWrappers);
        }

        private OrderButton GetFirstInactiveOrderWrapper(OrderButton[] orderWrappers)
        {
            foreach (var orderWrapper in orderWrappers)
            {
                if (!orderWrapper.gameObject.activeInHierarchy)
                    return orderWrapper;
            }

            throw new NotSupportedException("Not enought UI_OrderWrapper.");
        }

        private OrderButton[] GetLineOrders(int linePosition)
        {
            // linePosition starts at 1
            // we remove one, to make linePosition starts at 0
            linePosition--;
            Assert.IsTrue(_orders.IsIndexInsideBounds(linePosition), string.Format("Line position {0} isn't inside bounds. Line position should be between {1} and {2}.", linePosition, 0, _orders.Length));

            return _orders[linePosition];
        }
        #endregion
        #endregion
    }
}
