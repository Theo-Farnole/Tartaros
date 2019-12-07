using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityResourcesGeneration : EntityComponent
{
    private float _currentTimer = 0;

    void Update()
    {
        if (!Entity.Data.CanCreateResources)
            return;

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
