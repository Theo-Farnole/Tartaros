using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lortedo.Utilities.Pattern
{
    public abstract class OwnedState<T> : State
    {
        protected T _owner;

        public OwnedState(T owner)
        {
            _owner = owner;
        }
    }
}
