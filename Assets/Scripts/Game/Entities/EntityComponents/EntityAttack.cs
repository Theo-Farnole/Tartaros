using LeonidasLegacy.IA.Action;
using Lortedo.Utilities.Pattern;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityAttack : EntityComponent
{
    #region Fields
    private float _attackTimer = 0;
    #endregion

    #region Methods
    #region Mono Callbacks
    void Update()
    {
        _attackTimer += Time.deltaTime;

        // if Entity is idling, try to attack nearest enemy
        if (!Entity.HasCurrentAction)
        {
            TryStartActionAttackNearestEnemy();
        }
    }
    #endregion

    #region Public methods
    public void DoAttack(Entity target)
    {
        if (!Entity.Data.CanAttack)
            return;

        Entity.GetCharacterComponent<EntityMovement>().SetAvoidance(Avoidance.Fight);

        if (_attackTimer < Entity.Data.AttackSpeed)
            return;

        if (Entity.GetCharacterComponent<EntityDetection>().IsEntityInAttackRange(target))
        {
            _attackTimer = 0;

            if (Entity.Data.IsMelee)
            {
                target.GetCharacterComponent<EntityHealth>().GetDamage(Entity.Data.Damage, Entity);
            }
            else
            {
                Projectile projectile = ObjectPooler.Instance.SpawnFromPool(Entity.Data.PrefabProjectile, transform.position, Quaternion.identity, true).GetComponent<Projectile>();
                projectile.Throw(target, Entity);
            }
        }
    }

    /// <summary>
    /// If an enemy is visible, set Entity action to ActionAttackEntity.
    /// </summary>
    /// <returns>Return true if an enemy has been founded</returns>
    public bool TryStartActionAttackNearestEnemy()
    {
        if (!Entity.Data.CanAttack)
            return false;

        var nearestEnemy = Entity.GetCharacterComponent<EntityDetection>().GetNearestEnemyInViewRadius();
        
        if (nearestEnemy != null)
        {
            var action = new ActionAttackEntity(Entity, nearestEnemy);
            Entity.SetAction(action);

            return true;
        }

        return false;
    }
    #endregion
    #endregion
}
