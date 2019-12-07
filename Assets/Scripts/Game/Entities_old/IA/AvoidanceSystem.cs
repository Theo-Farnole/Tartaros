using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class AvoidanceSystem
{
    private static readonly int FIGHT_PRIORITY = 5;
    private static readonly int IDLE_PRIORITY = 50;
    private static readonly int HAS_TARGET_MIN_DISTANCE_PRIORITY = 10;
    private static readonly int HAS_TARGET_MAX_DISTANCE_PRIORITY = 49;

    public static int GetFightPriority() => FIGHT_PRIORITY;
    public static int GetIdlePriority() => IDLE_PRIORITY;
    public static int GetHasTargetPriority(float currentDistanceWithTarget, float initialDistanceWithTarget)
    {
        var m = (HAS_TARGET_MIN_DISTANCE_PRIORITY - HAS_TARGET_MAX_DISTANCE_PRIORITY) / (0 - initialDistanceWithTarget);
        var b = HAS_TARGET_MAX_DISTANCE_PRIORITY - (m * initialDistanceWithTarget);

        return Mathf.RoundToInt(Mathf.Clamp(m * currentDistanceWithTarget + b, HAS_TARGET_MIN_DISTANCE_PRIORITY, HAS_TARGET_MAX_DISTANCE_PRIORITY));
    }
}
