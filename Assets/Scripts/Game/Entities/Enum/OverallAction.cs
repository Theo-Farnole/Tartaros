using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum OverallAction
{
    Stop = 0,
    Move = 1,
    Attack = 2,
    Patrol = 3
}

public static class OverallActionExtension
{
    public static Action ToOrder(this OverallAction overallAction)
    {
        switch (overallAction)
        {
            case OverallAction.Stop:
                return () => SelectedGroupsActionsCaller.OrderStop();

            case OverallAction.Move:
                return () => SecondClickListener.Instance.ListenToMove(); 

            case OverallAction.Attack:
                return () => SecondClickListener.Instance.ListenToAttack();                

            case OverallAction.Patrol:
                return () => SecondClickListener.Instance.ListenToPatrol();                

            default:
                throw new NotImplementedException(string.Format("Please, implement {0} enum.", overallAction));
        }
    }
}