using Game.FogOfWar;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityFogVision : EntityComponent, IFogVision
{
    [Header("Viewer Settings")]
    [SerializeField] private SpriteRenderer _fogOfWarVision = null;

    public float ViewRadius => Entity.Data.ViewRadius;
    public Transform Transform => transform;

    void Start()
    {
        _fogOfWarVision.transform.localScale = Vector3.one * Entity.Data.ViewRadius * 2;
    }

    void OnEnable()
    {
        _fogOfWarVision.enabled = true;
        FOWManager.Instance.AddViewer(this);
    }

    void OnDisable()
    {
        _fogOfWarVision.enabled = false;
        FOWManager.Instance?.RemoveViewer(this);
    }

    void OnDestroy()
    {
        if (_fogOfWarVision != null)
        {
            Destroy(_fogOfWarVision.gameObject);
        }

    }
}
