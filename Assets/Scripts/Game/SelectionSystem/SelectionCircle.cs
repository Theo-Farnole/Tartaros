using Game.Entities;
using Lortedo.Utilities.Inspector;
using Lortedo.Utilities.Pattern;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionCircle : MonoBehaviour, IPooledObject
{
    #region Fields
    [EnumNamedArray(typeof(Team))]
    [SerializeField] private Material[] _materials;

    private Projector _projector;

    public string ObjectTag { get; set; }
    #endregion

    #region Methods
    #region Mono Callbacks
    void Awake()
    {
        _projector = GetComponent<Projector>();
    }

    // force array to be the size of TEnum
    void OnValidate()
    {
        Array.Resize(ref _materials, Enum.GetValues(typeof(Team)).Length);
    }
    #endregion

    #region Public methods
    public void SetCircleOwner(Team owner)
    {
        _projector.material = _materials[(int)owner];
    }
    
    public void OnObjectSpawn()
    {
        _projector.enabled = true;
    }
    #endregion
    #endregion
}
