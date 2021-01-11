using Game.Selection;
using Lortedo.Utilities.Inspector;
using Lortedo.Utilities.Managers;
using Sirenix.OdinInspector;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

namespace Game.UI
{
    public class PanelSelection : AbstractPanel
    {
        #region Fields
        [Space(order = 0)]
        [Header("Selected Groups", order = 1)]
        [SerializeField, Required] private GameObject _prefabSelectionPortrait;
        [SerializeField, Required] private Transform _parentSelectionPortraits;

        private UISelectionGroupPortrait[] _selectionPortraitUI = new UISelectionGroupPortrait[5];
        #endregion

        #region Properties
        #endregion

        #region Methods
        #region MonoBehaviour Callbacks
        protected override void Start()
        {
            base.Start();

            CreationSelectionPortraitUI();

            SelectionManager.OnSelectionUpdated += OnSelectionUpdated;
        }
        #endregion

        private void CreationSelectionPortraitUI()
        {
            for (int i = 0; i < _selectionPortraitUI.Length; i++)
            {
                var uiSelectedGroup = GameObject.Instantiate(_prefabSelectionPortrait);

                if (uiSelectedGroup.TryGetComponent(out UISelectionGroupPortrait uiSelectionGroup))
                {
                    _selectionPortraitUI[i] = uiSelectionGroup;
                    _selectionPortraitUI[i].gameObject.SetActive(false);

                    if (_parentSelectionPortraits != null)
                        _selectionPortraitUI[i].transform.SetParent(_parentSelectionPortraits.transform, false);
                    else
                        Debug.LogErrorFormat("Panel information : Can't set parent of selection group, because _parentSelectedGroup is null!");
                }
                else
                {
                    Debug.LogErrorFormat("Panel information : Can't get UISelectionGroup component of uiSelectionGroups!");
                }
            }
        }

        public void OnSelectionUpdated(SelectionManager.SelectionGroup[] selectedGroups, int highlightGroupIndex)
        {
            UpdateSelectedGroups(selectedGroups);
            HighlightGroup(highlightGroupIndex);
        }

        /// <summary>
        /// Set sprite & units number
        /// </summary>
        /// <param name="selection"></param>
        void UpdateSelectedGroups(SelectionManager.SelectionGroup[] selection)
        {
            for (int i = 0; i < _selectionPortraitUI.Length; i++)
            {
                if (i >= selection.Length)
                {
                    _selectionPortraitUI[i].gameObject.SetActive(false);
                    continue;
                }

                _selectionPortraitUI[i].gameObject.SetActive(true);
                _selectionPortraitUI[i].unitsCount.text = selection[i].unitsSelected.Count.ToString();
                TrySetPortrait(_selectionPortraitUI[i], selection[i].entityID);
            }
        }

        private void TrySetPortrait(UISelectionGroupPortrait group, string entityID)
        {
            var entityData = MainRegister.Instance.GetEntityData(entityID);

            Assert.IsNotNull(entityData,
                string.Format("Panel Selection: can't set portrait of {0} entity.", entityID));

            Sprite portrait = entityData.Portrait;
            group.portrait.sprite = portrait;
        }

        /// <summary>
        /// Only one group can be highlight at the same time.
        /// </summary>
        void HighlightGroup(int highlightGroupIndex)
        {
            for (int i = 0; i < _selectionPortraitUI.Length; i++)
            {
                bool isHighlight = (i == highlightGroupIndex);

                _selectionPortraitUI[i].SetHighlight(isHighlight);
            }
        }
        #endregion
    }
}
