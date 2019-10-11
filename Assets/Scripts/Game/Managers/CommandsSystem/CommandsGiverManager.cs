using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CommandsGiverManager : Singleton<CommandsGiverManager>
{
    #region Methods
    #region MonoBehaviour callbacks
    void Update()
    {
        ManageCommandsExecuter();
    }
    #endregion

    #region Commands Executer
    /// <summary>
    /// If right click pressed, order attack or movement to Spartan selected groups.
    /// </summary>
    void ManageCommandsExecuter()
    {
        if (Input.GetMouseButton(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, LayerMask.GetMask("Entity")))
            {
                OrderAttack(hit.transform);
            }
            else if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Grid")))
            {
                OrderMovement(hit.point);
            }
        }
    }

    /// <summary>
    /// Order attack to Spartan selected groups, except if target is also Spartan.
    /// </summary>
    /// <param name="target">Transform of the attack's target.</param>
    public void OrderAttack(Transform target)
    {
        if (target.GetComponent<Entity>().Owner == Owner.Sparta)
            return;
        
        foreach (SelectionManager.Group group in SelectionManager.Instance.SpartanGroups)
        {
            for (int i = 0; i < group.selectedEntities.Count; i++)
            {
                group.selectedEntities[i].Entity.CommandReceiver.Attack(target);
            }
        }
    }

    public void OrderStop()
    {
        foreach (SelectionManager.Group group in SelectionManager.Instance.SpartanGroups)
        {
            for (int i = 0; i < group.selectedEntities.Count; i++)
            {
                group.selectedEntities[i].Entity.CommandReceiver.Stop();
            }
        }
    }

    public void OrderSpawnUnits(Unit unit)
    {
        foreach (SelectionManager.Group group in SelectionManager.Instance.SpartanGroups)
        {
            for (int i = 0; i < group.selectedEntities.Count; i++)
            {
                group.selectedEntities[i].Entity.CommandReceiver.SpawnUnit(unit);
            }
        }
    }

    /// <summary>
    /// Order movement to Spartan selected groups.
    /// </summary>
    /// <param name="destination">Position of the wanted destination</param>
    public void OrderMovement(Vector3 destination)
    {
        foreach (SelectionManager.Group group in SelectionManager.Instance.SpartanGroups)
        {
            for (int j = 0; j < group.selectedEntities.Count; j++)
            {
                group.selectedEntities[j].Entity.CommandReceiver.Move(destination);
            }
        }
    }
    #endregion
    #endregion
}
