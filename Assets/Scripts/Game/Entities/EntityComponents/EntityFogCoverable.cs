using Game.FogOfWar;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityFogCoverable : EntityComponent
{
    [SerializeField] private GameObject _rootModelRenderer;
    private Collider _collider;

    private bool _isCover = false;

    public bool IsCover
    {
        get => _isCover;

        set
        {
            _isCover = value;

            _rootModelRenderer.SetActive(!value);
            _collider.enabled = !value;
        }
    }

    private void Awake()
    {
        _collider = GetComponent<Collider>();
    }

    void Start()
    {
        FOWManager.Instance.AddCoverable(this);    
    }

    void OnDestroy()
    {
        FOWManager.Instance?.RemoveCoverable(this);    
    }
}
