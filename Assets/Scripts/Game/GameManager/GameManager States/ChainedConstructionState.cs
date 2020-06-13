namespace Game.ConstructionSystem
{
    using Game.Appearance;
    using Game.Entities;
    using Lortedo.Utilities.Pattern;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using Game.TileSystem;

    /// <summary>
    /// This state is managed by GameManager. 
    /// It manage building that can be contrustec more than once per construct like walls.
    /// </summary>
    public class ChainedConstructionState : AbstractConstructionState
    {
        #region Fields
        private new const string debugLogHeader = "Chained Construction State : ";

        private List<PreviewBuilding> _previewBuildings = new List<PreviewBuilding>();
        private List<PreviewBuilding> _constructionAchievedBuilding = new List<PreviewBuilding>();

        private int _constructableBuildingCount = 0;

        private bool _stateLeaving;
        private bool _anchorSet = false;
        private Vector3 _anchorPosition;
        #endregion

        public ChainedConstructionState(GameManager owner, string buildingID) : base(owner, buildingID)
        {
            if (EntityData.TileSize.x > 1 || EntityData.TileSize.y > 1)
            {
                throw new System.NotSupportedException(buildingID + " has a tilesize > 1 and construction chained.");
            }
        }

        #region Methods
        #region Protected Override
        public override void OnStateExit()
        {
            base.OnStateExit();

            if (SucessfulBuild)
            {
                DestroyUnachievedBuilding();
            }
        }

        public override void Tick()
        {
            // don't move the 'ManageBuildingsPosition' below 'base.Tick'
            // however, OnStateExit 'ManageBuildingsPosition' will create new 'ConstructionBuilding'
            // that'll never be deleted
            UpdatePreviewsPosition();

            base.Tick();
        }

        protected override ResourcesWrapper GetConstructionCost()
        {
            int constructableBuildings = StateLeaving ? _constructableBuildingCount : CalculateConstructableBuildingCount();
            return EntityData.SpawningCost * constructableBuildings;
        }

        protected override void OnMouseDown()
        {
            if (_anchorSet)
                return;

            if (GameManager.Instance.Grid.GetNearestPositionFromMouse(out Vector3 newPosition, terrainLayerMask))
            {
                SetAnchor(newPosition);
            }
            else
            {
                SucessfulBuild = false;
                LeaveState();
            }
        }

        protected override void OnMouseUp()
        {
            if (_anchorSet)
            {
                ConstructBuilding();
            }
        }

        protected override void LeaveState()
        {
            _constructableBuildingCount = CalculateConstructableBuildingCount();

            base.LeaveState();
        }

        public override void OnGUI()
        {
            if (Camera.main == null)
            {
                Debug.LogErrorFormat("Entity Resources Generation : Camera.main is null. Can't draw resources per tick.");
                return;
            }

            // don't if no construction building
            if (_previewBuildings.Count == 0)
                return;

            GameObject nearMouseBuilding = _previewBuildings.Last().Building;

            // Draw wood X food X stone X above building
            Vector2 guiPosition = Camera.main.WorldToScreenPoint(nearMouseBuilding.transform.position);

            // The WorldToScreenPoint functions return and integer starting from 0,0
            // at the BOTTOM LEFT of the screen.
            // Because of this, the y-value is flipped.
            // So to solve the problem, substract the screen height.
            guiPosition.y = Screen.height - guiPosition.y;

            Rect labelRect = new Rect(guiPosition.x, guiPosition.y, 300, 50);
            GUI.Label(labelRect, GetConstructionCost().ToString());
        }

        protected override void ConstructBuilding()
        {
            ResourcesWrapper constructionCost = GetConstructionCost();

            if (!_owner.HasEnoughtResources(constructionCost))
            {
                SucessfulBuild = false;
                LeaveState();
                return;
            }

            bool canConstruct = CanConstructAllBuildings();

            if (canConstruct)
            {
                ConstructAllBuildings();
            }

            SucessfulBuild = canConstruct;
            LeaveState();
        }

        protected override void DestroyAllConstructionBuildings()
        {
            foreach (var cBuilding in _previewBuildings)
            {
                cBuilding.Destroy();
            }

            _previewBuildings.Clear();
        }
        #endregion

        #region Private methods
        #region Construction / Destroy Methods
        private void DestroyUnachievedBuilding()
        {
            foreach (var cBuilding in _previewBuildings)
            {
                if (!IsConstructionBuildingAchieved(cBuilding))
                {
                    cBuilding.Destroy();
                }
            }
        }

        private bool CanConstructAllBuildings()
        {
            TileSystem tileSystem = TileSystem.Instance;

            foreach (var cBuilding in _previewBuildings)
            {
                Vector3 buildingPosition = cBuilding.Building.transform.position;

                // if the tile is the same type of 'EntityID'
                if (tileSystem.GetTile(buildingPosition) != null && tileSystem.DoTileContainsEntityOfID(buildingPosition, EntityID))
                    continue;

                if (tileSystem.DoTilesFillConditions(buildingPosition, EntityData.TileSize, TileFlag.Free | TileFlag.Visible))
                    continue;

                return false;
            }

            return true;
        }

        private void ConstructAllBuildings()
        {
            foreach (var cBuilding in _previewBuildings)
            {
                ConstructBuilding(cBuilding);
            }
        }

        private void ConstructBuilding(PreviewBuilding cBuilding)
        {
            GameObject building = cBuilding.Building;
            Vector3 buildingPosition = building.transform.position;

            TileSystem tileSystem = TileSystem.Instance;

            // don't make construction unsuccessful because building on the same entiy
            if (tileSystem.GetTile(buildingPosition) != null && tileSystem.DoTileContainsEntityOfID(buildingPosition, EntityID))
            {
                // destroy, but don't interupt construction
                cBuilding.Destroy();
                return;
            }

            const TileFlag condition = TileFlag.Free | TileFlag.Visible;
            bool tileSetSuccessfully = tileSystem.TrySetTile(building, EntityData.TileSize, condition, EntityID);

            if (tileSetSuccessfully)
            {
                cBuilding.SetConstructionAsFinish(Team.Player);
                _constructionAchievedBuilding.Add(cBuilding);
            }
        }
        #endregion

        private void SetAnchor(Vector3 newPosition)
        {
            _anchorSet = true;
            _anchorPosition = newPosition;
        }

        #region Set Building Position Methods
        private void UpdatePreviewsPosition()
        {
            if (!GameManager.Instance.Grid.GetNearestPositionFromMouse(out Vector3 mousePosition, terrainLayerMask))
            {
                Debug.LogWarningFormat("Construction State : " + "Can't find nearest position from mouse. We can't update the building position.");
                return;
            }

            // update the preview before anchoring
            if (!_anchorSet)
            {
                SetPreviewBuildingPosition(mousePosition);
                return;
            }

            Vector2Int[] path = TileSystem.Instance.GetPath(_anchorPosition, mousePosition);

            if (path.Length > 0)
            {
                SetBuildingsPosition(path);
            }
            else
            {
                SetPreviewBuildingPosition(mousePosition);
            }

            ManageBuildingColorOverwrite();
        }

        private void SetPreviewBuildingPosition(Vector3 position)
        {
            ResizeConstructionBuildings(1);

            // TODO: 'add missing construction' shuld be in resize construction building
            if (_previewBuildings.Count == 0)
                AddConstructionBuilding();

            _previewBuildings[0].SetPosition(position);
        }

        private void ManageBuildingColorOverwrite()
        {
            if (!CanConstructAllBuildings())
            {
                foreach (var cbuilding in _previewBuildings)
                {
                    cbuilding.ForceColor(BuildingMesh.State.CannotBuild);
                }
            }
            else
            {
                foreach (var cbuilding in _previewBuildings)
                {
                    cbuilding.StopForceColor();
                }

            }
        }

        private void SetBuildingsPosition(Vector2Int[] coords)
        {
            Vector3[] positions = new Vector3[coords.Length];

            for (int i = 0; i < positions.Length; i++)
            {
                positions[i] = TileSystem.Instance.CoordsToWorld(coords[i]);
            }

            SetBuildingsPosition(positions);
        }

        private void SetBuildingsPosition(Vector3[] positions)
        {
            int length = positions.Length;

            // delete excess entries 
            ResizeConstructionBuildings(length);

            for (int i = 0; i < length; i++)
            {
                // TODO: 'add missing construction' shuld be in resize construction building
                // add missing construction
                if (!_previewBuildings.IsIndexInsideBounds(i))
                    AddConstructionBuilding();

                // update construction position
                _previewBuildings[i].SetPosition(positions[i]);
            }
        }
        #endregion

        #region List Modificators
        void AddConstructionBuilding()
        {
            var building = ObjectPooler.Instance.SpawnFromPool(EntityData.Prefab, Vector3.zero, Quaternion.identity, true);
            building.GetComponent<Entity>().Team = Team.Player;

            var constructionBuilding = new PreviewBuilding(building, EntityID, EntityData);
            _previewBuildings.Add(constructionBuilding);
        }

        void ResizeConstructionBuildings(int count)
        {
            for (int i = _previewBuildings.Count - 1; i >= 0; i--)
            {
                if (i >= count)
                {
                    _previewBuildings[i].Destroy();
                    _previewBuildings.RemoveAt(i);
                }
            }
        }
        #endregion

        #region Getter & Calculation methods
        private bool IsConstructionBuildingAchieved(PreviewBuilding cBuilding)
        {
            return (_constructionAchievedBuilding != null && _constructionAchievedBuilding.Contains(cBuilding));
        }

        private bool DoBuildingConstructionHasACost(PreviewBuilding cBuilding)
        {
            Vector2Int coords = TileSystem.Instance.WorldToCoords(cBuilding.Building.transform.position);

            GameObject tile = TileSystem.Instance.GetTile(coords);

            return (tile == null) ||
                (TileSystem.Instance.IsTileFree(coords) && !TileSystem.Instance.DoTileContainsEntityOfID(coords, EntityID));
        }

        int CalculateConstructableBuildingCount()
        {
            return _previewBuildings.Where(x => DoBuildingConstructionHasACost(x) == true).Count();
        }
        #endregion
        #endregion
        #endregion
    }
}
