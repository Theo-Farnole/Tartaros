using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Game.Entities
{
    public class EntityObstacle : EntityComponent
    {
        #region Fields
        private const string debugLogHeader = "Entity Obstacle : ";

        NavMeshObstacle _navMeshObstacle;
        #endregion

        #region Properties
        private NavMeshObstacle NavMeshObstacle
        {
            get
            {
                if (_navMeshObstacle == null)
                    _navMeshObstacle = GetComponent<NavMeshObstacle>();

                return _navMeshObstacle;
            }
        }
        #endregion

        #region Methods
        void Start()
        {
            SetupNavMeshObstacle();
        }

        private void SetupNavMeshObstacle()
        {
            if (NavMeshObstacle == null)
                return;

            switch (NavMeshObstacle.shape)
            {
                case NavMeshObstacleShape.Capsule:
                    SetupNavMeshObstacle_Capsule();
                    break;

                case NavMeshObstacleShape.Box:
                    SetupNavMeshObstacle_Box();
                    break;

                default:
                    throw new System.NotImplementedException();
            }

        }

        private void SetupNavMeshObstacle_Capsule()
        {
            if (Entity.Data.TileSize.x != Entity.Data.TileSize.y)
            {
                Debug.LogWarningFormat(debugLogHeader + "TileSize isn't a square. We set nav mesh agent's radius with highest value of tile size.");
            }

            float diameter = Mathf.Max(Entity.Data.TileSize.x, Entity.Data.TileSize.y);
            NavMeshObstacle.radius = diameter / 2;
        }

        private void SetupNavMeshObstacle_Box()
        {
            Vector3 box = NavMeshObstacle.size;

            box.x = Entity.Data.TileSize.x;
            box.y = 3;
            box.z = Entity.Data.TileSize.y;

            NavMeshObstacle.size = box;
        }
        #endregion
    }
}
