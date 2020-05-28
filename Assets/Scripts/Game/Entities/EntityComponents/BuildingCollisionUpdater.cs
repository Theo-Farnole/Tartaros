using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions;

// we don't set 'CollisionUpdater ' as EntityComponent.
// This script is a one run only.
//
// We don't want this script to be stored in the Entity's Dictionary 
// for performance reason
[RequireComponent(typeof(Entity))]
public class BuildingCollisionUpdater : MonoBehaviour
{
    void Start()
    {
        var entity = GetComponent<Entity>();

        Assert.IsNotNull(entity, string.Format("Missing Entity component inside object '{0}'.", name));

        Vector2Int tileSize = entity.Data.TileSize;
        Vector3 size = new Vector3(tileSize.x, 2, tileSize.y);

        UpdateBoxCollisionSize(size);
        UpdateNavMeshObstacleSize(size);

        AutoDestroy();
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
            navMeshObstacle.size = size;
            navMeshObstacle.center = size.y / 2 * Vector3.up;
        }
    }

    void AutoDestroy()
    {
        Destroy(this);
    }
}
