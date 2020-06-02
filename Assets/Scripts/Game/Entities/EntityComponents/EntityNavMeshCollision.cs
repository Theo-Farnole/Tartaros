using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;

// Note: we could split this script into 2 scripts
// - one for UpdateCollisionSize() method
// - one for Enable/Disable Nav Mesh Obstacle
[RequireComponent(typeof(Entity))]
public class EntityNavMeshCollision : EntityComponent
{
    private const string header = "Update Mesh on Obstacle Update";

    [ToggleGroup(nameof(_updateMeshOnObstacleUpdate), header)]
    [SerializeField] private bool _updateMeshOnObstacleUpdate = true;

    [ToggleGroup(nameof(_updateMeshOnObstacleUpdate), header)]
    [SerializeField] private GameObject _meshNavMeshEnabled;

    [ToggleGroup(nameof(_updateMeshOnObstacleUpdate), header)]
    [SerializeField] private GameObject _meshNavMeshDisabled;

    private NavMeshObstacle _navMeshObstacle;

    #region MonoCallbacks
    void Start()
    {
        _navMeshObstacle = GetComponent<NavMeshObstacle>();

        UpdateCollisionSize();
        UpdateMesh();
    }
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
        UpdateMesh();
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
        UpdateMesh();
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
    private void UpdateMesh()
    {
        if (!Entity.Data.CanToggleNavMeshObstacle)
            return;

        if (!_navMeshObstacle)
            return;

        if (!_updateMeshOnObstacleUpdate)
            return;

        _meshNavMeshEnabled.SetActive(_navMeshObstacle.enabled);
        _meshNavMeshDisabled.SetActive(!_navMeshObstacle.enabled);   
    }

    private void UpdateCollisionSize()
    {
        Vector2Int tileSize = Entity.Data.TileSize;
        Vector3 size = new Vector3(tileSize.x, 2, tileSize.y);

        UpdateBoxCollisionSize(size);
        UpdateNavMeshObstacleSize(size);
    }

    void UpdateBoxCollisionSize(Vector3 size)
    {
        if (TryGetComponent(out BoxCollider boxCollider))
        {
            boxCollider.size = size;
            boxCollider.center = size.y / 2 * Vector3.up;
        }
    }

    void UpdateNavMeshObstacleSize(Vector3 size)
    {
        if (TryGetComponent(out NavMeshObstacle navMeshObstacle))
        {
            Vector3 carveOffset = new Vector3(0.9f, 0, 0.9f);
            navMeshObstacle.size = size - carveOffset;
            navMeshObstacle.center = size.y / 2 * Vector3.up;
        }
    }

    #endregion
}
