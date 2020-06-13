using Game.Selection;
using Lortedo.Utilities.DataTypes;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(Camera))]
[AddComponentMenu("RTS Camera")]
public partial class CameraController : MonoBehaviour
{
    #region Fields
    [Required]
    [SerializeField] private CameraControllerData _data;

    [SerializeField] private bool _enableMapLimit = true;
    [SerializeField] private Bounds2D _mapLimitX = new Bounds2D(-100, 100);
    [SerializeField] private Bounds2D _mapLimitZ = new Bounds2D(-100, 100);
    #endregion

    #region Methods
    #region MonoBehaviour Callbacks
    void Update()
    {
        ManageMovement();

        if (_data.CanCenterOnSelection && Input.GetKeyDown(_data.CenterKeyCode))
        {
            CenterOnSelection();
        }
    }
    #endregion

    #region Private Methods
    private void CenterOnSelection()
    {
        // Centroid

        // abort center on selection if not selection
        if (SelectionManager.Instance.SelectedGroups.Count == 0)
            return;

        Vector3 centroid = SelectionManager.Instance.GetSelectedEntitesCentroid();

        // Calculating offset
        Ray ray = new Ray(transform.position, transform.forward);

        bool successfulRaycast = Physics.Raycast(ray, out RaycastHit hit);

        if (!successfulRaycast)
        {
            Debug.LogErrorFormat("Camera Controller : Raycast unsucessful on center selection. Abort.");
            return;
        }

        Vector3 offset = transform.position - hit.point;

        // set new position
        transform.position = centroid + offset;
    }

    private void ManageMovement()
    {
        float deltaTime = Time.deltaTime;
        Vector3 deltaPosition = Vector3.zero;

        ProcessInput_Movement_Keyboard(deltaTime, ref deltaPosition);
        ProcessInput_Movement_ScreenEdge(deltaTime, ref deltaPosition);

        ProcessInput_Zoom(deltaTime, ref deltaPosition);

        Translate(deltaPosition);
    }

    private void ProcessInput_Movement_Keyboard(float deltaTime, ref Vector3 deltaPosition)
    {
        if (!_data.UseKeyboardInput)
            return;

        // go forward
        if (Input.GetKey(_data.KeyPanForward) || Input.GetKey(_data.KeyPanForwardAlternative))
            deltaPosition.z += _data.PanSpeedKeyboard * deltaTime;

        // go backward
        if (Input.GetKey(_data.KeyPanBackward) || Input.GetKey(_data.KeyPanBackwardAlternative))
            deltaPosition.z -= _data.PanSpeedKeyboard * deltaTime;

        // go left
        if (Input.GetKey(_data.KeyPanLeft) || Input.GetKey(_data.KeyPanLeftAlternative))
            deltaPosition.x -= _data.PanSpeedKeyboard * deltaTime;

        // go right
        if (Input.GetKey(_data.KeyPanRight) || Input.GetKey(_data.KeyPanRightAlternative))
            deltaPosition.x += _data.PanSpeedKeyboard * deltaTime;
    }

    private void ProcessInput_Movement_ScreenEdge(float deltaTime, ref Vector3 deltaPosition)
    {
        if (!_data.UseScreenEdgeInput)
            return;

        // go forward
        if (Input.mousePosition.y >= Screen.height - _data.PanBorderThickness)
            deltaPosition.z += _data.PanSpeedScreenEdge * deltaTime;

        // go backward
        if (Input.mousePosition.y < 0 + _data.PanBorderThickness)
            deltaPosition.z -= _data.PanSpeedScreenEdge * deltaTime;

        // go left
        if (Input.mousePosition.x < 0 + _data.PanBorderThickness)
            deltaPosition.x -= _data.PanSpeedScreenEdge * deltaTime;

        // go right
        if (Input.mousePosition.x > Screen.width - _data.PanBorderThickness)
            deltaPosition.x += _data.PanSpeedScreenEdge * deltaTime;
    }

    private void ProcessInput_Zoom(float deltaTime, ref Vector3 deltaPosition)
    {
        if (!_data.CanZoom)
            return;

        float inputDelta = Input.mouseScrollDelta.y;

        if (_data.InverseInput)
            inputDelta *= -1f;

        deltaPosition.y += inputDelta * deltaTime * _data.ZoomSpeed;
    }


    void Translate(Vector3 deltaPosition)
    {
        Vector3 forward = transform.forward;
        Vector3 right = transform.right;
        Vector3 up = forward;

        // project 'forward' and 'right' on plane
        forward.y = 0;
        right.y = 0;

        Vector3 deltaForward = deltaPosition.z * forward;
        Vector3 deltaRight = deltaPosition.x * right;
        Vector3 deltaUp = deltaPosition.y * up;

        if (_enableMapLimit)
        {
            // Without this line,
            // if zoom reach bounds, the camera continue to moves on Z/X axis without zooming.        
            if (transform.position.y + deltaUp.y > _data.ZoomBounds.max || transform.position.y + deltaUp.y < _data.ZoomBounds.min)
                deltaUp = Vector3.zero;
        }

        Vector3 finalDelta = deltaForward + deltaRight + deltaUp;

        Vector3 finalPosition = transform.position + finalDelta;

        if (_enableMapLimit)
        {
            finalPosition.x = Mathf.Clamp(finalPosition.x, _mapLimitX.min, _mapLimitX.max);
            finalPosition.y = Mathf.Clamp(finalPosition.y, _data.ZoomBounds.min, _data.ZoomBounds.max);
            finalPosition.z = Mathf.Clamp(finalPosition.z, _mapLimitZ.min, _mapLimitZ.max);
        }

        transform.position = finalPosition;

        if (_enableMapLimit)
        {
            // if value is inside of bounds, Mathf Clamp doesn't change the 'value' argument's value.
            // However, if it changed it, it's mean that the camera is out of the map.
            Assert.AreEqual(transform.position.x, Mathf.Clamp(transform.position.x, _mapLimitX.min, _mapLimitX.max), "Camera out of map limits");
            Assert.AreEqual(transform.position.z, Mathf.Clamp(transform.position.z, _mapLimitZ.min, _mapLimitZ.max), "Camera out of map limits");
        }
    }
    #endregion
    #endregion
}

#if UNITY_EDITOR
public partial class CameraController : MonoBehaviour
{
    void OnDrawGizmosSelected()
    {
        // don't draw if we don't have 'zoom bounds'
        if (_data == null)
            return;

        var size = new Vector3(
            _mapLimitX.max - _mapLimitX.min,
            _data.ZoomBounds.max - _data.ZoomBounds.min,
            _mapLimitZ.max - _mapLimitZ.min
        );

        var center = new Vector3(
            _mapLimitX.min,
            _data.ZoomBounds.min,
            _mapLimitZ.min
        ) + size / 2;

        Bounds bounds = new Bounds(size, center);

        Gizmos.DrawWireCube(center, size);
    }
}
#endif