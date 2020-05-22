using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public class EntityDetection : EntityComponent
{
    private const string debugLogHeader = "Entity Detection : ";
    public readonly static float DISTANCE_THRESHOLD = 0.3f;

    [Required]
    [SerializeField] private GenericTrigger _viewTrigger;

    private List<Entity> _enemiesInViewRadius = new List<Entity>();

    #region Methods
    #region MonoBehaviour Callbacks
    void OnEnable()
    {
        if (_viewTrigger != null)
        {
            _viewTrigger.enabled = true;
            _viewTrigger.OnTriggerEnterEvent += GenericTrigger_OnTriggerEnterEvent;
            _viewTrigger.OnTriggerExitEvent += GenericTrigger_OnTriggerExitEvent;
        }
        else
        {
            Debug.LogErrorFormat(debugLogHeader + "Please assign a _genericTrigger to {0}", name);
        }

        _enemiesInViewRadius.Clear();
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
    private void GenericTrigger_OnTriggerExitEvent(Collider other)
    {
        if (other.gameObject.TryGetComponent(out Entity entity))
        {
            if (entity.Team != Entity.Team)
            {
                Assert.IsFalse(_enemiesInViewRadius.Contains(entity), string.Format(debugLogHeader + "entity {0} is already in {1} list.", name, nameof(_enemiesInViewRadius)));
                _enemiesInViewRadius.Add(entity);
            }
        }
    }

    private void GenericTrigger_OnTriggerEnterEvent(Collider other)
    {
        if (other.gameObject.TryGetComponent(out Entity entity))
        {
            // the enemy shoulddn't has the same team of the Entity
            Assert.IsFalse(_enemiesInViewRadius.Contains(entity) && entity.Team == Entity.Team,
                string.Format(debugLogHeader + "The entity {0} in {1} has the same team of {2}'s entity detection.", entity.name, nameof(_enemiesInViewRadius), name));

            if (entity.Team != Entity.Team && _enemiesInViewRadius.Contains(entity))
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
        if (!enabled)
        {
            Debug.LogWarningFormat(debugLogHeader + " {0} is disabled. Can't get all enemies in view radius.");
            return null;
        }

        return _enemiesInViewRadius.ToArray();
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
    #endregion
}
