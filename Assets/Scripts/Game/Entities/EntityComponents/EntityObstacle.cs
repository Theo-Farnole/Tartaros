using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Game.Entities
{
    public class EntitySizeSetter : EntityComponent
    {
        #region Fields
        private const string debugLogHeader = "Entity Obstacle : ";

        NavMeshObstacle _navMeshObstacle;
        #endregion

        #region Methods
        void Start()
        {
            _navMeshObstacle = GetComponent<NavMeshObstacle>();

            SetSize_NavMeshObstacle();
            SetSize_BoxCollider();
        }

        #region Private Methods
        private void SetSize_NavMeshObstacle()
        {
            if (_navMeshObstacle.shape != NavMeshObstacleShape.Box)
                throw new System.NotSupportedException("Can't set obstacle size with NavMeshObstacle's shape set as " + _navMeshObstacle.shape + ".");

            Vector3 carveOffset = new Vector3(0.9f, 0, 0.9f);

            Vector3 size = new Vector3
            {
                x = Entity.Data.TileSize.x,
                y = 3,
                z = Entity.Data.TileSize.y
            };

            _navMeshObstacle.size = size - carveOffset;
            _navMeshObstacle.center = size.y / 2 * Vector3.up;
        }

        void SetSize_BoxCollider()
        {
            Vector2Int tileSize = Entity.Data.TileSize;
            Vector3 size = new Vector3(tileSize.x, 2, tileSize.y);

            if (TryGetComponent(out BoxCollider boxCollider))
            {
                // keep the Y commponent of size
                size.y = boxCollider.size.y;

                boxCollider.size = size;
                boxCollider.center = size.y / 2 * Vector3.up;
            }
        }
        #endregion
        #endregion
    }
}
