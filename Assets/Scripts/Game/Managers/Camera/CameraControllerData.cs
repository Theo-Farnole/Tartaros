using Lortedo.Utilities.DataTypes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tartaros/Camera Controller")]
public class CameraControllerData : ScriptableObject
{
    [SerializeField] private float _speed = 3;

    [Header("ZOOM")]
    [SerializeField] private float _zoomSpeed = 5;    
    [SerializeField] private Bounds2D _zoomBounds = new Bounds2D(5, 10);

    public float Speed { get => _speed; }
    public float ZoomSpeed { get => _zoomSpeed; }

    public float GetClampedZoomPositionDelta(float deltaY, float currentPositionY)
    {
        return _zoomBounds.Clamp(deltaY + currentPositionY) - currentPositionY;
    }
}
