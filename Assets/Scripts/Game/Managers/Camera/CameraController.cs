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
}
