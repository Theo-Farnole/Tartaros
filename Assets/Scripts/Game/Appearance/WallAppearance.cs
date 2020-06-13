namespace Game.Appearance.Walls
{
    using UnityEngine;

    /// <summary>
    /// Change model if the game object is a wall joint.
    /// </summary>
    [RequireComponent(typeof(WallChainDetector))]
    public class WallAppearance : MonoBehaviour
    {
        #region Fields
        private const string debugLogHeader = "Wall Apperance : ";

        [Header("OBJECTS LINKING")]
        [SerializeField] private GameObject _jointModel;
        [SerializeField] private GameObject _wallModel;

        private WallChainDetector _wallChainDetector;
        #endregion

        #region Properties
        public WallChainDetector WallChainDetector
        {
            get
            {
                if (_wallChainDetector == null)
                    _wallChainDetector = GetComponent<WallChainDetector>();

                return _wallChainDetector;
            }
        }
        #endregion

        #region Methods
        #region MonoBehaviour Callbacks
        void OnEnable()
        {
            WallChainDetector.OnWallJointChanged += WallChainDetector_OnWallJointChanged;

            ForceUpdateAppearance();
        }

        void OnDisable()
        {
            WallChainDetector.OnWallJointChanged -= WallChainDetector_OnWallJointChanged;
        }
        #endregion

        #region EventsHandlers
        private void WallChainDetector_OnWallJointChanged(bool isWallJoint)
        {
            _jointModel.SetActive(isWallJoint);
            _wallModel.SetActive(!isWallJoint);
        }
        #endregion

        #region Private Methods
        private void ForceUpdateAppearance()
        {
            bool isWallJoint = WallChainDetector.Cached_IsWallJoint;
            _jointModel.SetActive(isWallJoint);
            _wallModel.SetActive(!isWallJoint);
        }
        #endregion
        #endregion
    }
}
