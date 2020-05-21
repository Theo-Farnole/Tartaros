using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

namespace Game.ConstructionSystem
{
    public class ChainedConstructionState : AbstractConstructionState
    {
        #region Fields
        private new const string debugLogHeader = "Chained Construction State : ";

        private List<ConstructionBuilding> _constructionBuildings = new List<ConstructionBuilding>();
        private List<ConstructionBuilding> _constructionAchievedBuilding;

        private bool _anchorSet = false;
        private Vector3 _anchorPosition;
        #endregion

        public ChainedConstructionState(GameManager owner, BuildingType buildingType) : base(owner, buildingType)
        {
        }

        #region Methods
        #region Protected Override
        public override void OnStateExit()
        {
            base.OnStateExit();

            DestroyUnachievedBuilding();
        }

        public override void Tick()
        {
            // don't move the 'ManageBuildingsPosition' below 'base.Tick'
            // however, OnStateExit 'ManageBuildingsPosition' will create new 'ConstructionBuilding'
            // that'll never be deleted
            ManageBuildingsPosition();
            base.Tick();
        }

        protected override void OnMouseDown()
        {
            if (_anchorSet)
                return;

            if (GameManager.Instance.Grid.GetNearestPositionFromMouse(out Vector3 newPosition, terrainLayerMask))
            {
                SetAnchor(newPosition);
            }
        }

        protected override void OnMouseUp()
        {
            if (_anchorSet)
            {
                ConstructBuilding();
            }
        }

        public override void OnGUI()
        {
            if (Camera.main == null)
            {
                Debug.LogErrorFormat("Entity Resources Generation : Camera.main is null. Can't draw resources per tick.");
                return;
            }

            // don't if no construction building
            if (_constructionBuildings.Count == 0)
                return;

            GameObject nearMouseBuilding = _constructionBuildings.Last().Building;

            // Draw wood X food X gold X above building
            Vector2 guiPosition = Camera.main.WorldToScreenPoint(nearMouseBuilding.transform.position);

            // The WorldToScreenPoint functions return and integer starting from 0,0
            // at the BOTTOM LEFT of the screen.
            // Because of this, the y-value is flipped.
            // So to solve the problem, substract the screen height.
            guiPosition.y = Screen.height - guiPosition.y;

            Rect labelRect = new Rect(guiPosition.x, guiPosition.y, 300, 50);
            GUI.Label(labelRect, CalculateWholeConstructionCost().ToString());
        }

        protected override void ConstructBuilding()
        {
            bool buildingsCanBeBuilt = true;

            // check if every cBuilding is on a freetile
            foreach (var cBuilding in _constructionBuildings)
            {
                // is tile free and is not a wall                
                if (!CanConstructBuiding(cBuilding))
                {
                    buildingsCanBeBuilt = false;
                    break;
                }
            }

            if (buildingsCanBeBuilt)
            {
                bool trySetTileFailed = false;

                _constructionAchievedBuilding = new List<ConstructionBuilding>();

                // We calculate cost before setting tiles.
                // However, after setting tiles 'CalculateWholeConstructionCost' returns 0.
                ResourcesWrapper constructionCost = CalculateWholeConstructionCost();

                foreach (var cBuilding in _constructionBuildings)
                {
                    bool trySetTileSucessful = TrySetTileAndConstructionAsFinish(cBuilding);

                    if (trySetTileSucessful)
                    {
                        _constructionAchievedBuilding.Add(cBuilding);
                    }
                    else
                    {
                        trySetTileFailed = true;
                        break;
                    }
                }

                SucessfulBuild = !trySetTileFailed;

                if (SucessfulBuild)
                {
                    _owner.Resources -= constructionCost;
                }
                else
                {
                    DestroyAllConstructionBuildings();
                }
            }

            _owner.State = null;
        }

        protected override void DestroyAllConstructionBuildings()
        {
            foreach (var cBuilding in _constructionBuildings)
            {
                cBuilding.Destroy();
            }

            _constructionBuildings.Clear();
        }
        #endregion

        #region Private methods
        private void DestroyUnachievedBuilding()
        {
            foreach (var cBuilding in _constructionBuildings)
            {
                if (!IsConstructionBuildingAchieved(cBuilding))
                {
                    cBuilding.Destroy();
                }
            }
        }

        private void SetAnchor(Vector3 newPosition)
        {
            _anchorSet = true;
            _anchorPosition = newPosition;
        }

