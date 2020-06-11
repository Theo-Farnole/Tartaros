using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Turn the wall to have the same orientation of neightboor.
/// </summary>
[RequireComponent(typeof(WallChainDetector))]
public class WallOrientation : MonoBehaviour
{
    #region
    [SerializeField] private Transform _modelToRotate;
    [Space]
    [SerializeField] private Vector3 _eulerAnglesNorthToSouth = Vector3.zero;
    [SerializeField] private Vector3 _eulerAnglesWestToEast = new Vector3(0, 90, 0);

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
    #region MonoCallback Callbacks
    void OnEnable()
    {
        WallChainDetector.OnWallOrientationChanged += WallChainDetector_OnWallOrientationChanged;
        ForceUpdateRotation();
    }

    void Update()
    {
        ForceUpdateRotation();        
    }

    void OnDisable()
    {
        WallChainDetector.OnWallOrientationChanged -= WallChainDetector_OnWallOrientationChanged;           
    }
    #endregion

    #region Events Handlers
    private void WallChainDetector_OnWallOrientationChanged(WallChainDetector.WallOrientation wallOrientation)
    {       
        Quaternion rotation = WallOrientationToRotation(wallOrientation);
        _modelToRotate.rotation = rotation;

        // on est dans une gate
        if (GetComponent<WallAppearance>() == null) Debug.Log("On est dans une gate Rotation updated to " + rotation);
    }
    #endregion

    #region Private Methods
    private void ForceUpdateRotation()
    {
        WallChainDetector.ForceCalculateWallOrientation();

        Quaternion rotation = WallOrientationToRotation(WallChainDetector.Cached_CurrentWallOrientation);
        _modelToRotate.rotation = rotation;
    }

    Quaternion WallOrientationToRotation(WallChainDetector.WallOrientation wallOrientation)
    {
        switch (wallOrientation)
        {
            case WallChainDetector.WallOrientation.NotAWallOrJoint:
            case WallChainDetector.WallOrientation.NorthToSouth:
                return Quaternion.Euler(_eulerAnglesNorthToSouth);

            case WallChainDetector.WallOrientation.WestToEast:
                return Quaternion.Euler(_eulerAnglesWestToEast);

            default:
                throw new System.NotImplementedException();
        }
    }
    #endregion
    #endregion
}
