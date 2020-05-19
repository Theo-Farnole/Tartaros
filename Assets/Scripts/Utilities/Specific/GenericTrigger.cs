using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void OnTriggerEnter(Collider other);
public delegate void OnTriggerExit(Collider other);

public class GenericTrigger : MonoBehaviour
{
    public event OnTriggerEnter OnTriggerEnterEvent;
    public event OnTriggerExit OnTriggerExitEvent;

    private void OnTriggerEnter(Collider other)
    {
        OnTriggerEnterEvent?.Invoke(other);
    }

    private void OnTriggerExit(Collider other)
    {
        OnTriggerExitEvent?.Invoke(other);
    }
}
