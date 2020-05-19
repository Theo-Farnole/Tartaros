using Lortedo.Utilities.Pattern;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.IA.Action
{
    public abstract class Action : OwnedState<Entity>
    {
        public Action(Entity owner) : base(owner)
        {
        }

        public Entity entity { get => _owner; }
    }
}
