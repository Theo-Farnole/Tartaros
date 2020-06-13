using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Game.Entities
{
    public abstract class AbstractEntityComponent : MonoBehaviour
    {
        private Entity _entity = null;

        public Entity Entity
        {
            get
            {
                if (_entity == null)
                {
                    _entity = GetComponent<Entity>();

                    Assert.IsNotNull(_entity, "Missing component 'Entity' on entity '" + name + "'.");

                    _entity.RegisterComponent(this);
                }

                return _entity;
            }

            set
            {
                // prevent initialization a second time
                if (_entity != null)
                    return;

                _entity = value;
            }
        }

        public T GetCharacterComponent<T>() where T : AbstractEntityComponent => Entity.GetCharacterComponent<T>();
        public EntityData Data => Entity.Data;
    }
}
