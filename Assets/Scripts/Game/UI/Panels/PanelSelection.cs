using Game.Selection;
using Lortedo.Utilities.Inspector;
using Lortedo.Utilities.Managers;
using Registers;
using System;
using TMPro;
using UnityEngine;

namespace UI.Game
{
    [Serializable]
    public class PanelSelection : Panel
    {
        #region Fields
        [Space(order = 0)]
        [Header("Selected Groups", order = 1)]
        [SerializeField] private GameObject _prefabSelectedGroup;
        [SerializeField] private Transform _parentSelectedGroup;
        [Header("Orders")]
        [SerializeField] private OrdersWrapper _ordersWrapper;

        private Group[] _selectedGroupWrapper = new Group[5];
        #endregion

        #region Properties
        #endregion

        #region Methods
        public override void Initialize<T>(T uiManager)
        {
            for (int i = 0; i < _selectedGroupWrapper.Length; i++)
            {
                _selectedGroupWrapper[i] = GameObject.Instantiate(_prefabSelectedGroup).GetComponent<Group>();
                _selectedGroupWrapper[i].transform.SetParent(_parentSelectedGroup.transform, false);
                _selectedGroupWrapper[i].gameObject.SetActive(false);
            }

            _ordersWrapper.HideOrders();

            SelectionManager.OnSelectionUpdated += UpdateSelection;
        }

        public override void OnValidate()
        {
            _ordersWrapper.ResizeArrayIfNeeded();
        }        

        public void UpdateSelection(SelectionManager.Group[] selectedGroups, int highlightGroupIndex)
        {
            UpdateSelectedGroups(selectedGroups);
            HighlightGroup(highlightGroupIndex);

            if (highlightGroupIndex >= 0 && highlightGroupIndex < selectedGroups.Length)
            {
                UpdateOrdersWrapper(selectedGroups[highlightGroupIndex].unitsSelected[0]);
            }
        }

        /// <summary>
        /// Set sprite & units number
        /// </summary>
        /// <param name="groupsSelected"></param>
        void UpdateSelectedGroups(SelectionManager.Group[] groupsSelected)
        {
            for (int i = 0; i < _selectedGroupWrapper.Length; i++)
            {
                if (i >= groupsSelected.Length)
                {
                    _selectedGroupWrapper[i].gameObject.SetActive(false);
                    continue;
                }

                _selectedGroupWrapper[i].gameObject.SetActive(true);
                _selectedGroupWrapper[i].portrait.sprite = EntitiesRegister.GetRegisterData(groupsSelected[i].entityType).Portrait;
                _selectedGroupWrapper[i].unitsCount.text = groupsSelected[i].unitsSelected.Count.ToString();
            }
        }

        /// <summary>
        /// Only one group can be highlight at the same time.
        /// </summary>
        void HighlightGroup(int highlightGroupIndex)
        {
            for (int i = 0; i < _selectedGroupWrapper.Length; i++)
            {
                bool isHighlight = (i == highlightGroupIndex);

                _selectedGroupWrapper[i].SetHighlight(isHighlight);
            }
        }

        void UpdateOrdersWrapper(Entity unit)
        {
            _ordersWrapper.UpdateOrders(unit);
        }
        #endregion
    }
}
