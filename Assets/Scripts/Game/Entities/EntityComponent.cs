using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

namespace Game.Entities
{
    public abstract class EntityComponent : MonoBehaviour
    {
        private Entity _entity = null;

        // REFACTOR NOTE:
        // Do 'virtual' keyword is useful ?
        public virtual Entity Entity
        {
            get
            {
                if (_entity == null)
                {
                    _entity = GetComponent<Entity>();

                    Assert.IsNotNull(_entity, "Missing component 'Entity' on entity " + name + ".");

                    _entity.RegisterComponent(this);
                }

                return _entity;
            }

            set
            {
                if (_entity != null)
                    return;

                _entity = value;
            }
        }

        public T GetCharacterComponent<T>() where T : EntityComponent
            => Entity.GetCharacterComponent<T>();
    }
}
