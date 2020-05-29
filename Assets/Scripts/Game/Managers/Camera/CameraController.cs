using Lortedo.Utilities.DataTypes;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class CameraController : MonoBehaviour
{
    [Required]
    [SerializeField] private CameraControllerData _data;

    [SerializeField] private Bounds2D _mapLimitX = new Bounds2D(-100, 100);
    [SerializeField] private Bounds2D _mapLimitZ = new Bounds2D(-100, 100);

    void Update()
    {
        ProcessPanBorderMovement(Time.deltaTime);
        ProcessZoomInput(Time.deltaTime);
    }

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

    private void ProcessPanBorderMovement(float deltaTime)
    {
        Vector3 deltaPosition = Vector3.zero;

        if (Input.GetKey(_data.KeyPanForward) || Input.GetKey(_data.KeyPanForwardAlternative) || Input.mousePosition.y >= Screen.height - _data.PanBorderThickness)
        {
            // go forward
            deltaPosition.z += _data.PanSpeed * deltaTime;
        }

        if (Input.GetKey(_data.KeyPanBackward) || Input.GetKey(_data.KeyPanBackwardAlternative) || Input.mousePosition.y < 0 + _data.PanBorderThickness)
        {
            // go backward
            deltaPosition.z -= _data.PanSpeed * deltaTime;
        }

        if (Input.GetKey(_data.KeyPanLeft) || Input.GetKey(_data.KeyPanLeftAlternative) || Input.mousePosition.x < 0 + _data.PanBorderThickness)
        {
            // go left
            deltaPosition.x -= _data.PanSpeed * deltaTime;
        }

        if (Input.GetKey(_data.KeyPanRight) || Input.GetKey(_data.KeyPanRightAlternative) || Input.mousePosition.x > Screen.width - _data.PanBorderThickness)
        {
            // go right
            deltaPosition.x += _data.PanSpeed * deltaTime;
        }
               
        Translate(deltaPosition);
    }

    void Translate(Vector3 deltaPosition)
    {       
        Vector3 forward = transform.forward;
        Vector3 right = transform.right;

        // project 'forward' and 'right' on plane
        forward.y = 0;
        right.y = 0;

        Vector3 deltaForward = deltaPosition.z * forward;
        Vector3 deltaRight = deltaPosition.x * right;

        Vector3 finalDelta = deltaForward + deltaRight;

        Vector3 finalPosition = transform.position + finalDelta;

        finalPosition.x = Mathf.Clamp(finalPosition.x, _mapLimitX.min, _mapLimitX.max);
        finalPosition.z = Mathf.Clamp(finalPosition.z, _mapLimitZ.min, _mapLimitZ.max);

        transform.position = finalPosition;

        // if value is inside of bounds, Mathf Clamp doesn't change the 'value' argument's value.
        // However, if it changed it, it's mean that the camera is out of the map.
        Assert.AreEqual(transform.position.x, Mathf.Clamp(transform.position.x, _mapLimitX.min, _mapLimitX.max), "Camera out of map limits");
        Assert.AreEqual(transform.position.z, Mathf.Clamp(transform.position.z, _mapLimitZ.min, _mapLimitZ.max), "Camera out of map limits");
    }

    private void ProcessZoomInput(float deltaTime)
    {
        Assert.IsNotNull(_data, "Please assign a CameraControllerData to camera " + name + ".");

        var inputDelta = Input.mouseScrollDelta.y;

        // if there is no user input,
        // don't execute code below for performance reason
        if (inputDelta == 0)
            return;

        // we need to reverse 'positionDeltaY'
        // to make the camera goes up, when the mouse scroll goes up
        var positionDeltaY = -(inputDelta * deltaTime * _data.ZoomSpeed);

        positionDeltaY = _data.GetClampedZoomPositionDelta(positionDeltaY, transform.position.y);

        transform.position += positionDeltaY * Vector3.up;
    }
}
