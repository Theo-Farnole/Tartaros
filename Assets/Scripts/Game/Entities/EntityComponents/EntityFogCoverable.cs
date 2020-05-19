using Game.FogOfWar;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void OnFog(IFogCoverable fogCoverable);


public class EntityFogCoverable : EntityComponent, IFogCoverable
{
    public event OnFog OnFogCover;
    public event OnFog OnFogUncover;

    private const string debugLogHeader = "Entity Fog Coverable : ";
    [SerializeField] private GameObject _rootModelRenderer;
    private Collider _collider;

    private bool _isCover = false;

    public bool IsCover
    {
        get => _isCover;

        set
        {
            if (value && !_isCover) OnFogCover?.Invoke(this);
            else if (!value && _isCover) OnFogUncover?.Invoke(this);

            _isCover = value;
            UpdateVisibility();
        }
    }

    public Transform Transform { get => transform; }

    private void Awake()
    {
        _collider = GetComponent<Collider>();
    }

    void OnEnable()
    {                
        if (FOWManager.Instance != null)
        {
            FOWManager.Instance.AddCoverable(this);
        }
        else
        {
            Debug.LogErrorFormat(debugLogHeader + "FOWManager is missing. Can't add {0} as a coverable", name);
        }
    }

    void OnDisable()
    {
        if (FOWManager.Instance != null)
        {
            FOWManager.Instance.RemoveCoverable(this);
        }
    }

    void UpdateVisibility()
    {
        _rootModelRenderer.SetActive(!_isCover);
        _collider.enabled = !_isCover;
    }
}
