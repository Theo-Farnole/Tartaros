using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Avoidance
{
    Idle,
    Move,
    Fight
}

public static class AvoidanceExtension
{
    public static int ToPriority(this Avoidance a)
    {
        switch (a)
        {
            case Avoidance.Fight:
                return 5;

            case Avoidance.Move:
                return 15;

            case Avoidance.Idle:
                return 50;
        }

        Debug.LogErrorFormat("{0} isn't set in AvoidanceExtension.");

        return 99;
    }
}