﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EntityDetection : EntityComponent
{
    public readonly static float DISTANCE_THRESHOLD = 0.3f;

    public bool IsEntityInAttackRange(Entity target)
    {
        return Vector3.Distance(transform.position, target.transform.position) <= Entity.Data.AttackRadius;
    }
    
    public bool IsNearFromEntity(Entity target)
    {
        if (Vector3.Distance(transform.position, target.transform.position) <= DISTANCE_THRESHOLD)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public bool IsNearFromPosition(Vector3 position)
    {
        if (Vector3.Distance(transform.position, position) <= DISTANCE_THRESHOLD)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    /// <summary>
    /// Use a OverlapSphere, then process the OverlapSphere output with 2 Linq queries.
    /// </summary>
    /// <returns></returns>
    public Entity[] GetAllEnemiesInViewRadius()
    {
        var entityTeam = Entity.Team;
        
        var overlapCollision = Physics.OverlapSphere(transform.position, Entity.Data.ViewRadius);

        var enemiesOutput = overlapCollision
            .Select(x => x.GetComponent<Entity>()) // get all components
            .Where(x => x != null && x.Team != entityTeam) // remove allies & null components
            .ToArray();

        return enemiesOutput;
    }

    /// <summary>
    /// Use a OverlapSphere, then process the OverlapSphere output with 2 Linq queries. Then make a distance on every enemies.
    /// </summary>
    public Entity GetNearestEnemyInViewRadius()
    {
        Entity[] nearEnemies = GetAllEnemiesInViewRadius();

        if (nearEnemies.Length > 0)
        {
            Entity nearestEnemy = transform.GetClosestComponent(nearEnemies).Object;

            return nearestEnemy;
        }
        else
        {
            return null;
        }
    }

    public bool IsEntityInViewRadius(Entity target)
    {
        return Vector3.Distance(transform.position, target.transform.position) <= Entity.Data.ViewRadius;
    }
}