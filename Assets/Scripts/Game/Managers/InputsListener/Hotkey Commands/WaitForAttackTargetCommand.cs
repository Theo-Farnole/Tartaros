using Lortedo.Utilities.Pattern;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommandPattern
{
    public class WaitForAttackTargetCommand : Command
    {
        public override void Execute()
        {
            HotkeyActionListener.Instance.waitingForMouseClick = true;
            HotkeyActionListener.Instance.waitingForInputCursor = HotkeyActionListener.AskCursor.Attack;
        }
    }
}
