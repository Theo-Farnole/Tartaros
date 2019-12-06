using Lortedo.Utilities.Pattern;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LeonidasLegacy.IA.Action
{
    public abstract class Action : OwnedState<Unit>
    {
        public Action(Unit owner) : base(owner)
        {
        }

        public Unit unitManager { get => _owner; }
    }
}
