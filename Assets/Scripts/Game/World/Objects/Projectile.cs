using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    #region Fields
    private Entity _entityAttacker;
    private Rigidbody _rigidbody;

    private bool _isLaunched = false;
    #endregion

    #region Methods
    #region MonoBehaviour Callbacks
    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        if (_isLaunched)
        {
            transform.up = Vector3.Slerp(transform.up, _rigidbody.velocity.normalized, Time.deltaTime * 15);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (!_isLaunched)
            return;

        Entity ent = other.GetComponent<Entity>();

        if (ent != null && ent.owner != _entityAttacker.owner)
        {
            ent.HealthComponent.GetDamage(_entityAttacker.Data.Damage, _entityAttacker);
            ObjectPooler.Instance.EnqueueGameObject(_entityAttacker.Data.PrefabProjectile, gameObject);

            _isLaunched = false;
        }
    }
    #endregion

    public void Throw(Transform target, Entity attacker)
    {
        _isLaunched = true;
        _entityAttacker = attacker;

        _rigidbody.velocity = Vector3.zero;
        Vector3 initialVector = CalcBallisticVelocityVector(transform.position, target.position + Vector3.up, 45);
        _rigidbody.AddForce(initialVector, ForceMode.VelocityChange);
    }


    // code from
    // https://answers.unity.com/questions/1362266/calculate-force-needed-to-reach-certain-point-addf-1.html
    Vector3 CalcBallisticVelocityVector(Vector3 source, Vector3 target, float angle)
    {
        Vector3 direction = target - source;
        float h = direction.y;
        direction.y = 0;
        float distance = direction.magnitude;
        float a = angle * Mathf.Deg2Rad;
        direction.y = distance * Mathf.Tan(a);
        distance += h / Mathf.Tan(a);

        // calculate velocity
        float velocity = Mathf.Sqrt(distance * Physics.gravity.magnitude / Mathf.Sin(2 * a));
        return velocity * direction.normalized;
    }
    #endregion
}
