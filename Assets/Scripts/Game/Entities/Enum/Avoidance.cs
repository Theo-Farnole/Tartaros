namespace Game.Entities
{
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
                    return 50;

                case Avoidance.Move:
                    return 30;

                case Avoidance.Idle:
                    return 5;
                default:
                    Debug.LogErrorFormat("{0} isn't set in AvoidanceExtension.");
                    return 99;
            }
        }
    }
}