        private bool TrySetTileAndConstructionAsFinish(ConstructionBuilding cBuilding)
        {
            GameObject building = cBuilding.Building;
            Vector2Int coords = TileSystem.Instance.WorldPositionToCoords(building.transform.position);

            // allow building on walls
            if (TileSystem.Instance.IsTileOfType(coords, EntityType.Wall))
            {
                cBuilding.Destroy();
                return true;
            }
            else
            {
                bool successfulSetTile = TileSystem.Instance.TrySetTile(building);

                if (successfulSetTile)
                {
                    cBuilding.SetConstructionAsFinish(Team.Sparta);
                    return true;
                }
                else
                {
                    Debug.LogErrorFormat(debugLogHeader + "Failed to set tile. Successfulbuild set to false");
                    return false;
                }
            }
        }

        #region Set Building Position Methods
        private void ManageBuildingsPosition()
        {
            if (GameManager.Instance.Grid.GetNearestPositionFromMouse(out Vector3 newPosition, terrainLayerMask))
            {
                if (_anchorSet)
                {
                    var path = TileSystem.Instance.GetPath(_anchorPosition, newPosition);

                    bool isPathEmpty = path.Length > 0;
                    if (isPathEmpty)
                    {
                        SetBuildingsPosition(path);
                    }
                    else
                    {
                        SetBuildingsPosition(newPosition);
                    }

                }
                else
                {
                    SetBuildingsPosition(newPosition);
                }
            }
            else
            {
                Debug.LogWarningFormat("Construction State : " + "Can't find nearest position from mouse. We can't update the building position.");
            }
        }

        private void SetBuildingsPosition(Vector3 position)
        {
            ResizeConstructionBuildings(1);

            // TODO: 'add missing construction' shuld be in resize construction building
            if (_constructionBuildings.Count == 0)
                AddConstructionBuilding();

            _constructionBuildings[0].SetPosition(position);
        }

        private void SetBuildingsPosition(Vector2Int[] coords)
        {
            Vector3[] positions = new Vector3[coords.Length];

            for (int i = 0; i < positions.Length; i++)
            {
                positions[i] = TileSystem.Instance.CoordsToWorldPosition(coords[i]);
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
                if (!_constructionBuildings.IsIndexInsideBounds(i))
                    AddConstructionBuilding();

                // update construction position
                _constructionBuildings[i].SetPosition(positions[i]);
            }
        }
        #endregion

        #region List Modificators
        void AddConstructionBuilding()
        {
            var gameObject = Object.Instantiate(BuildingData.Prefab);

            var constructionBuilding = new ConstructionBuilding(gameObject, EntityType, true);
            _constructionBuildings.Add(constructionBuilding);
        }

        void ResizeConstructionBuildings(int count)
        {
            for (int i = _constructionBuildings.Count - 1; i >= 0; i--)
            {
                if (i >= count)
                {
                    _constructionBuildings[i].Destroy();
                    _constructionBuildings.RemoveAt(i);
                }
            }
        }
        #endregion

        #region Getter & Calculation methods
        private bool IsConstructionBuildingAchieved(ConstructionBuilding cBuilding)
        {
            return (_constructionAchievedBuilding != null && _constructionAchievedBuilding.Contains(cBuilding));
        }

        private bool CanConstructBuiding(ConstructionBuilding cBuilding)
        {
            Vector2Int coords = TileSystem.Instance.WorldPositionToCoords(cBuilding.Building.transform.position);
            return TileSystem.Instance.IsTileFree(coords) || TileSystem.Instance.IsTileOfType(coords, EntityType);
        }

        private bool DoBuildingConstructionIsPaid(ConstructionBuilding cBuilding)
        {
            Vector2Int coords = TileSystem.Instance.WorldPositionToCoords(cBuilding.Building.transform.position);
            return TileSystem.Instance.IsTileFree(coords) && !TileSystem.Instance.IsTileOfType(coords, EntityType);
        }

        ResourcesWrapper CalculateWholeConstructionCost()
        {
            int constructableBuildings = CalculateConstructableBuildingCount();
            return BuildingData.SpawningCost * constructableBuildings;
        }

        int CalculateConstructableBuildingCount()
        {
            return _constructionBuildings.Where(x => DoBuildingConstructionIsPaid(x) == true).Count();
        }
        #endregion
        #endregion
        #endregion
    }
}
