using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LeonidasLegacy.IA.Action
{
    public class ActionAttackUnit : Action
    {
        Unit _target;

        public ActionAttackUnit(Unit owner, Unit target) : base(owner)
        {
            _target = target;
        }

        public override void OnStateExit()
        {
            unitManager.GetCharacterComponent<UnitMovement>().StopMoving();
        }

        public override void Tick()
        {
            // if target has been killed
            if (_target == null)
            {
                unitManager.GetCharacterComponent<UnitMovement>().StopMoving();
                unitManager.StopCurrentAction();
                return;
            }

            UnitMovement unitMovement = unitManager.GetCharacterComponent<UnitMovement>();

            // Can unit attack target?
            if (unitMovement.IsUnitInAttackRange(_target))
            {
                unitMovement.StopMoving();

                UnitAttack unitAttack = unitManager.GetCharacterComponent<UnitAttack>();
                unitAttack.DoAttack(_target);
            }
            else // Otherwise, unit go closer.
            {
                unitMovement.MoveToUnit(_target);
            }
        }
    }
}
