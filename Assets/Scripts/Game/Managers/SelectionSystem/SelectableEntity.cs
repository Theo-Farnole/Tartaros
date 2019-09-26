using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectableEntity : MonoBehaviour
{
    #region Fields
    [SerializeField] private EntityType _type;

    private GameObject _selectionCircle = null;
    private MovableEntity _movableEntity;
    private AttackerEntity _attackEntity;
    private OwnedEntity _ownedEntity;
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
    public MovableEntity MovableEntity { get => _movableEntity; }
    public AttackerEntity AttackEntity { get => _attackEntity; }
    #endregion

    #region Methods
    #region MonoBehaviour Callbacks
    void Awake()
    {
        _movableEntity = GetComponent<MovableEntity>();
        _attackEntity = GetComponent<AttackerEntity>();
        _ownedEntity = GetComponent<OwnedEntity>();
    }

    void OnMouseDown()
    {
        SelectionManager.Instance.AddEntity(this);
    }
     
    void OnDestroy()
    {
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
