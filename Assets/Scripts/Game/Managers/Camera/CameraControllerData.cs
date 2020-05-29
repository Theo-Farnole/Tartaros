using Lortedo.Utilities.DataTypes;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tartaros/Camera Controller")]
public class CameraControllerData : ScriptableObject
{
    public const string keyboardInputHeader = "Use Keyboard Input";
    public const string screenEdgeInputHeader = "Screen Edge Input";
    public const string canZoomHeader = "Can Zoom";

    #region Use Keyboard Input
    [ToggleGroup(nameof(_useKeyboardInput), keyboardInputHeader)]
    [SerializeField] private bool _useKeyboardInput = true;

    [ToggleGroup(nameof(_useKeyboardInput), keyboardInputHeader)]
    [SerializeField] private float _panSpeedKeyboard = 3;

    [ToggleGroup(nameof(_useKeyboardInput), keyboardInputHeader)]
    [Header("PAN SPEED > INPUTS")]
    [SerializeField] private KeyCode _keyPanForward = KeyCode.Z;
    [ToggleGroup(nameof(_useKeyboardInput), keyboardInputHeader)]
    [SerializeField] private KeyCode _keyPanBackward = KeyCode.S;
    [ToggleGroup(nameof(_useKeyboardInput), keyboardInputHeader)]
    [SerializeField] private KeyCode _keyPanLeft = KeyCode.Q;
    [ToggleGroup(nameof(_useKeyboardInput), keyboardInputHeader)]
    [SerializeField] private KeyCode _keyPanRight = KeyCode.D;
    [Space]
    [ToggleGroup(nameof(_useKeyboardInput), keyboardInputHeader)]
    [SerializeField] private KeyCode _keyPanForwardAlternative = KeyCode.UpArrow;
    [ToggleGroup(nameof(_useKeyboardInput), keyboardInputHeader)]
    [SerializeField] private KeyCode _keyPanBackwardAlternative = KeyCode.DownArrow;
    [ToggleGroup(nameof(_useKeyboardInput), keyboardInputHeader)]
    [SerializeField] private KeyCode _keyPanLeftAlternative = KeyCode.LeftArrow;
    [ToggleGroup(nameof(_useKeyboardInput), keyboardInputHeader)]
    [SerializeField] private KeyCode _keyPanRightAlternative = KeyCode.RightArrow;
    #endregion

    #region Screen Edge Input
    [ToggleGroup(nameof(_useScreenEdgeInput), screenEdgeInputHeader)]
    [SerializeField] private bool _useScreenEdgeInput = true;

    [ToggleGroup(nameof(_useScreenEdgeInput), screenEdgeInputHeader)]
    [SerializeField] private float _panSpeedScreenEdge = 3;

    [ToggleGroup(nameof(_useScreenEdgeInput), screenEdgeInputHeader)]
    [SerializeField] private float _panBorderThickness = 10;
    #endregion

    #region Can Zoom
    [ToggleGroup(nameof(_canZoom), canZoomHeader)]
    [SerializeField] private bool _canZoom = true;

    [ToggleGroup(nameof(_canZoom), canZoomHeader)]
    [SerializeField] private float _zoomSpeed = 5;

    [ToggleGroup(nameof(_canZoom), canZoomHeader)]
    [SerializeField] private Bounds2D _zoomBounds = new Bounds2D(5, 10);
    #endregion




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
    public bool UseKeyboardInput { get => _useKeyboardInput; }
    public float PanSpeedKeyboard { get => _panSpeedKeyboard; }
    public bool UseScreenEdgeInput { get => _useScreenEdgeInput; }
    public float PanSpeedScreenEdge { get => _panSpeedScreenEdge; }
    public bool CanZoom { get => _canZoom; }
}
