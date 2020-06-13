namespace Game.Appearance.Walls
{
    using Game.TileSystem;
    using System;
    using UnityEngine;

    /// <summary>
    /// Detect construction or deletion of neighboor on TileSystem.
    /// 
    /// If yes, it fire two events:
    /// - OnWallOrientationChanged : WallOrientation is the wall direction.
    /// - OnWallJointChanged : a wall joint is a wall in a corner or the end of a wall
    /// </summary>
    public class WallChainDetector : MonoBehaviour
    {
        #region Fields
        public event Action<BuildingChainOrientation> OnWallOrientationChanged;
        public event Action<bool> OnWallJointChanged;

        [Header("IDS")]
        [SerializeField] private string[] _wallIDs = new string[] { "building_wall" }; // used on "IsWall" method

        private BuildingChainOrientation _currentBuildingChainOrientation;
        #endregion

        #region Properties
        public bool Cached_IsWallJoint { get => _currentBuildingChainOrientation == BuildingChainOrientation.NotAWallOrWallJoint; }
        public BuildingChainOrientation Cached_CurrentWallOrientation { get => _currentBuildingChainOrientation; }
        #endregion

        #region Methods
        #region MonoBehaviour Callbacks
        void OnEnable()
        {
            TileSystem.Instance.OnTileTerrainChanged += Instance_OnTileTerrainChanged;
        }

        void OnDisable()
        {
            if (TileSystem.Instance != null)
            {
                TileSystem.Instance.OnTileTerrainChanged -= Instance_OnTileTerrainChanged;
            }
        }
        #endregion

        #region Events Handlers
        void Instance_OnTileTerrainChanged(Vector2Int changedCoords, GameObject gameObjectAtCoords)
        {
            Vector2Int myCoords = TileSystem.Instance.WorldToCoords(transform.position);

            // only change appearance if tiles are neightboor
            if (TileSystem.Instance.DoTilesAreNeightboor(myCoords, changedCoords))
            {
                ForceCalculateWallOrientation();

                OnWallOrientationChanged?.Invoke(_currentBuildingChainOrientation);
                OnWallJointChanged?.Invoke(_currentBuildingChainOrientation == BuildingChainOrientation.NotAWallOrWallJoint);
            }
        }
        #endregion

        #region Private Methods
        public void ForceCalculateWallOrientation()
        {
            _currentBuildingChainOrientation = GetCurrentWallOrientation();
        }

        BuildingChainOrientation GetCurrentWallOrientation() => TileSystem.Instance.GetBuildingChainOrientation(transform.position, _wallIDs);
        #endregion
        #endregion
    }
}