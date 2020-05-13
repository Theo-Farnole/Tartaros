using Game.Selection;
using Lortedo.Utilities.Inspector;
using Lortedo.Utilities.Managers;
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

        private UISelectionGroup[] _uiSelectionGroupsWrapper = new UISelectionGroup[5];
        #endregion

        #region Properties
        #endregion

        #region Methods
        public override void Initialize<T>(T uiManager)
        {
            for (int i = 0; i < _uiSelectionGroupsWrapper.Length; i++)
            {
                _uiSelectionGroupsWrapper[i] = GameObject.Instantiate(_prefabSelectedGroup).GetComponent<UISelectionGroup>();
                _uiSelectionGroupsWrapper[i].transform.SetParent(_parentSelectedGroup.transform, false);
                _uiSelectionGroupsWrapper[i].gameObject.SetActive(false);
            }

            _ordersWrapper.HideOrders();

            SelectionManager.OnSelectionUpdated += UpdateSelection;
        }

        public override void OnValidate()
        {
            _ordersWrapper.ResizeArrayIfNeeded();
            _ordersWrapper.Initialize();
        }

        public void UpdateSelection(SelectionManager.SelectionGroup[] selectedGroups, int highlightGroupIndex)
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
        /// <param name="selection"></param>
        void UpdateSelectedGroups(SelectionManager.SelectionGroup[] selection)
        {
            for (int i = 0; i < _uiSelectionGroupsWrapper.Length; i++)
            {
                if (i >= selection.Length)
                {
                    _uiSelectionGroupsWrapper[i].gameObject.SetActive(false);
                    continue;
                }

                _uiSelectionGroupsWrapper[i].gameObject.SetActive(true);
                _uiSelectionGroupsWrapper[i].unitsCount.text = selection[i].unitsSelected.Count.ToString();
                TrySetPortrait(_uiSelectionGroupsWrapper[i], selection[i].entityType);
            }
        }

        private void TrySetPortrait(UISelectionGroup group, EntityType entityType)
        {
            if (MainRegister.Instance.TryGetEntityData(entityType, out EntityData entityData))
            {
                Sprite portrait = entityData.Portrait;
                group.portrait.sprite = portrait;
            }
            else
            {
                Debug.LogErrorFormat("Panel Selection: can't set portrait of {0} entity.", entityType);
            }
        }

        /// <summary>
        /// Only one group can be highlight at the same time.
        /// </summary>
        void HighlightGroup(int highlightGroupIndex)
        {
            for (int i = 0; i < _uiSelectionGroupsWrapper.Length; i++)
            {
                bool isHighlight = (i == highlightGroupIndex);

                _uiSelectionGroupsWrapper[i].SetHighlight(isHighlight);
            }
        }

        void UpdateOrdersWrapper(Entity unit)
        {
            _ordersWrapper.UpdateOrders(unit);
        }
        #endregion
    }
}
