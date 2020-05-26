using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[RequireComponent(typeof(Canvas))]
public class SetCanvasCamera : MonoBehaviour
{    
    void Start()
    {
        var canvas = GetComponent<Canvas>();

        Assert.IsNotNull(canvas);

        canvas.worldCamera = Camera.main;
    }
}
