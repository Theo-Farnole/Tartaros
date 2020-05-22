using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Tartaros/Camera Controller")]
public class CameraControllerData : ScriptableObject
{
    [SerializeField] private float _speed = 3;

    public float Speed { get => _speed; }
}
