using Lortedo.Utilities.Pattern;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommandPattern
{
    public class AskForMoveDestinationCommand : Command
    {
        public override void Execute()
        {
            HotkeyManager.Instance.askCursor = true;
            HotkeyManager.Instance.askCursorType = HotkeyManager.AskCursor.Move;
        }
    }
}
