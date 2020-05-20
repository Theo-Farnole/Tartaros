using Lortedo.Utilities.Debugging;
using Lortedo.Utilities.Pattern;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Game.ConstructionSystem
{
    public class ConstructionState : AbstractConstructionState
    {
        #region Fields
        ConstructionBuilding _constructionBuilding;
        #endregion

        public ConstructionState(GameManager owner, BuildingType buildingType) : base(owner, buildingType)
        { }

        #region Methods
        #region Public override
        public override void Tick()
        {
            UpdateConstructionBuildingPosition();

            base.Tick();
        }
        #endregion

        #region Protected override        
        protected override void OnMouseUp()
            => ConstructBuilding();

        protected override void OnCurrentBuildingSet(BuildingType buildingType, EntityData buildingData)
        {
            var buildingPrefab = Object.Instantiate(buildingData.Prefab);            
            _constructionBuilding = new ConstructionBuilding(buildingPrefab);

            UpdateConstructionBuildingPosition();
        }

        protected override void ConstructBuilding()
        {
            GameObject building = _constructionBuilding.Building;

            // register tile
            bool successfulSetTile = TileSystem.Instance.TrySetTile(building);

            if (!successfulSetTile)
                return;

            _constructionBuilding.SetConstructionAsFinish(Team.Sparta);

            // then leave
            SucessfulBuild = true;
            _owner.State = null;
        }

        protected override void DestroyAllConstructionBuildings()
        {
            _constructionBuilding.Destroy();
        }
        #endregion

        #region Private methods
        private void UpdateConstructionBuildingPosition()
        {
            if (GameManager.Instance.Grid.GetNearestPositionFromMouse(out Vector3 newPosition, terrainLayerMask))
            {
                _constructionBuilding.SetPosition(newPosition);
            }
            else
            {
                Debug.LogWarningFormat("Building State : Can't find nearest position from mouse. We can't update the building position.");
            }
        }
        #endregion
        #endregion
    }
}
