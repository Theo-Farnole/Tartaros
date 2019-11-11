using Lortedo.Utilities.Inspector;
using Lortedo.Utilities.Managers;
using Registers;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Game
{
    [System.Serializable]
    public class PanelConstruction : Panel
    {
        #region Fields
        [Space]
        [SerializeField] private Button[] _buildingButtons;
        #endregion

        #region Properties
        #endregion

        #region Methods
        public override void Initialize()
        {
            foreach (Building type in Enum.GetValues(typeof(Building)))
            {
                int i = (int)type - EntitiesSystem.STARTING_INDEX_BUILDING;

                _buildingButtons[i].onClick.AddListener(() => GameManager.Instance.StartBuilding(type));
                _buildingButtons[i].GetComponent<Image>().sprite = BuildingsRegister.Instance.GetItem(type).Portrait;
            }
        }

        public void UpdateConstructionButtons()
        {
            // deactive all _buildingButtons
            for (int i = 0; i < _buildingButtons.Length; i++)
            {
                _buildingButtons[i].gameObject.SetActive(false);
            }

            // then active buttons in Building int range
            foreach (Building building in Enum.GetValues(typeof(Building)))
            {
                int index = (int)building - EntitiesSystem.STARTING_INDEX_BUILDING;

                if (index < _buildingButtons.Length)
                {
                    _buildingButtons[index].gameObject.SetActive(true);
                    _buildingButtons[index].GetComponent<Image>().sprite = BuildingsRegister.Instance.GetItem(building).Portrait;
                }
            }
        }
        #endregion
    }
}
