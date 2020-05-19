using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnGUI_DisplayEntityCount : MonoBehaviour
{
    int entityCount = 0;

    void OnEnable()
    {
        Entity.OnSpawn += Entity_OnSpawn;
        Entity.OnDeath += Entity_OnDeath;
    }

    private void OnDisable()
    {
        Entity.OnSpawn -= Entity_OnSpawn;
        Entity.OnDeath -= Entity_OnDeath;
    }

    private void Entity_OnSpawn(Entity ent)
    {
        entityCount++;
    }

    private void Entity_OnDeath(Entity ent)
    {
        entityCount--;
    }


    void OnGUI()
    {
        const int padding = 20;

        Rect rect = new Rect(
            padding, padding,
            Screen.width, Screen.height);

        GUI.Label(rect, "Entities count : " + entityCount);
    }
}
