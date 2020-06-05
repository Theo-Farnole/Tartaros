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

    public readonly static int frameIntervalToCheckNearestEntities = 5;

    public event OnEntityDetected OnAllyEnterShiftRange;
    public event OnEntityDetected OnOpponentEnterAttackRange;

    private Entity _nearestOpponentTeamEntity = null;
    private Entity _nearestAllyTeamEntity = null;

    public int _frameOffset = -1;
    #endregion

    #region Methods
    #region MonoBehaviour Callbacks   
    void Awake()
    {
        _frameOffset = Random.Range(0, frameIntervalToCheckNearestEntities);
    }

    void Start()
    {
        EntitiesManager.Initialize();
    }

    void Update()
    {
        // avoid calculation every frame
        if (Time.frameCount - _frameOffset % frameIntervalToCheckNearestEntities == 0)
        {
            CalculateNearestAllyTeamEntity();
            CalculateNearestOpponentTeamEntity();
        }
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

    public Entity GetNearestOpponentInViewRadius()
    {
        return _nearestOpponentTeamEntity;
    }

    public Entity GetNearestAllyInViewRadius()
    {
        return _nearestAllyTeamEntity;
    }

    public bool IsEntityInViewRadius(Entity target)
    {
        return Vector3.Distance(transform.position, target.transform.position) <= Entity.Data.ViewRadius;
    }
    #endregion

    #region Private Methods
    private void CalculateNearestOpponentTeamEntity()
    {
        _nearestOpponentTeamEntity = EntitiesManager.GetClosestOpponentEntity(transform.position, Entity.Team);

        if (_nearestOpponentTeamEntity != null && IsEntityInAttackRange(_nearestOpponentTeamEntity))
        {
            OnOpponentEnterAttackRange?.Invoke(_nearestOpponentTeamEntity);
        }
    }

    private void CalculateNearestAllyTeamEntity()
    {
        _nearestAllyTeamEntity = EntitiesManager.GetClosestAllyEntity(transform.position, Entity.Team);

        if (_nearestAllyTeamEntity != null && IsEntityInShiftRange(_nearestAllyTeamEntity))
        {
            OnAllyEnterShiftRange?.Invoke(_nearestAllyTeamEntity);
        }
    }
    #endregion
    #endregion
}
