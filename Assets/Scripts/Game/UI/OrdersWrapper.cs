using Registers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI.Game
{
    [Serializable]
    public class OrdersWrapper
    {
        [SerializeField, EnumNamedArray(typeof(OverallAction))] private CommandsWrapper[] _wrapperOverallOrders = new CommandsWrapper[2];
        [SerializeField] private CommandsWrapper[] _wrapperSpawnUnitsOrders = new CommandsWrapper[2];

        public void OnValidateCallback()
        {
            if (_wrapperOverallOrders.Length != Enum.GetValues(typeof(OverallAction)).Length)
            {
                Array.Resize(ref _wrapperOverallOrders, Enum.GetValues(typeof(OverallAction)).Length);
            }
        }

        public void HideOrders()
        {
            for (int i = 0; i < _wrapperOverallOrders.Length; i++)
            {
                _wrapperOverallOrders[i].gameObject.SetActive(false);
            }

            for (int i = 0; i < _wrapperSpawnUnitsOrders.Length; i++)
            {
                _wrapperSpawnUnitsOrders[i].gameObject.SetActive(false);
            }
        }

        public void UpdateOrders(OrdersReceiver orderReceiver)
        {
            HideOrders();

            if (orderReceiver == null)
                return;

            UpdateOverallOrders(orderReceiver);
            UpdateSpawnUnitsOrder(orderReceiver);
        }

        void UpdateSpawnUnitsOrder(OrdersReceiver orderReceiver)
        {
            for (int i = 0; i < _wrapperSpawnUnitsOrders.Length && i < orderReceiver.CreatableUnits.Length; i++)
            {
                _wrapperSpawnUnitsOrders[i].gameObject.SetActive(true);

                Unit unitType = orderReceiver.CreatableUnits[i];
                _wrapperSpawnUnitsOrders[i].hotkey.text = UnitsRegister.Instance.GetItem(unitType).Hotkey.ToString();
                _wrapperSpawnUnitsOrders[i].backgroundButton.sprite = UnitsRegister.Instance.GetItem(unitType).Portrait;

                _wrapperSpawnUnitsOrders[i].button.onClick.RemoveAllListeners();
                _wrapperSpawnUnitsOrders[i].button.onClick.AddListener(() => OrderGiverManager.Instance.OrderSpawnUnits(unitType));
            }
        }

        void UpdateOverallOrders(OrdersReceiver orderReceiver)
        {
            foreach (OverallAction action in Enum.GetValues(typeof(OverallAction)))
            {
                int index = (int)action;

                if (orderReceiver.CanOverallAction(action))
                {
                    _wrapperOverallOrders[index].gameObject.SetActive(true);

                    _wrapperOverallOrders[index].hotkey.text = OverallActionsRegister.Instance.GetItem(action).Hotkey.ToString();
                    _wrapperOverallOrders[index].backgroundButton.sprite = OverallActionsRegister.Instance.GetItem(action).Portrait;

                    _wrapperOverallOrders[index].button.onClick.RemoveAllListeners();

                    switch (action)
                    {
                        case OverallAction.Stop:
                            _wrapperOverallOrders[index].button.onClick.AddListener(() =>
                            {
                                OrderGiverManager.Instance.OrderStop();
                            });
                            break;

                        case OverallAction.Move:
                            _wrapperOverallOrders[index].button.onClick.AddListener(() =>
                            {
                                HotkeyManager.Instance.askCursor = true;
                                HotkeyManager.Instance.askCursorType = HotkeyManager.AskCursor.Move;
                            });
                            break;

                        case OverallAction.Attack:
                            _wrapperOverallOrders[index].button.onClick.AddListener(() =>
                            {
                                HotkeyManager.Instance.askCursor = true;
                                HotkeyManager.Instance.askCursorType = HotkeyManager.AskCursor.Attack;
                            });
                            break;
                    }
                }
                else
                {
                    _wrapperOverallOrders[index].gameObject.SetActive(false);
                }
            }
        }
    }
}
