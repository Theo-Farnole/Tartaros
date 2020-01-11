using Lortedo.Utilities.Pattern;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommandPattern
{
    public class ListenSecondClickToPatrolCommand : Command
    {
        public override void Execute()
        {
            SecondClickListener.Instance.ListenToPatrol();
        }
    }
}
