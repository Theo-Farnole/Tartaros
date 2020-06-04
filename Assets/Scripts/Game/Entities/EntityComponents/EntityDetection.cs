using Lortedo.Utilities.Pattern;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public delegate void OnEntityDetected(Entity enemy);

/// <summary>
/// This script manage the detection of nearby entities.
/// </summary>
public class EntityDetection : EntityComponent, IPooledObject
{
    #region Fields
    private const string debugLogHeader = "Entity Detection : ";
    public readonly static float DISTANCE_THRESHOLD = 0.3f;

    public event OnEntityDetected OnEnemyDetected;
    public event OnEntityDetected OnEnemyLeaveDetection;
    public event OnEntityDetected OnAllyDetected;
    public event OnEntityDetected OnAllyLeaveDetection;

    [Required]
    [SerializeField] private GenericTrigger _viewTrigger;

    [Required]
    [SerializeField] private GenericTrigger _closeTrigger;

    private List<Entity> _enemiesInViewRadius = new List<Entity>();
    private List<Entity> _alliesInViewRadius = new List<Entity>();

    private List<Entity> _alliesInCloseRadius = new List<Entity>();
    private List<Entity> _enemiesInCloseRadius = new List<Entity>();

    string IPooledObject.ObjectTag { get; set; }
    #endregion

    #region Properties
    public List<Entity> AlliesInCloseRadius { get => _alliesInCloseRadius; }
    #endregion

    #region Methods
    #region MonoBehaviour Callbacks
    void OnEnable()
    {
        EnableViewRadius();
        EnableCloseRadius();

        SubscribeToEntityEvents();
    }
    void OnDisable()
    {
        DisableViewRadius();
        DisableCloseRadius();

        UnsubcribeToEntityEvents();
    }
    #endregion

    #region Events Subscriptions
    private void EnableViewRadius()
    {
        Assert.IsNotNull(_viewTrigger, "Please assign a _genericTrigger to " + name);

        _viewTrigger.enabled = true;
        _viewTrigger.SetCollisionRadius(Entity.Data.ViewRadius);
        _viewTrigger.OnTriggerEnterEvent += ViewTrigger_OnTriggerEnterEvent;
        _viewTrigger.OnTriggerExitEvent += ViewTrigger_OnTriggerExitEvent;
    }

    private void DisableViewRadius()
    {
        if (_viewTrigger != null)
        {
            _viewTrigger.enabled = false;
            _viewTrigger.OnTriggerEnterEvent -= ViewTrigger_OnTriggerEnterEvent;
            _viewTrigger.OnTriggerExitEvent -= ViewTrigger_OnTriggerExitEvent;
        }
    }

    private void EnableCloseRadius()
    {
        _closeTrigger.enabled = true;
        _closeTrigger.SetCollisionRadius(Mathf.Max(Entity.Data.TileSize.x, Entity.Data.TileSize.y));
        _closeTrigger.OnTriggerEnterEvent += CloseTrigger_OnTriggerEnterEvent;
        _closeTrigger.OnTriggerExitEvent += CloseTrigger_OnTriggerExitEvent;
    }

    private void DisableCloseRadius()
    {
        _closeTrigger.enabled = false;

    }

    private void UnsubcribeToEntityEvents()
    {
        Entity.OnTeamSwap -= Entity_OnTeamSwap;
        Entity.OnDeath -= Entity_OnDeath;
        Entity.OnSpawn -= Entity_OnSpawn;
    }

    private void SubscribeToEntityEvents()
    {
        Entity.OnTeamSwap += Entity_OnTeamSwap;
        Entity.OnDeath += Entity_OnDeath;
        Entity.OnSpawn += Entity_OnSpawn;
    }

    #endregion

    #region Events handlers
    private void ViewTrigger_OnTriggerEnterEvent(Collider other)
    {
        if (other.gameObject.TryGetComponent(out Entity entity))
        {
            AddEntityInViewRadiusList(entity);
        }
    }

    private void ViewTrigger_OnTriggerExitEvent(Collider other)
    {
        if (other.gameObject.TryGetComponent(out Entity entity))
        {
            RemoveEntityInViewRadiusList(entity);
        }
    }

    private void CloseTrigger_OnTriggerExitEvent(Collider other)
    {
        if (other.gameObject.TryGetComponent(out Entity entity))
        {
            RemoveEntityInCloseRadiusList(entity);
        }
    }

    private void CloseTrigger_OnTriggerEnterEvent(Collider other)
    {
        if (other.gameObject.TryGetComponent(out Entity entity))
        {
            AddEntityInCloseRadiusList(entity);
        }
    }

    private void Entity_OnTeamSwap(Entity entity, Team oldTeam, Team newTeam)
    {
        if (entity == Entity || _enemiesInViewRadius.Contains(entity))
        {
            ForceRefreshDetection();
        }
    }

    private void Entity_OnSpawn(Entity entity)
    {
#if DEVELOPMENT_BUILD || UNITY_EDITOR
        if (_enemiesInViewRadius.Contains(entity) || _enemiesInCloseRadius.Contains(entity) || _alliesInViewRadius.Contains(entity) || _alliesInCloseRadius.Contains(entity))
            Debug.LogErrorFormat("Entity {0} has spawn, but entity {1} already has it in its view radius. It's might lead to problems", entity.name, name);
#endif
    }

