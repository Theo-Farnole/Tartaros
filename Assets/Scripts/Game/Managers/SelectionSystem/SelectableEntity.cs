using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FogOfWar;

public class SelectableEntity : MonoBehaviour
{
    #region Fields
    private GameObject _selectionCircle = null;

    // cache variables
    private Entity _entity;
    #endregion

    #region Properties
    public EntityType Type { get => _entity.Type; }
    public Owner Owner  { get => _entity.owner; }

    public Entity Entity { get => _entity; }
    #endregion

    #region Methods
    #region MonoBehaviour Callbacks
    void Awake()
    {
        _entity = GetComponent<Entity>();
    }

    void OnDestroy()
    {
        if (GameManager.ApplicationIsQuitting)
            return;

        if (_selectionCircle != null)
        {
            ObjectPooler.Instance.EnqueueGameObject("selection_circle", _selectionCircle);
        }

        SelectionManager.Instance.RemoveEntity(this);
    }
    #endregion

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
        _selectionCircle.GetComponent<SelectionCircle>().SetCircleOwner(_entity.owner);
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
}
