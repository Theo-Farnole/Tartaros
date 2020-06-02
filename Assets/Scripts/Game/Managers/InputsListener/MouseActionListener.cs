using Game.Selection;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>
/// If mouse right click pressed, order attack or movement to Spartan selected groups.
/// </summary>
public class MouseActionListener : MonoBehaviour
{
    public static readonly string debugLogHeader = "Mouse Action Listener : ";

    [Required]
    [SerializeField] private GameObject _prefabMoveToOrderFeedback;
    [SerializeField] private Vector3 _orderMoveToInstanciatedOffset = Vector3.up;

    private GameObject _onclick;

    void Update()
    {
        ManageOrdersExecuter();
    }

    void ManageOrdersExecuter()
    {
        if (SecondClickListener.Instance.ListenToClick)
            return;

        if (Input.GetMouseButtonDown(1))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            
            if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Entity")))
            {
                SelectedGroupsActionsCaller.OnEntityClick(hit.transform.GetComponent<Entity>());
            }
            else if (Physics.Raycast(ray, out hit, Mathf.Infinity, LayerMask.GetMask("Terrain")))
            {
                SelectedGroupsActionsCaller.OrderMoveToPosition(hit.point);

                if (SelectionManager.Instance.HasSelection)
                {
                    DisplayMoveToOrderFeedback(hit.point);
                }
            }
        }
    }

    void DisplayMoveToOrderFeedback(Vector3 position)
    {
        if (_onclick == null)
            InstanciateMoveToOrderFeedback();

        _onclick.SetActive(true);
        _onclick.transform.position = position + _orderMoveToInstanciatedOffset;
        _onclick.GetComponent<ParticleSystem>().Play(true);
    }

    void InstanciateMoveToOrderFeedback()
    {
        if (_onclick != null)
        {
            Debug.LogWarningFormat(debugLogHeader + "Can't instanciate prefab on click because it's already instanciated.");
            return;
        }

        Assert.IsNotNull(_prefabMoveToOrderFeedback);

        _onclick = Instantiate(_prefabMoveToOrderFeedback);
        _onclick.SetActive(false);
    }
}
