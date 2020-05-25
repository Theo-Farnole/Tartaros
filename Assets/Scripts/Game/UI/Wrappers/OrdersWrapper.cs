using CommandPattern;
using Lortedo.Utilities.Inspector;
using System;
using System.Collections;
using System.Collections.Generic;
using Game.UI.HoverPopup;
using UnityEngine;
using UnityEngine.Assertions;

namespace Game.UI
{
    [Serializable]
    public class OrdersWrapper
    {
        [SerializeField, EnumNamedArray(typeof(OverallAction))] private Order[] _overallOrders = new Order[2];
        [SerializeField] private Order[] _spawnUnitsOrders = new Order[2];

        public void OnValidate()
        {
            ResizeArrayIfNeeded();
        }

        void ResizeArrayIfNeeded()
        {
            if (_overallOrders.Length != Enum.GetValues(typeof(OverallAction)).Length)
            {
                Array.Resize(ref _overallOrders, Enum.GetValues(typeof(OverallAction)).Length);
            }
        }

        #region Initialization
        public void Initialize()
        {
            SetHoverPopupOnOverallActionOrders();
        }

        void SetHoverPopupOnOverallActionOrders()
        {            
            Assert.AreEqual(_overallOrders.Length, Enum.GetValues(typeof(OverallAction)).Length, string.Format("Overall action orders should have {0} buttons. However, it have only {1}.", Enum.GetValues(typeof(OverallAction)).Length, _overallOrders.Length));

            for (int i = 0; i < _overallOrders.Length; i++)
            {
                OverallAction action = (OverallAction)i;

                var overallActionData = MainRegister.Instance.GetOverallActionData(action);

                Assert.IsNotNull(overallActionData, string.Format("Orders Wrapper: OverallActionData of {0} missing in MainRegister. Can't set hover popup.", action));

                _overallOrders[i].GetComponent<HoverDisplayPopup>().HoverPopupData = overallActionData.HoverPopupData;
            }
        }
        #endregion

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

        public void UpdateOrders(Entity entity)
        {
            HideOrders();

            if (entity == null)
                return;

            DisplayOverallOrders(entity);
            DisplayCreateUnitsOrders(entity);
        }

        void DisplayCreateUnitsOrders(Entity entity)
        {
            if (!entity.Data.CanSpawnUnit)
                return;

            for (int i = 0; i < _spawnUnitsOrders.Length && i < entity.Data.AvailableUnitsForCreation.Length; i++)
            {
                Order order = _spawnUnitsOrders[i];
                string entityID = entity.Data.AvailableUnitsForCreation[i];

                SetContent_UnitOrder(entity, order, entityID);
            }
        }

        private void SetContent_UnitOrder(Entity entity, Order order, string unitID)
        {
            order.gameObject.SetActive(true);

            order.button.onClick.RemoveAllListeners();
            order.button.onClick.AddListener(() => SelectedGroupsActionsCaller.OrderSpawnUnits(unitID));

            var entityData = MainRegister.Instance.GetEntityData(unitID);

            Assert.IsNotNull(entityData,
                string.Format("Orders Wrapper: Couldn't find EntityData of {0}. Can't display hotkey and portrait.", unitID));

            // UPGRADE NOTE: respect Law of Demeter
            order.hotkey.text = entityData.Hotkey.ToString();
            order.backgroundButton.sprite = entityData.Portrait;
            order.HoverDisplayPopup.HoverPopupData = entityData.HoverPopupData;
        }

        void DisplayOverallOrders(Entity unit)
        {
            foreach (OverallAction overallAction in Enum.GetValues(typeof(OverallAction)))
            {
                int index = (int)overallAction;

                if (unit.Data.CanDoOverallAction(overallAction))
                {
                    SetOverallOrderContent(overallAction, index);
                }
                else
                {
                    _overallOrders[index].gameObject.SetActive(false);
                }
            }
        }

        void SetOverallOrderContent(OverallAction overallAction, int index)
        {
            _overallOrders[index].gameObject.SetActive(true);

            // try display hotkey and backgroundButton
            OverallActionData overallActionData = MainRegister.Instance.GetOverallActionData(overallAction);

            Assert.IsNotNull(overallActionData,
                string.Format("Orders Wrapper: Could set hotkey and portrait of action {0}", overallActionData));

            _overallOrders[index].hotkey.text = overallActionData.Hotkey.ToString();
            _overallOrders[index].backgroundButton.sprite = overallActionData.Portrait;

            AddCommandOnClick(overallAction, index);
        }

        void AddCommandOnClick(OverallAction overallAction, int index)
        {
            _overallOrders[index].button.onClick.RemoveAllListeners();

            switch (overallAction)
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
    }
}
