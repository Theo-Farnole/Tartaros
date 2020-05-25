using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public delegate void OnTriggerEnter(Collider other);
public delegate void OnTriggerExit(Collider other);

[RequireComponent(typeof(Collider))]
public class GenericTrigger : MonoBehaviour
{
    public event OnTriggerEnter OnTriggerEnterEvent;
    public event OnTriggerExit OnTriggerExitEvent;

#if UNITY_EDITOR
    void Start() => CheckIfColliderIsATrigger();
    void OnValidate() => CheckIfColliderIsATrigger();
#endif

    private void OnTriggerEnter(Collider other)
    {
        OnTriggerEnterEvent?.Invoke(other);
    }

    private void OnTriggerExit(Collider other)
    {
        OnTriggerExitEvent?.Invoke(other);
    }

    public void SetCollisionRadius(float radius)
    {
        if (TryGetComponent(out SphereCollider sphereCollider))
        {
            sphereCollider.radius = radius;
        }
        else
        {
            Debug.LogErrorFormat("Can't set collision because generic trigger doesn't have a sphere collider.");
        }
    }

    bool IsColliderATrigger()
    {
        var collider = GetComponent<Collider>();

        Assert.IsNotNull(collider, "Generic trigger " + name + " misses a Collider component.");

        return collider.isTrigger;
    }

    private void CheckIfColliderIsATrigger()
    {
        Assert.IsTrue(IsColliderATrigger(),
                    string.Format("Please, set collider as trigger on {0}.", name));
    }
}
