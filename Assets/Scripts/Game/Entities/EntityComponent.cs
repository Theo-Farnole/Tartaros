using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public abstract class EntityComponent : MonoBehaviour
{
    private Entity _entity = null;

    public virtual Entity Entity
    {
        get
        {
            if (_entity == null)
            {
                _entity = GetComponent<Entity>();
                _entity.RegisterComponent(this);
            }

            Assert.IsNotNull(_entity, "Missing entity on entity " + name + ".");
            return _entity;
        }

        set
        {
            if (_entity != null)
            {
                Debug.LogWarning("Entity is already set. You can't change it value!");
                return;
            }

            _entity = value;
        }
    }
}
