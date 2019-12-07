using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EntityComponent : MonoBehaviour
{
    private Entity _entity;

    public Entity Entity
    {
        get => _entity;
        set
        {
            if (_entity != null)
            {
                Debug.LogWarning("Entity is already set. You can't change it value!");
                return;
            }
            else
            {
                _entity = value;
            }
        }
    }
}
