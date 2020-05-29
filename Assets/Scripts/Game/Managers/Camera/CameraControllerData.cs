using Lortedo.Utilities.DataTypes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tartaros/Camera Controller")]
public class CameraControllerData : ScriptableObject
{
    [Header("PAN SPEED")]
    [SerializeField] private float _panSpeed = 3;
    [SerializeField] private float _panBorderThickness = 10;

    [Header("PAN SPEED > INPUTS")]
    [SerializeField] private KeyCode _keyPanForward = KeyCode.Z;
    [SerializeField] private KeyCode _keyPanBackward = KeyCode.S;
    [SerializeField] private KeyCode _keyPanLeft = KeyCode.Q;
    [SerializeField] private KeyCode _keyPanRight = KeyCode.D;
    [Space]
    [SerializeField] private KeyCode _keyPanForwardAlternative = KeyCode.UpArrow;
    [SerializeField] private KeyCode _keyPanBackwardAlternative = KeyCode.DownArrow;
    [SerializeField] private KeyCode _keyPanLeftAlternative = KeyCode.LeftArrow;
    [SerializeField] private KeyCode _keyPanRightAlternative = KeyCode.RightArrow;

    [Header("ZOOM")]
    [SerializeField] private float _zoomSpeed = 5;
    [SerializeField] private Bounds2D _zoomBounds = new Bounds2D(5, 10);

    public float PanSpeed { get => _panSpeed; }
    public float ZoomSpeed { get => _zoomSpeed; }
    public float PanBorderThickness { get => _panBorderThickness; }
    public KeyCode KeyPanForward { get => _keyPanForward; }
    public KeyCode KeyPanBackward { get => _keyPanBackward; }
    public KeyCode KeyPanLeft { get => _keyPanLeft; }
    public KeyCode KeyPanRight { get => _keyPanRight; }
    public KeyCode KeyPanForwardAlternative { get => _keyPanForwardAlternative; }
    public KeyCode KeyPanBackwardAlternative { get => _keyPanBackwardAlternative; }
    public KeyCode KeyPanLeftAlternative { get => _keyPanLeftAlternative; }
    public KeyCode KeyPanRightAlternative { get => _keyPanRightAlternative; }
    public Bounds2D ZoomBounds { get => _zoomBounds; }

    public float GetClampedZoomPositionDelta(float deltaY, float currentPositionY)
    {
        return _zoomBounds.Clamp(deltaY + currentPositionY) - currentPositionY;
    }
}
