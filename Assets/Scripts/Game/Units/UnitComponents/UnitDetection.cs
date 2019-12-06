using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class UnitDetection : UnitComponent
{
    /// <summary>
    /// Use a OverlapSphere, then process the OverlapSphere output with 2 Linq queries.
    /// </summary>
    /// <returns></returns>
    public Unit[] GetAllEnemiesInViewRadius()
    {
        var unitTeam = UnitManager.Team;
        
        var overlapCollision = Physics.OverlapSphere(transform.position, UnitManager.Data.ViewRadius);

        var enemiesOutput = overlapCollision
            .Select(x => x.GetComponent<Unit>()) // get all components
            .Where(x => x != null && x.Team != unitTeam) // remove allies and not Units component
            .ToArray();

        return enemiesOutput;
    }

    /// <summary>
    /// Use a OverlapSphere, then process the OverlapSphere output with 2 Linq queries. Then make a distance on every enemies.
    /// </summary>
    public Unit GetNearestEnemyInViewRadius()
    {
        Unit[] nearEnemies = GetAllEnemiesInViewRadius();

        if (nearEnemies.Length > 0)
        {
            Unit nearestEnemy = transform.GetClosestComponent(nearEnemies).Object;

            return nearestEnemy;
        }
        else
        {
            return null;
        }
    }

    public bool IsUnitInViewRadius(Unit targetUnit)
    {
        return Vector3.Distance(transform.position, targetUnit.transform.position) <= UnitManager.Data.ViewRadius;
    }
}
