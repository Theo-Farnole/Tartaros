using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectableEntity : MonoBehaviour
{
    #region Fields
    [SerializeField] private EntityType _type;

    private GameObject _selectionCircle = null;

    // cache variables
    private Entity _entity;
    private CommandsReceiverEntity _commandReceiverEntity;
    #endregion

    #region Properties
    public EntityType Type { get => _type; }
    public Owner Owner
    {
        get
        {
            if (_entity == null)
            {
                Debug.LogWarning("SelectableEntity doesn't own a OwnedEntity!");
            }

            return _entity.Owner;
        }
    }

    public CommandsReceiverEntity CommandReceiverEntity { get => _commandReceiverEntity; }
    #endregion

    #region Methods
    #region MonoBehaviour Callbacks
    void Awake()
    {
        _entity = GetComponent<Entity>();
        _commandReceiverEntity = GetComponent<CommandsReceiverEntity>();
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
        _selectionCircle.GetComponent<SelectionCircle>().SetCircleOwner(_entity.Owner);
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
