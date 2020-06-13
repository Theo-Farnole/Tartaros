using Game.Appearance;
using Game.Appearance.Walls;
using Game.Entities;
using Lortedo.Utilities.Debugging;
using Lortedo.Utilities.Pattern;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;

namespace Game.ConstructionSystem
{
    /// <summary>
    /// This script manages display of a building to be constructed.
    /// It manages deactivation of components. 
    /// It manages building shader properties.
    /// </summary>
    public class PreviewBuilding
    {
        #region Fields
        public readonly static string debugLogHeader = "Construction Building : ";

        private readonly GameObject _building;
        private readonly Vector2Int _buildingSize;
        private readonly bool _isChainedBuilding;
        private readonly string _entityID;

        private readonly EntityData _entityData;

        private bool _forcePreviewColor = false;
        private BuildingMesh _buildingMesh;
        #endregion

        #region Properties
        public GameObject Building { get => _building; }

        public BuildingMesh BuildingMesh
        {
            get
            {
                if (_buildingMesh == null)
                    _buildingMesh = _building.GetComponent<BuildingMesh>();

                return _buildingMesh;
            }
        }
        #endregion

        public PreviewBuilding(GameObject gameObject, string entityID, EntityData entityData)
        {
            _building = gameObject;
            _entityID = entityID;
            _isChainedBuilding = entityData.IsConstructionChained;
            _buildingSize = entityData.TileSize;
            _entityData = entityData;

            EnableBuildingComponents(false);

            EntitiesNeightboorManager.ManualRemove(_building.GetComponent<Entity>());
        }

        #region Methods
        #region Public Methods
        public void SetPosition(Vector3 newPosition)
        {
            if (newPosition != _building.transform.position)
            {
                _building.transform.position = newPosition;
                _building.GetComponent<EntityResourcesGeneration>().CalculateResourcesPerTick();
                UpdateBuildingMeshColor();
            }
        }

        public void Destroy() => _building.GetComponent<Entity>().Death();

        /// <summary>
        /// Set building mesh to 'NotInBuildState'
        /// </summary>
        public void ResetBuildingMeshColor()
        {
            BuildingMesh buildingMesh = _building.GetComponent<BuildingMesh>();

            if (buildingMesh == null)
            {
                Debug.LogErrorFormat(debugLogHeader + "The current building {0} is missing a BuildingMesh component.", _building.name);
                return;
            }

            buildingMesh.SetState(BuildingMesh.State.NotInBuildState);
        }

        public void EnableBuildingComponents(bool enabled)
        {
            Assert.IsNotNull(_building, "Building is null");

            var fowEntity = _building.GetComponent<EntityFogVision>();
            if (fowEntity) fowEntity.enabled = enabled;

            var collider = _building.GetComponent<Collider>();
            if (collider) collider.enabled = enabled;

            var navMeshAgent = _building.GetComponent<NavMeshAgent>();
            if (navMeshAgent) navMeshAgent.enabled = enabled;

            var navMeshObstacle = _building.GetComponent<NavMeshObstacle>();
            if (navMeshObstacle) navMeshObstacle.enabled = enabled;

            if (_building.TryGetComponent(out EntityResourcesGeneration resourcesGeneration))
                resourcesGeneration.EnableResourceProduction = enabled;

            if (_building.TryGetComponent(out Entity entity))
                entity.enabled = enabled; // disable OnSpawn call

            if (_building.TryGetComponent(out EntityFogCoverable entityFogCoverable))
                entityFogCoverable.enabled = enabled;

            WallAppearance wallAppearence = _building.GetComponentInChildren<WallAppearance>();
            if (wallAppearence)
                wallAppearence.enabled = enabled;
        }

        public void SetConstructionAsFinish(Team teamToSet)
        {
            EnableBuildingComponents(true);
            ResetBuildingMeshColor();

            if (Building.TryGetComponent(out Entity entity))
            {
                entity.Team = teamToSet;
            }

            Assert.IsNotNull(entity);

            EntitiesNeightboorManager.ManualAdd(_building.GetComponent<Entity>());

            DynamicsObjects.Instance.SetToParent(_building.transform, "Building");
        }

        public void ForceColor(BuildingMesh.State state)
        {
            _forcePreviewColor = true;
            BuildingMesh.SetState(state);
        }

        public void StopForceColor()
        {
            _forcePreviewColor = false;
            UpdateBuildingMeshColor();
        }
        #endregion

        #region Private Methods
        private void UpdateBuildingMeshColor()
        {
            if (_forcePreviewColor)
                return;

            BuildingMesh.State state = CanBeConstruct() ? BuildingMesh.State.CanBuild : BuildingMesh.State.CannotBuild;
            BuildingMesh.SetState(state);
        }

        private bool CanBeConstruct()
        {
            TileFlag condition = TileFlag.Free | TileFlag.Visible;
            string idCondition = _isChainedBuilding ? _entityID : string.Empty;

            return TileSystem.Instance.DoTilesFillConditions(_building.transform.position, _buildingSize, condition, idCondition);
        }
        #endregion
        #endregion
    }
}
