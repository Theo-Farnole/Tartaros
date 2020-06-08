using UnityEngine;

namespace Game.Entities
{
    /// <summary>
    /// This script has been created for performance. It only call FindObjectOfType each frame.
    /// </summary>
    public static class EntitiesManager
    {
        #region Fields
        private static bool initialized = false;

        private static KdTree<Entity> _playerTeamEntities = new KdTree<Entity>(true);
        private static KdTree<Entity> _enemyTeamEntities = new KdTree<Entity>(true);
        #endregion

        #region Events Handlers
        private static void Entity_OnTeamSwap(Entity entity, Team oldTeam, Team newTeam)
        {
            GetKDTree(oldTeam).RemoveAll(x => x == entity);
            GetKDTree(newTeam).Add(entity);
        }

        private static void Entity_OnDeath(Entity entity)
        {
            GetKDTree(entity.Team).RemoveAll(x => x == entity);
        }

        private static void Entity_OnSpawn(Entity entity)
        {
            GetKDTree(entity.Team).Add(entity);
        }
        #endregion

        #region Private methods
        private static KdTree<Entity> GetKDTree(Team team)
        {
            switch (team)
            {
                case Team.Player:
                    return _playerTeamEntities;

                case Team.Enemy:
                    return _enemyTeamEntities;

                // unsupported cases
                default:
                    throw new System.NotImplementedException();
            }
        }
        #endregion

        #region Public Methods
        public static void Initialize()
        {
            if (initialized)
                return;

            SubscribeToEvents();
            RecalculateKDTrees();

            initialized = true;
        }

        private static void SubscribeToEvents()
        {
            Entity.OnSpawn += Entity_OnSpawn;
            Entity.OnDeath += Entity_OnDeath;
            Entity.OnTeamSwap += Entity_OnTeamSwap;
        }

        private static void RecalculateKDTrees()
        {
            _playerTeamEntities = new KdTree<Entity>();
            _enemyTeamEntities = new KdTree<Entity>();

            var entities = Object.FindObjectsOfType<Entity>();

            foreach (var entity in entities)
            {
                switch (entity.Team)
                {
                    case Team.Player:
                        _playerTeamEntities.Add(entity);
                        break;
                    case Team.Enemy:
                        _enemyTeamEntities.Add(entity);
                        break;
                    // unsupported cases
                    default:
                        throw new System.NotImplementedException();
                }
            }
        }

        public static Entity GetClosestOpponentEntity(Vector3 position, Team entityTeam)
        {
            return GetKDTree(entityTeam.GetOpponent()).FindClosest(position);
        }

        public static Entity GetClosestAllyEntity(Vector3 position, Team entityTeam)
        {
            return GetKDTree(entityTeam).FindClosest(position);
        }
        #endregion
    }
}