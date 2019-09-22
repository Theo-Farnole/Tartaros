using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectableEntity : MonoBehaviour
{
    #region Fields
    [SerializeField] private Entity _type;

    private GameObject _selectionCircle = null;
    private MovableEntity _movableEntity;
    private AttackerEntity _attackEntity;
    #endregion

    #region Properties
    public Entity Type { get => _type; }
    public MovableEntity MovableEntity { get => _movableEntity; }
    public AttackerEntity AttackEntity { get => _attackEntity; }
    #endregion

    #region Methods
    #region MonoBehaviour Callbacks
    void Awake()
    {
        _movableEntity = GetComponent<MovableEntity>();
        _attackEntity = GetComponent<AttackerEntity>();
    }

    void OnMouseDown()
    {
        SelectionManager.Instance.AddEntity(_type, this);
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
        }
    }

    public void OnDeselect()
    {
        if (_selectionCircle != null)
        {
            ObjectPooler.Instance.EnqueueGameObject("selection_circle", _selectionCircle);
            _selectionCircle = null;
        }
    }
    #endregion
}
