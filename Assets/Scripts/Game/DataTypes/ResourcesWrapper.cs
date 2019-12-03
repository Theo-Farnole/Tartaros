using MyBox;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum Resource
{
    Food,
    Wood,
    Gold
}

[System.Serializable]
public struct ResourcesWrapper
{
    [PositiveValueOnly] public int food;
    [PositiveValueOnly] public int wood;
    [PositiveValueOnly] public int gold;

    public ResourcesWrapper(int food = 0, int wood = 0, int gold = 0)
    {
        this.food = food;
        this.wood = wood;
        this.gold = gold;
    }

    public int GetResource(Resource resource)
    {
        switch (resource)
        {
            case Resource.Food:
                return food;

            case Resource.Wood:
                return wood;

            case Resource.Gold:
                return gold;
        }

        return -1;
    }

    public void AddResource(Resource resource, int amount)
    {
        switch (resource)
        {
            case Resource.Food:
                food += amount;
                break;

            case Resource.Wood:
                wood += amount;
                break;

            case Resource.Gold:
                gold += amount;
                break;
        }
    }

    public bool HasEnoughtResources(ResourcesWrapper b)
    {
        return (this > b);
    }


    #region Operator
    public static ResourcesWrapper operator -(ResourcesWrapper a, ResourcesWrapper b) =>
        new ResourcesWrapper(a.food - b.food, a.wood - b.wood, a.gold - b.gold);

    public static ResourcesWrapper operator +(ResourcesWrapper a, ResourcesWrapper b) =>
        new ResourcesWrapper(a.food + b.food, a.wood + b.wood, a.gold + b.gold);

    public static bool operator <(ResourcesWrapper a, ResourcesWrapper b) =>
         a.food < b.food && a.wood < b.wood && a.gold < b.gold;

    public static bool operator >(ResourcesWrapper a, ResourcesWrapper b) =>
        a.food > b.food && a.wood > b.wood && a.gold > b.gold;
    
    public static bool operator >=(ResourcesWrapper a, ResourcesWrapper b) =>
        a.food >= b.food && a.wood >= b.wood && a.gold >= b.gold;

    public static bool operator <=(ResourcesWrapper a, ResourcesWrapper b) =>
        a.food <= b.food && a.wood <= b.wood && a.gold <= b.gold;

    public static bool operator ==(ResourcesWrapper a, ResourcesWrapper b) =>
        a.food == b.food && a.wood == b.wood && a.gold == b.gold;

    public static bool operator !=(ResourcesWrapper a, ResourcesWrapper b) =>
        !(a == b);
    #endregion
}