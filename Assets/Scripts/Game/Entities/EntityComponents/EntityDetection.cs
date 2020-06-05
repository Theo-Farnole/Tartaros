using Lortedo.Utilities.Pattern;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public delegate void OnEntityDetected(Entity entity);

/// <summary>
/// This script manage the detection of nearby entities.
/// </summary>
public class EntityDetection : EntityComponent
{
    #region Fields
    private const string debugLogHeader = "Entity Detection : ";
    public readonly static float DISTANCE_THRESHOLD = 0.3f;

    public event OnEntityDetected OnAllyEnterShiftRange;
    public event OnEntityDetected OnEnemyEnterAttackRange;

    private Entity _nearestEnemyTeamEntity = null;
    private Entity _nearestPlayerTeamEntity = null;
    #endregion

    #region Methods
    #region MonoBehaviour Callbacks   
    void Update()
    {
        // PERFORMANCE NOTE: calculate each 2/3 frames
        
        CalculateNearestPlayerTeamEntity();
        CalculateNearestEnemyTeamEntity();
    }
    #endregion

    #region Public methods
    public bool IsEntityInAttackRange(Entity target)
    {
        Assert.IsNotNull(target);
        Assert.IsNotNull(target.Data);
        Assert.IsNotNull(Entity);
        Assert.IsNotNull(Entity.Data);

        return Vector3.Distance(transform.position, target.transform.position) <= Entity.Data.AttackRadius + Mathf.Max(target.Data.TileSize.x, target.Data.TileSize.y);
    }

    public bool IsEntityInShiftRange(Entity target)
    {
        Assert.IsNotNull(target);
        Assert.IsNotNull(target.Data);
        Assert.IsNotNull(Entity);
        Assert.IsNotNull(Entity.Data);

        return Vector3.Distance(transform.position, target.transform.position) <= Entity.Data.StartShiftRange + Mathf.Max(target.Data.TileSize.x, target.Data.TileSize.y);
    }

    public bool IsNearFromEntity(Entity target)
    {
        return Vector3.Distance(transform.position, target.transform.position) <= DISTANCE_THRESHOLD + Mathf.Max(target.Data.TileSize.x, target.Data.TileSize.y);
    }

    public bool IsNearFromPosition(Vector3 position)
    {
        return Vector3.Distance(transform.position, position) <= DISTANCE_THRESHOLD;
    }

    public Entity GetNearestEnemyInViewRadius()
    {
        return Entity.Team == Team.Player ? _nearestEnemyTeamEntity : _nearestPlayerTeamEntity;
    }

    public Entity GetNearestAllyInViewRadius()
    {
        return Entity.Team == Team.Player ? _nearestPlayerTeamEntity : _nearestEnemyTeamEntity;
    }

    public bool IsEntityInViewRadius(Entity target)
    {
        return Vector3.Distance(transform.position, target.transform.position) <= Entity.Data.ViewRadius;
    }
    #endregion

    #region Private Methods
    private void CalculateNearestEnemyTeamEntity()
    {
        Entity[] enemyTeamEntities = EntitiesManager.GetEnemyTeamEntities();

        // PERFORMANCE NOTE: instead of 'GetClosestComponent' use a kd tree
        _nearestEnemyTeamEntity = enemyTeamEntities.Length == 0 ? null : transform.GetClosestComponent(enemyTeamEntities).Object;

        if (_nearestEnemyTeamEntity != null && IsEntityInAttackRange(_nearestEnemyTeamEntity))
        {
            OnEnemyEnterAttackRange?.Invoke(_nearestEnemyTeamEntity);
        }
    }

    private void CalculateNearestPlayerTeamEntity()
    {
        Entity[] playerTeamEntities = EntitiesManager.GetPlayerTeamEntities();

        // PERFORMANCE NOTE: instead of 'GetClosestComponent' use a kd tree
        _nearestPlayerTeamEntity = playerTeamEntities.Length == 0 ? null : transform.GetClosestComponent(playerTeamEntities).Object;

        if (_nearestPlayerTeamEntity != null && IsEntityInShiftRange(_nearestPlayerTeamEntity))
        {
            OnAllyEnterShiftRange?.Invoke(_nearestPlayerTeamEntity);
        }
    }
    #endregion
    #endregion
}
