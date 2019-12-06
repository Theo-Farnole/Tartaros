using Lortedo.Utilities.Pattern;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSelectable : UnitComponent
{
    #region Fields
    private GameObject _selectionCircle = null;
    #endregion

    #region Methods
    #region Mono Callbacks
    void OnDestroy()
    {
        if (GameManager.ApplicationIsQuitting)
            return;

        if (_selectionCircle != null)
        {
            ObjectPooler.Instance.EnqueueGameObject("selection_circle", _selectionCircle);
        }

        SelectionManager.Instance.RemoveEntity(UnitManager);
    }
    #endregion

    #region Public methods
    public void OnSelected()
    {
        if (_selectionCircle != null)
        {
            Debug.LogWarning("SelectionCircle is already existing");
            return;
        }

        Vector3 pos = transform.position + Vector3.up * 0.78f;
        Quaternion rot = Quaternion.Euler(90, 0, 0);

        _selectionCircle = ObjectPooler.Instance.SpawnFromPool("selection_circle", pos, rot);
        _selectionCircle.transform.parent = transform;
        _selectionCircle.GetComponent<SelectionCircle>().SetCircleOwner(UnitManager.Team);
    }

    public void OnDeselect()
    {
        if (_selectionCircle != null)
        {
            ObjectPooler.Instance.EnqueueGameObject("selection_circle", _selectionCircle);
        }

        _selectionCircle = null;
    }
    #endregion
    #endregion
}
