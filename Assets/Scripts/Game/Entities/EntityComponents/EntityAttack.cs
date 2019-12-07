using LeonidasLegacy.IA.Action;
using Lortedo.Utilities.Pattern;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityAttack : EntityComponent
{
    #region Fields
    public readonly int MAX_ALLOCATED_FRAME_CALCULATION = 3;

    private float _attackTimer = 0;
    private int _allocatedFrameToCalculation = 1; // eg. calculate auto attack each X frames
    #endregion

    #region Methods
    #region Mono Callbacks
    void Start()
    {
        _allocatedFrameToCalculation = Random.Range(1, MAX_ALLOCATED_FRAME_CALCULATION);
    }

    void Update()
    {
        _attackTimer += Time.deltaTime;

        if (Time.frameCount % _allocatedFrameToCalculation == 0)
        {
            //ManageAutoAttack();
        }
    }
    #endregion

    #region Public methods
    public void DoAttack(Entity target)
    {
        if (!Entity.Data.CanAttack)
            return;

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
                Projectile projectile = ObjectPooler.Instance.SpawnFromPool(Entity.Data.PrefabProjectile, transform.position, Quaternion.identity).GetComponent<Projectile>();
                projectile.Throw(target.transform, Entity);
            }
        }
    }

    /// <summary>
    /// If an enemy is visible, set Entity action to ActionAttackEntity.
    /// </summary>
    public void StartActionAttackNearestEnemy()
    {
        var nearestEnemy = Entity.GetCharacterComponent<EntityDetection>().GetNearestEnemyInViewRadius();

        if (nearestEnemy != null)
        {
            var action = new ActionAttackEntity(Entity, nearestEnemy);
            Entity.SetAction(action);
        }
    }
    #endregion

    #region Private methods
    /// <summary>
    /// Attack neareast enemy if Entity hasn't current action
    /// </summary>
    void ManageAutoAttack()
    {
        // only auto attact if Entity hasn't current action
        if (!Entity.HasCurrentAction)
        {
            StartActionAttackNearestEnemy();
        }
    }
    #endregion
    #endregion
}
