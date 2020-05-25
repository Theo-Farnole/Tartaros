using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public class EntityDetection : EntityComponent
{
    #region Fields
    private const string debugLogHeader = "Entity Detection : ";
    public readonly static float DISTANCE_THRESHOLD = 0.3f;

    [Required]
    [SerializeField] private GenericTrigger _viewTrigger;

    private List<Entity> _enemiesInViewRadius = new List<Entity>();
    #endregion

    #region Methods
    #region MonoBehaviour Callbacks
    void OnEnable()
    {
        _enemiesInViewRadius.Clear();

        EnableViewRadius();
    }

    void OnDisable()
    {
        if (_viewTrigger != null)
        {
            _viewTrigger.enabled = false;
            _viewTrigger.OnTriggerEnterEvent -= GenericTrigger_OnTriggerEnterEvent;
            _viewTrigger.OnTriggerExitEvent -= GenericTrigger_OnTriggerExitEvent;
        }
    }
    #endregion

    #region Events handlers
    private void GenericTrigger_OnTriggerEnterEvent(Collider other)
    {
        if (other.gameObject.TryGetComponent(out Entity entity))
        {
            Assert.IsFalse(_enemiesInViewRadius.Contains(entity),
                string.Format(debugLogHeader + "entity {0} is already in {1} list.", name, nameof(_enemiesInViewRadius)));

            if (entity.Team != Entity.Team)
            {
                _enemiesInViewRadius.Add(entity);
            }
        }
    }

    private void GenericTrigger_OnTriggerExitEvent(Collider other)
    {
        if (other.gameObject.TryGetComponent(out Entity entity))
        {
            if (entity.Team != Entity.Team)
            {
                _enemiesInViewRadius.Remove(entity);
            }
        }
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
    private void EnableViewRadius()
    {
        Assert.IsNotNull(_viewTrigger, "Please assign a _genericTrigger to " + name);

        _viewTrigger.enabled = true;
        _viewTrigger.SetCollisionRadius(Entity.Data.ViewRadius);
        _viewTrigger.OnTriggerEnterEvent += GenericTrigger_OnTriggerEnterEvent;
        _viewTrigger.OnTriggerExitEvent += GenericTrigger_OnTriggerExitEvent;
    }
    #endregion
    #endregion
}
