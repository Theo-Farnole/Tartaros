using CommandPattern;
using Lortedo.Utilities.Inspector;
using Registers;
using System;
using System.Collections;
using System.Collections.Generic;
using UI.Game.HoverPopup;
using UnityEngine;

namespace UI.Game
{
    [Serializable]
    public class OrdersWrapper
    {
        [SerializeField, EnumNamedArray(typeof(OverallAction))] private Order[] _overallOrders = new Order[2];
        [SerializeField] private Order[] _spawnUnitsOrders = new Order[2];

        public void ResizeArrayIfNeeded()
        {
            if (_overallOrders.Length != Enum.GetValues(typeof(OverallAction)).Length)
            {
                Array.Resize(ref _overallOrders, Enum.GetValues(typeof(OverallAction)).Length);
            }
        }

        public void Initialize()
        {
            foreach (OverallAction action in Enum.GetValues(typeof(OverallAction)))
            {
                int index = (int)action;

                _overallOrders[index].GetComponent<HoverDisplayPopup>().HoverPopupData = OverallActionsRegister.Instance.GetItem(action).HoverPopupData;
            }
        }

        public void HideOrders()
        {
            for (int i = 0; i < _overallOrders.Length; i++)
            {
                _overallOrders[i].gameObject.SetActive(false);
            }

            for (int i = 0; i < _spawnUnitsOrders.Length; i++)
            {
                _spawnUnitsOrders[i].gameObject.SetActive(false);
            }
        }

        public void UpdateOrders(Entity unit)
        {
            HideOrders();

            if (unit == null)
                return;

            UpdateOverallOrders(unit);
            UpdateSpawnUnitsOrder(unit);
        }

        void UpdateSpawnUnitsOrder(Entity unit)
        {
            if (!unit.Data.CanSpawnUnit)
                return;

            for (int i = 0; i < _spawnUnitsOrders.Length && i < unit.Data.AvailableUnitsForCreation.Length; i++)
            {
                _spawnUnitsOrders[i].gameObject.SetActive(true);

                UnitType unitType = unit.Data.AvailableUnitsForCreation[i];
                _spawnUnitsOrders[i].hotkey.text = UnitsRegister.Instance.GetItem(unitType).Hotkey.ToString();
                _spawnUnitsOrders[i].backgroundButton.sprite = UnitsRegister.Instance.GetItem(unitType).Portrait;

                _spawnUnitsOrders[i].button.onClick.RemoveAllListeners();
                _spawnUnitsOrders[i].button.onClick.AddListener(() => SelectedGroupsActionsCaller.OrderSpawnUnits(unitType));
            }
        }

        void UpdateOverallOrders(Entity unit)
        {
            foreach (OverallAction action in Enum.GetValues(typeof(OverallAction)))
            {
                int index = (int)action;

                if (unit.Data.CanDoOverallAction(action))
                {
                    _overallOrders[index].gameObject.SetActive(true);

                    _overallOrders[index].hotkey.text = OverallActionsRegister.Instance.GetItem(action).Hotkey.ToString();
                    _overallOrders[index].backgroundButton.sprite = OverallActionsRegister.Instance.GetItem(action).Portrait;

                    _overallOrders[index].button.onClick.RemoveAllListeners();

                    switch (action)
                    {
                        case OverallAction.Stop:
                            _overallOrders[index].button.onClick.AddListener(() =>
                            {
                                SelectedGroupsActionsCaller.OrderStop();
                            });
                            break;

                        case OverallAction.Move:
                            _overallOrders[index].button.onClick.AddListener(() =>
                            {
                                SecondClickListener.Instance.ListenToMove();
                            });
                            break;

                        case OverallAction.Attack:
                            _overallOrders[index].button.onClick.AddListener(() =>
                            {
                                SecondClickListener.Instance.ListenToAttack();
                            });
                            break;
                    }
                }
                else
                {
                    _overallOrders[index].gameObject.SetActive(false);
                }
            }
        }
    }
}