    private void Entity_OnDeath(Entity entity)
    {
        RemoveEntityInEveryList(entity);

        if (entity == Entity)
        {
            _enemiesInViewRadius.Clear();
        }
    }
    #endregion

    #region IPooledObject interface
    void IPooledObject.OnObjectSpawn()
    {
        ForceRefreshDetection();
    }
    #endregion

    #region Public methods
    public bool IsEntityInAttackRange(Entity target)
    {
        return Vector3.Distance(transform.position, target.transform.position) <= Entity.Data.AttackRadius + Mathf.Max(target.Data.TileSize.x, target.Data.TileSize.y);
    }

    public bool IsNearFromEntity(Entity target)
    {
        return Vector3.Distance(transform.position, target.transform.position) <= DISTANCE_THRESHOLD + Mathf.Max(target.Data.TileSize.x, target.Data.TileSize.y);
    }

    public bool IsNearFromPosition(Vector3 position)
    {
        return Vector3.Distance(transform.position, position) <= DISTANCE_THRESHOLD;
    }

    /// <summary>
    /// Use a OverlapSphere, then process the OverlapSphere output with 2 Linq queries.
    /// </summary>
    /// <returns></returns>
    public Entity[] GetAllEnemiesInViewRadius()
    {
        Assert.IsTrue(enabled, "EntityDetection '" + name + "'should be enable to get all enmies in view radius.");
        Assert.IsTrue(GetAlliesInEnemiesViewRadius() == 0, "There is allies in " + name + ".");

        return _enemiesInViewRadius.Where(x => x != null).ToArray();
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
    #endregion

    #region Private Methods
    private void ForceRefreshDetection()
    {
        _enemiesInViewRadius.Clear();

        float viewRadius = Entity.Data.ViewRadius;

        // cast an overlap sphere
        //Collider[] hitColliders = Physics.OverlapSphere(transform.position, viewRadius);
        var hitColliders = _viewTrigger.CollidersInTrigger;

        // and get every entity from it
        Entity[] hitEntities = hitColliders
            .Select(x => x.GetComponent<Entity>())
            .Where(x => x != null)
            .ToArray();

        // then, add each entity to list
        foreach (var hitEntity in hitEntities)
        {
            AddEntityInViewRadiusList(hitEntity);
        }
    }

    private void RemoveEntityInEveryList(Entity entity)
    {
        if (_enemiesInCloseRadius.Contains(entity)) _enemiesInCloseRadius.Remove(entity);
        if (_enemiesInViewRadius.Contains(entity)) _enemiesInViewRadius.Remove(entity);

        if (_alliesInCloseRadius.Contains(entity)) _alliesInCloseRadius.Remove(entity);
        if (_alliesInViewRadius.Contains(entity)) _alliesInViewRadius.Remove(entity);
    }

    private void AddEntityInCloseRadiusList(Entity entity)
    {
        if (_enemiesInCloseRadius.Contains(entity) || _alliesInCloseRadius.Contains(entity)) Debug.LogErrorFormat(debugLogHeader + "entity {0} is already in {1} list.", name, nameof(_enemiesInViewRadius));

        if (entity.Team != Entity.Team)
        {
            _enemiesInCloseRadius.Add(entity);
        }
        else
        {
            _alliesInCloseRadius.Add(entity);
        }
    }

    private void RemoveEntityInCloseRadiusList(Entity entity)
    {
        if (!_enemiesInCloseRadius.Contains(entity) && !_alliesInCloseRadius.Contains(entity)) Debug.LogErrorFormat(debugLogHeader + "To remove entity in list, it should be contained");

        if (entity.Team != Entity.Team)
        {
            _enemiesInCloseRadius.Remove(entity);
        }
        else
        {
            _alliesInCloseRadius.Remove(entity);
        }
    }

    private void AddEntityInViewRadiusList(Entity entity)
    {
        if (_enemiesInViewRadius.Contains(entity) || _alliesInViewRadius.Contains(entity)) Debug.LogErrorFormat(debugLogHeader + "entity {0} is already in {1} list.", name, nameof(_enemiesInViewRadius));

        if (entity.Team != Entity.Team)
        {
            _enemiesInViewRadius.Add(entity);
            OnEnemyDetected?.Invoke(entity);
        }
        else
        {
            _alliesInViewRadius.Add(entity);
            OnAllyDetected?.Invoke(entity);
        }
    }

    private void RemoveEntityInViewRadiusList(Entity entity)
    {
        if (!_enemiesInViewRadius.Contains(entity) && !_alliesInViewRadius.Contains(entity)) Debug.LogErrorFormat(debugLogHeader + "To remove entity in list, it should be contained");

        if (entity.Team != Entity.Team)
        {
            _enemiesInViewRadius.Remove(entity);
            OnEnemyLeaveDetection?.Invoke(entity);
        }
        else
        {
            _alliesInViewRadius.Remove(entity);
            OnAllyLeaveDetection?.Invoke(entity);
        }
    }

    /// <summary>
    /// Used in assertations. This should always returns 0.
    /// </summary>
    private int GetAlliesInEnemiesViewRadius()
    {
        return _enemiesInViewRadius.Where(x => x.Team == Entity.Team).Count();
    }
    #endregion
    #endregion
}
