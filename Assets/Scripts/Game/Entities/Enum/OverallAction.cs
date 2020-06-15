namespace Game.Entities
{
    using Game.Inputs;
    using Game.Selection;
    using System;

    [Flags]
    public enum OverallAction
    {
        None = 0,
        Stop = 1,
        Move = 2,
        Attack = 4,
        Patrol = 8,
        MoveAggressively = 16
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

                case OverallAction.MoveAggressively:
                    return () => SecondClickListener.Instance.ListenToMoveAggresively();

                default:
                    throw new NotImplementedException(string.Format("Please, implement {0} enum.", overallAction));
            }
        }
    }
}