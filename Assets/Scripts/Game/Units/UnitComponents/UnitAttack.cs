using LeonidasLegacy.IA.Action;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAttack : UnitComponent
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
            ManageAutoAttack();
        }
    }
    #endregion

    #region Public methods
    public void DoAttack(Unit target)
    {
        if (!UnitManager.Data.CanAttack)
            return;

        if (_attackTimer < target.Data.AttackSpeed)
            return;

        if (Vector3.Distance(transform.position, target.transform.position) <= target.Data.AttackRadius)
        {
            _attackTimer = 0;

            target.GetCharacterComponent<UnitHealth>().GetDamage(UnitManager.Data.Damage, UnitManager);
        }
    }

    /// <summary>
    /// If an enemy is visible, set Unit action to ActionAttackUnit.
    /// </summary>
    public void StartActionAttackNearestEnemy()
    {
        var nearestEnemy = UnitManager.GetCharacterComponent<UnitDetection>().GetNearestEnemyInViewRadius();

        if (nearestEnemy != null)
        {
            var actionAttackUnit = new ActionAttackUnit(UnitManager, nearestEnemy);
            UnitManager.SetAction(actionAttackUnit);
        }
    }
    #endregion

    #region Private methods
    /// <summary>
    /// Attack neareast enemy if Unit hasn't current action
    /// </summary>
    void ManageAutoAttack()
    {
        // only auto attact if Unit hasn't current action
        if (!UnitManager.HasCurrentAction)
        {
            StartActionAttackNearestEnemy();
        }
    }
    #endregion
    #endregion
}
