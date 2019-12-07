using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityResourcesGeneration : EntityComponent
{
    private float _currentTimer = 0;

    void Start()
    {
        // disable this component if entity can't create resources
        if (!Entity.Data.CanCreateResources)
        {
            enabled = false;
        }        
    }

    void Update()
    {
        _currentTimer += Time.deltaTime;

        if (_currentTimer >= Entity.Data.GenerationTick)
        {
            _currentTimer = 0;
            CreateResources();
        }
    }

    void CreateResources()
    {
        GameManager.Instance.Resources += Entity.Data.ResourcesAmount;
    }
}
