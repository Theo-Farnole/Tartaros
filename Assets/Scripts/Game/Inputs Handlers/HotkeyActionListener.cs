namespace Game.Inputs
{
    using Game.Entities;
    using Game.GameManagers;
    using Game.Selection;
    using System;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Assertions;

    public class HotkeyActionListener : MonoBehaviour
    {
        public enum AskCursor
        {
            None = 0,
            Move = 1,
            Attack = 2
        }

        #region Fields
        [SerializeField] private KeyCode _killEntityKeyCode = KeyCode.Delete;

        Dictionary<KeyCode, Action> _commands = new Dictionary<KeyCode, Action>();
        #endregion

        #region Methods
        #region MonoBehaviour Callbacks
        void Start()
        {
            SetHotkeyFromConstructionOrders();
        }

        void Update()
        {
            ManageHotkeyInputs();
        }

        void OnEnable()
        {
            SelectionManager.OnSelectionUpdated += OnSelectionUpdated;
            SelectionManager.OnSelectionClear += SelectionManager_OnSelectionClear;
        }

        void OnDisable()
        {
            SelectionManager.OnSelectionUpdated -= OnSelectionUpdated;
            SelectionManager.OnSelectionClear -= SelectionManager_OnSelectionClear;
        }
        #endregion

        #region Events Handlers
        private void OnSelectionUpdated(SelectionManager.SelectionGroup[] selectedGroups, int highlightGroupIndex) => SetHotkeyFromEntityOrders(selectedGroups[highlightGroupIndex].entityID);


        private void SelectionManager_OnSelectionClear() => SetHotkeyFromConstructionOrders();
        #endregion

        #region Inputs management
        private void ManageHotkeyInputs()
        {
            foreach (var kvp in _commands)
            {
                if (Input.GetKeyDown(kvp.Key))
                {
                    kvp.Value?.Invoke();

                    // if a key change the selection, and so the hotkeys, System throws an exception.
                    // To avoid that, we return after one hotkey has been pressed.
                    return;
                }
            }

            if (Input.GetKeyDown(_killEntityKeyCode))
            {
                SelectedGroupsActionsCaller.KillSelectedEntities();
            }
        }
        #endregion

        void ClearCommandsHandler()
        {
            _commands.Clear();
        }

        void SetHotkeyFromEntityOrders(string entityIDToListen)
        {
            ClearCommandsHandler();

            var data = MainRegister.Instance.GetEntityData(entityIDToListen);

            AddHotkeys(data);
        }

        void SetHotkeyFromConstructionOrders()
        {
            ClearCommandsHandler();

            var constructionOrders = GameManager.Instance.ManagerData.GetConstructionOrders();

            foreach (var order in constructionOrders)
            {
                AddOrderHotkey(order);
            }
        }

        void AddHotkeys(EntityData data)
        {
            Assert.IsNotNull(data, string.Format("Hotkey Listener: cannot find entity data. Aborting input listening."));

            var orders = data.GetAvailableOrders();

            foreach (var order in orders)
            {
                AddOrderHotkey(order);
            }
        }

        void AddOrderHotkey(OrderContent orderContent)
        {
            if (_commands.ContainsKey(orderContent.Hotkey))
            {
                Debug.LogWarningFormat("Hotkey {0} is already register. Aborting", orderContent.Hotkey);
                return;
            }

            _commands.Add(orderContent.Hotkey, orderContent.OnClick);
        }
        #endregion
    }
}
