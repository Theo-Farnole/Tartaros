using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectableEntity : MonoBehaviour
{
    #region Fields

    [SerializeField] private Entity _type;

    private GameObject _selectionCircle = null;
    private MoveCommand _moveCommand;
    #endregion

    #region Properties
    public Entity Type { get => _type; }
    public MoveCommand MoveCommand { get => _moveCommand; }
    #endregion

    #region Methods
    #region MonoBehaviour Callbacks
    void Awake()
    {
        if (_type.IsUnitType() != null)
        {
            _moveCommand = new MoveCommand(gameObject);
        }
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
