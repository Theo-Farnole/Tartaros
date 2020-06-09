using Game.WaveSystem;
using Lortedo.Utilities.Inspector;
using Lortedo.Utilities.Managers;
using Sirenix.OdinInspector;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Assertions;

namespace Game.UI
{
    public class PanelGameResources : AbstractPanel
    {
        #region Fields
        [Space(order = 0)]
        [Header("RESOURCES", order = 1)]
        [SerializeField] private TextMeshProUGUI[] _resourcesLabel;
        [SerializeField] private TextMeshProUGUI _populationCount;

        [Header("INCOME")]
        [SerializeField] private IncomeCalculator _incomeCalculator;
        [SerializeField] private TextMeshProUGUI[] _incomesLabels;
        #endregion

        #region Methods
        #region MonoBehaviour Callbacks
        void Start()
        {
            _incomesLabels[(int)Resource.Food].text = "0";
            _incomesLabels[(int)Resource.Wood].text = "0";
            _incomesLabels[(int)Resource.Stone].text = "0";
        }

        void OnEnable()
        {
            GameManager.OnGameResourcesUpdate += UpdateResourcesLabel;
            PopulationManager.OnPopulationCountChanged += SetPopulationCountLabel;

            Assert.IsNotNull(_incomeCalculator, "Missing IncomeCalculator in inspector.");

            _incomeCalculator.OnIncomeChanged += UpdateIncomeLabel;
        }

        void OnDisable()
        {
            GameManager.OnGameResourcesUpdate -= UpdateResourcesLabel;
            PopulationManager.OnPopulationCountChanged -= SetPopulationCountLabel;
            _incomeCalculator.OnIncomeChanged -= UpdateIncomeLabel;
        }
        #endregion

        #region Private Methods
        private void SetPopulationCountLabel(int popCount, int maxPopCount)
        {
            Assert.IsNotNull(_populationCount, "Panel Game Information : Please assign a TextMeshProUGUI to _populationCount.");

            _populationCount.text = popCount.ToString() + " / " + maxPopCount.ToString();
        }

        private void UpdateResourcesLabel(ResourcesWrapper currentResources)
        {
            _resourcesLabel[(int)Resource.Food].text = currentResources.food.ToString();
            _resourcesLabel[(int)Resource.Wood].text = currentResources.wood.ToString();
            _resourcesLabel[(int)Resource.Stone].text = currentResources.stone.ToString();
        }

        private void UpdateIncomeLabel(ResourcesWrapper incomeLabel)
        {
            _incomesLabels[(int)Resource.Food].text = incomeLabel.food.ToString();
            _incomesLabels[(int)Resource.Wood].text = incomeLabel.wood.ToString();
            _incomesLabels[(int)Resource.Stone].text = incomeLabel.stone.ToString();
        }
        #endregion
        #endregion
    }
}
