using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitResourcesGenerator : UnitComponent
{
    private float _currentTimer = 0;

    void Update()
    {
        if (!UnitManager.Data.CanCreateResources)
            return;

        _currentTimer += Time.deltaTime;

        if (_currentTimer >= UnitManager.Data.GenerationTick)
        {
        _currentTimer = 0;
            CreateResources();
        }
    }

    void CreateResources()
    {
        GameManager.Instance.Resources += UnitManager.Data.ResourcesAmount;
    }
}
