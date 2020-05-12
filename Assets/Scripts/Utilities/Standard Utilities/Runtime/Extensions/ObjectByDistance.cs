using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ObjectByDistance<T>
{
    public T Object { get; set; }
    public float Distance { get; set; }

    public ObjectByDistance(T distanceObject, float distance)
    {
        this.Object = distanceObject;
        this.Distance = distance;
    }
}
