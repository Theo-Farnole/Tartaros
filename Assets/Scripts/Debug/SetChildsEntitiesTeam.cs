using Game.Entities;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetChildsEntitiesTeam : MonoBehaviour
{
    [SerializeField] private Team _wantedTeam;
    [SerializeField] private bool _setAllOnAwake = true;

    private void Awake()
    {
        if (_setAllOnAwake)
            SetAll();
    }

    [Button]
    void SetAll()
    {
        Entity[] entities = GetComponentsInChildren<Entity>();

        for (int i = 0; i < entities.Length; i++)
        {
            entities[i].Team = _wantedTeam;
        }

        Debug.LogFormat("Set all of {0} entities.", entities.Length);
    }
}
