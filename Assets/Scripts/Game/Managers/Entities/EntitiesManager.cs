using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// This script has been created for performance. It only call FindObjectOfType each frame.
/// </summary>
public class EntitiesManager
{

    private static int _lastFrameCount = -1;

    private static Entity[] _playerTeamEntities;
    private static Entity[] _enemyTeamEntities;

    private static void RecalculateEntities()
    {
        // PERFORMANCE NOTE: 
        // Replace 'FindObjectsOfType' by handling Entity.OnSpawn / OnDeath / OnSwap events
        Entity[] entities = Object.FindObjectsOfType<Entity>();

        // trier par team
        _playerTeamEntities = entities.Where(x => x.Team == Team.Player).ToArray();
        _enemyTeamEntities = entities.Where(x => x.Team == Team.Enemy).ToArray();

        _lastFrameCount = Time.frameCount;
    }

    public static Entity[] GetPlayerTeamEntities()
    {
        // PERFORMANCE NOTE : Calculation only each 2 or 3 frames
        if (_lastFrameCount != Time.frameCount)
            RecalculateEntities();

        return _playerTeamEntities;
    }

    public static Entity[] GetEnemyTeamEntities()
    {
        // PERFORMANCE NOTE : Calculation only each 2 or 3 frames
        if (_lastFrameCount != Time.frameCount)
            RecalculateEntities();

        return _enemyTeamEntities;
    }
}
