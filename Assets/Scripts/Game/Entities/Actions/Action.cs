using Lortedo.Utilities.Pattern;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.IA.Action
{
    /// <summary>
    /// An action is owned by an Entity. 
    /// It's tick each Update() call.
    /// </summary>
    public abstract class Action : OwnedState<Entity>
    {
        public Action(Entity owner) : base(owner)
        {
        }

        public Entity entity { get => _owner; }

        public abstract bool CanExecuteAction();

        public override void OnStateEnter()
        {
            if (!CanExecuteAction())
            {
                Debug.LogWarningFormat("{0} : Aborting action because CanExecuteAction returns false.", this.GetType());
                _owner.StopCurrentAction();
            }
        }

        protected void LeaveAction()
        {
            _owner.StopCurrentAction();
        }
    }
}
