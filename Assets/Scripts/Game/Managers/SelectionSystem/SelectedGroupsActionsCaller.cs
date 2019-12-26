using Game.Selection;
using LeonidasLegacy.IA.Action;
using Lortedo.Utilities.Pattern;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Wrapper used to call action to selected groups from others scripts.
/// </summary>
public static class SelectedGroupsActionsCaller
{
    /// <summary>
    /// Order attack to Spartan selected groups, except if target is also Spartan.
    /// </summary>
    /// <param name="target">Transform of the attack's target.</param>
    public static void OrderAttackUnit(Entity target)
    {
        if (target == null)
            return;

        foreach (SelectionManager.Group group in SelectionManager.Instance.SpartanGroups)
        {
            for (int i = 0; i < group.unitsSelected.Count; i++)
            {
                var actionAttack = new ActionAttackEntity(group.unitsSelected[i], target);

                group.unitsSelected[i].SetAction(actionAttack);
            }
        }
    }

    /// <summary>
    /// If Entity is Spartan, we move to destination. Else, we attack the entity.
    /// </summary>
    public static void OnEntityClick(Entity target)
    {
        if (target.Team == Team.Sparta)
        {
            OrderMoveToPosition(target.transform.position);
        }
        else
        {
            OrderAttackUnit(target);
        }
    }

    public static void OrderStop()
    {
        foreach (SelectionManager.Group group in SelectionManager.Instance.SpartanGroups)
        {
            for (int i = 0; i < group.unitsSelected.Count; i++)
            {
                group.unitsSelected[i].StopCurrentAction();
            }
        }
    }

    public static void OrderSpawnUnits(UnitType unit)
    {
        foreach (SelectionManager.Group group in SelectionManager.Instance.SpartanGroups)
        {
            for (int i = 0; i < group.unitsSelected.Count; i++)
            {
                group.unitsSelected[i].GetCharacterComponent<EntityUnitSpawner>().SpawnUnit(unit);
            }
        }
    }

    public static void OrderMoveToPosition(Vector3 destination)
    {
        foreach (SelectionManager.Group group in SelectionManager.Instance.SpartanGroups)
        {
            for (int j = 0; j < group.unitsSelected.Count; j++)
            {
                var actionMove = new ActionMoveToPosition(group.unitsSelected[j], destination);

                group.unitsSelected[j].SetAction(actionMove);
            }
        }
    }

    public static void OrderMoveAggressively(Vector3 destination)
    {
        foreach (SelectionManager.Group group in SelectionManager.Instance.SpartanGroups)
        {
            for (int j = 0; j < group.unitsSelected.Count; j++)
            {
                var actionMoveAggressively = new ActionMoveToPositionAggressively(group.unitsSelected[j], destination);

                group.unitsSelected[j].SetAction(actionMoveAggressively);
            }
        }
    }
}
