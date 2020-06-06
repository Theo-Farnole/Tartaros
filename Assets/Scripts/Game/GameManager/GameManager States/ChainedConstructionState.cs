using Game.Entities;
using Lortedo.Utilities.Pattern;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.EventSystems;

namespace Game.ConstructionSystem
{
    /// <summary>
    /// This state is managed by GameManager. 
    /// It manage building that can be contrustec more than once per construct like walls.
    /// </summary>
    public class ChainedConstructionState : AbstractConstructionState
    {
        #region Fields
        private new const string debugLogHeader = "Chained Construction State : ";

        private List<ConstructionBuilding> _constructionBuildings = new List<ConstructionBuilding>();
        private List<ConstructionBuilding> _constructionAchievedBuilding;

        private bool _anchorSet = false;
        private Vector3 _anchorPosition;
        #endregion

        public ChainedConstructionState(GameManager owner, string buildingID) : base(owner, buildingID)
        {
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
            ManageBuildingsPosition();

            base.Tick();
        }

        protected override ResourcesWrapper GetConstructionCost()
        {
            int constructableBuildings = CalculateConstructableBuildingCount();
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

            bool constructionSuccessful = TryConstructBuildings();

            if (constructionSuccessful)
            {
                _owner.Resources -= constructionCost;
            }

            SucessfulBuild = constructionSuccessful;
            LeaveState();
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
        #region Construction / Destroy Methods
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

        private bool TryConstructBuildings()
        {
            _constructionAchievedBuilding = new List<ConstructionBuilding>();

            foreach (var cBuilding in _constructionBuildings)
            {
                bool constructionSuccessful = TryConstructBuilding(cBuilding);

                if (!constructionSuccessful)
                    return false;
            }

            return true;
        }

        private bool TryConstructBuilding(ConstructionBuilding cBuilding)
        {
            GameObject building = cBuilding.Building;
            Vector3 buildingPosition = building.transform.position;

            TileSystem tileSystem = TileSystem.Instance;

            // don't make construction unsuccessful because building on the same entiy
            if (tileSystem.GetTile(buildingPosition) != null && tileSystem.DoTileContainsEntityOfID(buildingPosition, EntityID))
            {
                // destroy, but don't interupt construction
                cBuilding.Destroy();
                return true;
            }
            else
            {
                const TileFlag condition = TileFlag.Free | TileFlag.Visible;
                bool tileSetSuccessfully = tileSystem.TrySetTile(building, EntityData.TileSize, condition, EntityID);

                if (tileSetSuccessfully)
                {
                    cBuilding.SetConstructionAsFinish(Team.Player);
                    _constructionAchievedBuilding.Add(cBuilding);

                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
        #endregion

        private void SetAnchor(Vector3 newPosition)
        {
            _anchorSet = true;
            _anchorPosition = newPosition;
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
            var gameObject = ObjectPooler.Instance.SpawnFromPool(EntityData.Prefab, Vector3.zero, Quaternion.identity, true);

            var constructionBuilding = new ConstructionBuilding(gameObject, EntityID, EntityData);
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

        private bool DoBuildingConstructionHasACost(ConstructionBuilding cBuilding)
        {
            Vector2Int coords = TileSystem.Instance.WorldToCoords(cBuilding.Building.transform.position);

            GameObject tile = TileSystem.Instance.GetTile(coords);

            return (tile == null) ||
                (TileSystem.Instance.IsTileFree(coords) && !TileSystem.Instance.DoTileContainsEntityOfID(coords, EntityID));
        }

        int CalculateConstructableBuildingCount()
        {
            return _constructionBuildings.Where(x => DoBuildingConstructionHasACost(x) == true).Count();
        }
        #endregion
        #endregion
        #endregion
    }
}
