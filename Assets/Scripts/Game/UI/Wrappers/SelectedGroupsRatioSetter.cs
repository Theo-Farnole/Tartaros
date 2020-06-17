namespace Game.UI
{
    using Game.Selection;
    using UnityEngine;
    using UnityEngine.UI;

    [RequireComponent(typeof(GridLayoutGroup))]
    public class SelectedGroupsRatioSetter : MonoBehaviour
    {
        #region Fields
        [SerializeField] private Vector2 _unitsCellSize = new Vector2(88.42F, 210);
        [SerializeField] private Vector2 _buildingsCellSize = new Vector2(80, 80);

        private GridLayoutGroup _gridLayoutGroup;
        #endregion

        #region Properties
        public GridLayoutGroup GridLayoutGroup
        {
            get
            {
                if (_gridLayoutGroup == null)
                    _gridLayoutGroup = GetComponent<GridLayoutGroup>();

                return _gridLayoutGroup;
            }
        }
        #endregion

        #region Methods
        void OnEnable()
        {
            SelectionManager.OnSelectionUpdated += OnSelectionUpdated;
        }

        void OnDisable()
        {
            SelectionManager.OnSelectionUpdated -= OnSelectionUpdated;            
        }

        private void OnSelectionUpdated(SelectionManager.SelectionGroup[] selectedGroups, int highlightGroupIndex)
        {
            switch (selectedGroups[highlightGroupIndex].unitsSelected[0].Data.EntityType)
            {
                case Entities.EntityType.Unit:
                    GridLayoutGroup.cellSize = _unitsCellSize;
                    break;

                case Entities.EntityType.Building:
                    GridLayoutGroup.cellSize = _buildingsCellSize;
                    break;

                default:
                    throw new System.NotImplementedException();
            }
            #endregion
        }
    }
}