namespace Game.Entities
{
    using UnityEngine;
    using UnityEngine.AI;
    using DG.Tweening;

    public class EntitySizeSetter : AbstractEntityComponent
    {
        private enum Size
        {
            NormalSize,
            Shrinking,
            Shrinked,
            Expanding
        }

        #region Fields
        private const string debugLogHeader = "Entity Obstacle : ";

        [SerializeField] private CollisionScalerData _collisionScalerData;
        [SerializeField] private bool _reduceCollisionOnMove = true;

        private Vector3 _originalSizeObstacle = Vector3.zero;
        private float _originalAgentRadius = -1;
        private Size _currentSize = Size.NormalSize;

        private NavMeshObstacle _navMeshObstacle = null;
        private NavMeshAgent _navMeshAgent = null;
        #endregion

        #region Methods
        #region MonoBehaviour Callbacks
        void Start()
        {
            _navMeshObstacle = GetComponent<NavMeshObstacle>();
            _navMeshAgent = GetComponent<NavMeshAgent>();

            if (_navMeshAgent != null)
                _originalAgentRadius = _navMeshAgent.radius;

            Vector3 size = new Vector3
            {
                x = Entity.Data.TileSize.x,
                y = 1,
                z = Entity.Data.TileSize.y
            };

            _originalSizeObstacle = size;

            SetSize_NavMeshObstacle(size);
            SetSize_BoxCollider(size);
        }

        void OnEnable()
        {
            EntityMovement entityMovement = GetCharacterComponent<EntityMovement>();

            entityMovement.MovementStopped += EntitySizeSetter_MovementStopped;
            entityMovement.StartMove += EntitySizeSetter_StartMove;
        }

        void OnDisable()
        {
            EntityMovement entityMovement = GetCharacterComponent<EntityMovement>();

            entityMovement.MovementStopped -= EntitySizeSetter_MovementStopped;
            entityMovement.StartMove -= EntitySizeSetter_StartMove;
        }
        #endregion

        #region Events Handlers
        private void EntitySizeSetter_StartMove(Vector3 obj) => StartReduceCollision();

        private void EntitySizeSetter_MovementStopped() => StartGrowCollision();
        #endregion

        #region Public Methods
        public void StartReduceCollision()
        {
            if (_navMeshAgent == null)
                return;

            if (_currentSize == Size.Shrinking && _currentSize == Size.Shrinked)
                return;

            _currentSize = Size.Shrinking;

            DOTween.To(() => _navMeshAgent.radius, x => _navMeshAgent.radius = x, _originalAgentRadius * _collisionScalerData.CollisionScaleDownPercent, _collisionScalerData.ReduceTime)
                .OnComplete(() => { _currentSize = Size.Shrinked; });
        }

        public void StartGrowCollision()
        {
            if (_navMeshAgent == null)
                return;

            if (_currentSize == Size.NormalSize && _currentSize == Size.Expanding)
                return;

            _currentSize = Size.Expanding;

            DOTween.To(
                () => _navMeshAgent.radius,
                x => _navMeshAgent.radius = x,
                _originalAgentRadius,
                _collisionScalerData.IncreaseTime)
            .OnComplete(() => { _currentSize = Size.NormalSize; });
        }
        #endregion

        #region Private Methods
        private void SetSize_NavMeshObstacle(Vector3 size)
        {
            if (_navMeshObstacle == null)
                return;

            if (_navMeshObstacle.shape != NavMeshObstacleShape.Box)
                throw new System.NotSupportedException("Can't set obstacle size with NavMeshObstacle's shape set as " + _navMeshObstacle.shape + ".");

            Vector3 carveOffset = new Vector3(0.9f, 0, 0.9f);

            _navMeshObstacle.size = size - carveOffset;
            _navMeshObstacle.center = size.y / 2 * Vector3.up;
        }

        void SetSize_BoxCollider(Vector3 size)
        {
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
