using Lortedo.Utilities.Pattern;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntitySelectable : EntityComponent
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
            ObjectPooler.Instance.EnqueueGameObject(ObjectPoolingTags.keySelectionCircle, _selectionCircle);
        }

        Game.Selection.SelectionManager.Instance.RemoveEntity(Entity);
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

        _selectionCircle = ObjectPooler.Instance.SpawnFromPool(ObjectPoolingTags.keySelectionCircle, pos, rot);
        
        if (_selectionCircle)
        {
        _selectionCircle.transform.parent = transform;
        _selectionCircle.GetComponent<SelectionCircle>().SetCircleOwner(Entity.Team);
        }
        else
        {
            Debug.LogErrorFormat("On entity selection: no selection circle pool found. Please check your ObjectPooler.");
        }
    }

    public void OnDeselect()
    {
        if (_selectionCircle != null)
        {
            ObjectPooler.Instance.EnqueueGameObject(ObjectPoolingTags.keySelectionCircle, _selectionCircle);
        }

        _selectionCircle = null;
    }
    #endregion
    #endregion
}
