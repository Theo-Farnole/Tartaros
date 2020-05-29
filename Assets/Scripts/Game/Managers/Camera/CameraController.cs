using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class CameraController : MonoBehaviour
{
    [Required]
    [SerializeField] private CameraControllerData _data;    

    void Update()
    {
        ProcessMovement(Time.deltaTime);
        ProcessZoomInput(Time.deltaTime);
    }

    private void ProcessMovement(float deltaTime)
    {
        Assert.IsNotNull(_data, "Please assign a CameraControllerData to camera " + name + ".");

        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 forward = transform.forward;
        Vector3 right = transform.right;

        // project 'forward' and 'right' on plane
        forward.y = 0;
        right.y = 0;

        Vector3 deltaForward = _data.Speed * deltaTime * vertical * forward;
        Vector3 deltaRight = _data.Speed * deltaTime * horizontal * right;

        transform.position += deltaForward + deltaRight;
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
