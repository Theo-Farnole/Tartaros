using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionCircle : MonoBehaviour
{
    #region Fields
    [EnumNamedArray(typeof(Owner))]
    [SerializeField] private Material[] _materials;

    private Projector _projector;
    #endregion

    #region Methods
    void Awake()
    {
        _projector = GetComponent<Projector>();
    }

    public void SetCircleOwner(Owner owner)
    {
        _projector.material = _materials[(int)owner];
    }

    // force array to be the size of TEnum
    void OnValidate()
    {
        Array.Resize(ref _materials, Enum.GetValues(typeof(Owner)).Length);
    }
    #endregion
}
