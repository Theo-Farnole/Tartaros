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
public class ResourcesWrapper
{
    public int wood = 0;
    public int food = 0;
    public int gold = 0;

    public ResourcesWrapper(int wood = 0, int food = 0, int gold = 0)
    {
        this.wood = wood;
        this.food = food;
        this.gold = gold;
    }
}

