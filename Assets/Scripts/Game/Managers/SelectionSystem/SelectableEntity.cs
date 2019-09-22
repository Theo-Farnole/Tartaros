using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectableEntity : MonoBehaviour
{
    #region Fields
    [SerializeField] private Entity _type;

    private MoveCommand _moveCommand;
    #endregion

    #region Properties
    public Entity Type { get => _type; }
    public MoveCommand MoveCommand { get => _moveCommand; }
    #endregion

    #region Methods
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
}
