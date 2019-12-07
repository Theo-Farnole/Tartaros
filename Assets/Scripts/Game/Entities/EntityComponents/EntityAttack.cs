using LeonidasLegacy.IA.Action;
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

        if (_attackTimer < target.Data.AttackSpeed)
            return;

        if (Vector3.Distance(transform.position, target.transform.position) <= target.Data.AttackRadius)
        {
            _attackTimer = 0;

            target.GetCharacterComponent<EntityHealth>().GetDamage(Entity.Data.Damage, Entity);
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
