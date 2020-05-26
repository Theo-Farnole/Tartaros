using Lortedo.Utilities.Pattern;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

public delegate void OnEnemyDetected(Entity enemy);

public class EntityDetection : EntityComponent
{
    #region Fields
    private const string debugLogHeader = "Entity Detection : ";
    public readonly static float DISTANCE_THRESHOLD = 0.3f;

    public event OnEnemyDetected OnEnemyDetected;

    [Required]
    [SerializeField] private GenericTrigger _viewTrigger;

    private List<Entity> _enemiesInViewRadius = new List<Entity>();

    private bool _isFirstEnable = true;
    #endregion

    #region Methods
    #region MonoBehaviour Callbacks
    void OnEnable()
    {
        _enemiesInViewRadius.Clear();
        Entity.OnTeamSwap += Entity_OnTeamSwap;

        EnableViewRadius();

        if (!_isFirstEnable)
        {
            ForceRefreshDetection();
            _isFirstEnable = false;
        }
    }

    void OnDisable()
    {
        if (_viewTrigger != null)
        {
            _viewTrigger.enabled = false;
            _viewTrigger.OnTriggerEnterEvent -= GenericTrigger_OnTriggerEnterEvent;
            _viewTrigger.OnTriggerExitEvent -= GenericTrigger_OnTriggerExitEvent;
        }

        Entity.OnTeamSwap -= Entity_OnTeamSwap;

        _enemiesInViewRadius.Clear(); 
    }
    #endregion

    #region Events handlers
    private void GenericTrigger_OnTriggerEnterEvent(Collider other)
    {
        if (other.gameObject.TryGetComponent(out Entity entity))
        {
            AddEntitiesInList(entity);
        }
    }

    private void GenericTrigger_OnTriggerExitEvent(Collider other)
    {
        if (other.gameObject.TryGetComponent(out Entity entity))
        {
            RemoveEntityInList(entity);
        }
    }

    private void Entity_OnTeamSwap(Entity entity, Team oldTeam, Team newTeam)
    {
        if (entity == Entity)
        {
            ForceRefreshDetection();
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

    private void ForceRefreshDetection()
    {
        _enemiesInViewRadius.Clear();

        float viewRadius = Entity.Data.ViewRadius;

        // cast an overlap sphere
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, viewRadius);

        // and get every entity from it
        Entity[] hitEntities = hitColliders
            .Select(x => x.GetComponent<Entity>())
            .Where(x => x != null)
            .ToArray();            

        // then, add each entity to list
        foreach (var hitEntity in hitEntities)
        {
            AddEntitiesInList(hitEntity);
        }
    }

    private void AddEntitiesInList(Entity entity)
    {
        Assert.IsFalse(_enemiesInViewRadius.Contains(entity), string.Format(debugLogHeader + "entity {0} is already in {1} list.", name, nameof(_enemiesInViewRadius)));

        if (entity.Team != Entity.Team)
        {
            _enemiesInViewRadius.Add(entity);

            OnEnemyDetected?.Invoke(entity);
        }
    }

    private void RemoveEntityInList(Entity entity)
    {
        if (entity.Team != Entity.Team)
        {
            Assert.IsTrue(_enemiesInViewRadius.Contains(entity), "To remove entity in list, it should be contained");

            _enemiesInViewRadius.Remove(entity);
        }
    }
    #endregion
    #endregion
}
