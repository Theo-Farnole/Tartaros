using System;

namespace Game.Entities
{
    public enum Team
    {
        Player,
        Enemy
    }

    public static class TeamExtension
    {
        public static Team GetOpponent(this Team t)
        {
            switch (t)
            {
                case Team.Player:
                    return Team.Enemy;

                case Team.Enemy:
                    return Team.Player;

                default:
                    throw new NotImplementedException();
            }
        }
    }
}