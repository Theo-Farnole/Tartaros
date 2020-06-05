using Game.IA.Action;
using Lortedo.Utilities.Pattern;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// This script manage the attack (smart classname isn't it ?).
/// </summary>
public class EntityAttack : EntityComponent
{
    #region Fields
    private float _attackTime = 0;
    #endregion

    #region Methods
    #region Mono Callbacks
    void Awake()
    {
        CalculateNewAttackTimer();
    }

    void OnEnable()
    {
        Entity.GetCharacterComponent<EntityDetection>().OnOpponentEnterAttackRange += EntityAttack_OnEnemyEnterAttackRange;
    }

    void OnDisable()
    {
        Entity.GetCharacterComponent<EntityDetection>().OnOpponentEnterAttackRange += EntityAttack_OnEnemyEnterAttackRange;
    }
    #endregion

    #region Events Handlers
    private void EntityAttack_OnEnemyEnterAttackRange(Entity enemy)
    {
        if (Entity.IsIdle)
        {
            // auto attack the enemy
            var action = new ActionAttackEntity(Entity, enemy);
            Entity.SetAction(action);
        }
    }
    #endregion

    #region Public methods
    public void DoAttack(Entity target)
    {
        if (!Entity.Data.CanAttack)
            return;

        Entity.GetCharacterComponent<EntityMovement>().SetAvoidance(Avoidance.Fight);

        if (Time.time < _attackTime)
            return;

        if (Entity.GetCharacterComponent<EntityDetection>().IsEntityInAttackRange(target))
        {
            CalculateNewAttackTimer();

            if (Entity.Data.IsMelee)
            {
                target.GetCharacterComponent<EntityHealth>().GetDamage(Entity.Data.Damage, Entity);
            }
            else
            {
                // prefab project not null
                Assert.IsNotNull(Entity.Data.PrefabProjectile, string.Format("Entity Attack : Projectile set in EntityData is null for {0} of type {1}", name, Entity.EntityID));

                GameObject gameObjectProjectile = ObjectPooler.Instance.SpawnFromPool(Entity.Data.PrefabProjectile, transform.position, Quaternion.identity, true);

                // projectile from pool not null
                Assert.IsNotNull(gameObjectProjectile, string.Format("Entity Attack : Projectile '{0}' from object pooler is null.", Entity.Data.PrefabProjectile.name));

                Projectile projectile = gameObjectProjectile.GetComponent<Projectile>();

                // projectile from pool has Projectile component
                Assert.IsNotNull(string.Format("Prefab projectile of {0} is missing Projectile component. Please, add one.", name));

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

        var nearestEnemy = Entity.GetCharacterComponent<EntityDetection>().GetNearestOpponentInViewRadius();

        // enemy found
        if (nearestEnemy != null)
        {            
            var action = new ActionAttackEntity(Entity, nearestEnemy);
            Entity.SetAction(action);

            return true;
        }

        return false;
    }
    #endregion

    #region Private methods
    void CalculateNewAttackTimer()
    {
        _attackTime = Time.time + 1 / Entity.Data.AttackPerSecond;
    }
    #endregion
    #endregion
}
