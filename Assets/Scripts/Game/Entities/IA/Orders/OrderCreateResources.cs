using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrderCreateResources : OwnedState<OrdersReceiver>
{
    private float _currentTimer = 0;

    public OrderCreateResources(OrdersReceiver owner) : base(owner)
    { }

    public override void Tick()
    {
        _currentTimer += Time.deltaTime;

        if (_currentTimer >= _owner.Entity.Data.GenerationTick)
        {
            CreateResources();
        }
    }

    void CreateResources()
    {
        _currentTimer = 0;
        GameManager.Instance.Resources += _owner.Entity.Data.ResourcesAmount;
    }
}
