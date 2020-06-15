namespace Game.Entities
{
    using Lortedo.Utilities.Pattern;
    using Sirenix.OdinInspector;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.AI;
    using UnityEngine.Assertions;

    [RequireComponent(typeof(Entity))]
    public class EntityNavMeshCollisionToggler : AbstractEntityComponent, IPooledObject
    {
        #region Fields
        private const string header = "Update Mesh on Obstacle Update";

        [ToggleGroup(nameof(_updateMeshOnObstacleUpdate), header)]
        [SerializeField] private bool _updateMeshOnObstacleUpdate = true;

        [ToggleGroup(nameof(_updateMeshOnObstacleUpdate), header)]
        [SerializeField] private GameObject _meshNavMeshEnabled;

        [ToggleGroup(nameof(_updateMeshOnObstacleUpdate), header)]
        [SerializeField] private GameObject _meshNavMeshDisabled;

        private NavMeshObstacle _navMeshObstacle;

        string IPooledObject.ObjectTag { get; set; }
        #endregion

        #region MonoCallbacks
        void Awake()
        {
            _navMeshObstacle = GetComponent<NavMeshObstacle>();
        }

        void Start() => Initialize();
        #endregion

        #region IPooledObject
        void IPooledObject.OnObjectSpawn() => Initialize();
        #endregion

        #region Public Methods
        public void EnableNavMeshObstacle()
        {
            if (!_navMeshObstacle)
            {
                Debug.LogWarningFormat("{0} miss NavMeshObstacle component. Can't ToggleNavMeshObstacle", name);
                return;
            }

            if (!Entity.Data.CanToggleNavMeshObstacle)
                return;

            _navMeshObstacle.enabled = true;
            UpdateMeshAppereance();
        }

        public void DisableNavMeshObstacle()
        {
            if (!_navMeshObstacle)
            {
                Debug.LogWarningFormat("{0} miss NavMeshObstacle component. Can't ToggleNavMeshObstacle", name);
                return;
            }

            if (!Entity.Data.CanToggleNavMeshObstacle)
                return;

            _navMeshObstacle.enabled = false;
            UpdateMeshAppereance();
        }

        public void ToggleNavMeshObstacle()
        {
            if (!Entity.Data.CanToggleNavMeshObstacle)
                return;

            if (_navMeshObstacle.enabled) DisableNavMeshObstacle();
            else EnableNavMeshObstacle();
        }
        #endregion

        #region Private Methods
        private void Initialize()
        {
            if (_navMeshObstacle == null)
                return;

            //_navMeshObstacle.enabled = true;

            UpdateMeshAppereance();
        }

        private void UpdateMeshAppereance()
        {
            if (!Entity.Data.CanToggleNavMeshObstacle)
                return;

            if (_navMeshObstacle == null)
                return;

            if (!_updateMeshOnObstacleUpdate)
                return;

            _meshNavMeshEnabled.SetActive(_navMeshObstacle.enabled);
            _meshNavMeshDisabled.SetActive(!_navMeshObstacle.enabled);
        }
        #endregion
    }
}
