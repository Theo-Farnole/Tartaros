using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// If mouse right click pressed, order attack or movement to Spartan selected groups.
/// </summary>
public class MouseActionListener : MonoBehaviour
{
    void Update()
    {
        ManageOrdersExecuter();
    }

    void ManageOrdersExecuter()
    {
        if (Input.GetMouseButton(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Entity")))
            {
                CallActionsToSelectedGroups.OrderAttackUnit(hit.transform.GetComponent<Entity>());
            }
            else if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Terrain")))
            {
                CallActionsToSelectedGroups.OrderMoveToPosition(hit.point);
            }
        }
    }
}
