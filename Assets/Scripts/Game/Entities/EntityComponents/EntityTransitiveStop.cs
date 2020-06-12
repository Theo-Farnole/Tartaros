namespace Game.Entities
{
    using UnityEngine;

    /// <summary>
    /// This script stops the entity movement, if the current entity hits another entity that's idling and has the destination of entity.
    /// </summary>
    public class EntityTransitiveStop : EntityComponent
    {
        #region Fields
        [SerializeField] private float _maxDistanceFromDestinationToAllowStopping = -1;

        private bool _transitiveStopEnable = false;

        // cache variable
        private EntityDetection _cachedEntityDetection;
        private EntityMovement _cachedEntityMovement;
        #endregion

        #region Methods
        #region MonoBehaviour Callbacks
        void Start()
        {
            _cachedEntityDetection = GetComponent<EntityDetection>();
            _cachedEntityMovement = GetComponent<EntityMovement>();
        }

        void Update()
        {            
            TryToStopMovement();
        }
        #endregion

        #region Public Methods
        public void EnableTransitiveStop() => _transitiveStopEnable = true;
        public void DisableTransitiveStop() => _transitiveStopEnable = false;
        #endregion

        #region Private Methods
        private void TryToStopMovement()
        {
            // don't try to stop if idling
            if (Entity.IsIdle)
                return;

            if (!_transitiveStopEnable)
                return;

            // not enought near from destination
            if (_maxDistanceFromDestinationToAllowStopping != -1 && Vector3.Distance(_cachedEntityMovement.Destination, transform.position) > _maxDistanceFromDestinationToAllowStopping)
                return;

            float radius = Entity.Data.GetBiggerTileSize();
            Entity[] allies = _cachedEntityDetection.GetAlliesInRadius(radius);

            for (int i = 0; i < allies.Length; i++)
            {
                Entity ally = allies[i];

                if (ally.IsIdle && ally.GetCharacterComponent<EntityMovement>().Destination == _cachedEntityMovement.Destination)
                {
                    Entity.StopCurrentAction();
                    return;
                }
            }
        }
        #endregion
        #endregion
    }
}
