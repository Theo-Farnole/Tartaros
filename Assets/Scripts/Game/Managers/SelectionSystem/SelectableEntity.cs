using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectableEntity : MonoBehaviour
{
    #region Fields
    [SerializeField] private EntityType _type;

    private GameObject _selectionCircle = null;

    // cache variables
    private OwnedEntity _ownedEntity;
    private CommandReceiverEntity _commandReceiverEntity;
    #endregion

    #region Properties
    public EntityType Type { get => _type; }
    public Owner Owner
    {
        get
        {
            if (_ownedEntity == null)
            {
                Debug.LogWarning("SelectableEntity doesn't own a OwnedEntity!");
            }

            return _ownedEntity.Owner;
        }
    }

    public CommandReceiverEntity CommandReceiverEntity { get => _commandReceiverEntity; }
    #endregion

    #region Methods
    #region MonoBehaviour Callbacks
    void Awake()
    {
        _ownedEntity = GetComponent<OwnedEntity>();
        _commandReceiverEntity = GetComponent<CommandReceiverEntity>();
    }

    void OnMouseDown()
    {
        SelectionManager.Instance.AddEntity(this);
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
        if (_selectionCircle == null)
        {
            Vector3 pos = transform.position + Vector3.up * 0.78f;
            Quaternion rot = Quaternion.Euler(90, 0, 0);

            _selectionCircle = ObjectPooler.Instance.SpawnFromPool("selection_circle", pos, rot);
            _selectionCircle.transform.parent = transform;
            _selectionCircle.GetComponent<SelectionCircle>().SetCircleOwner(_ownedEntity.Owner);
        }
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
