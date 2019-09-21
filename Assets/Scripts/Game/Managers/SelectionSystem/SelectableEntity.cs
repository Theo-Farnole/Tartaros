using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectableEntity : MonoBehaviour
{
    #region Fields
    [SerializeField] private Entity _entityType;
    #endregion

    #region Methods
    void OnMouseDown()
    {
        Debug.Log("OnMouseDown");
        SelectionManager.Instance.AddEntity(_entityType, gameObject);
    }

    void OnMouseUp()
    {
        Debug.Log("OnMouseUp");
        SelectionManager.Instance.RemoveEntity(_entityType, gameObject);
    }
    #endregion
}
