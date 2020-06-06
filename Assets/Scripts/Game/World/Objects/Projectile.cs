using Game.Entities;
using Lortedo.Utilities.Pattern;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour, IPooledObject
{
    #region Fields
    private readonly float _projectileLifetime = 10;

    private Entity _attacker;
    private Entity _victim;
    private Team _attackerTeam;
    private Rigidbody _rigidbody;

    private bool _isLaunched = false;
    private Coroutine _autoDestroyCoroutine = null;

    public string ObjectTag { get; set; }
    #endregion

    #region Methods
    #region MonoBehaviour Callbacks
    void Awake()
    {
        _rigidbody = GetComponent<Rigidbody>();

        OnObjectSpawn();
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
        
        if (other.TryGetComponent(out Entity entity))
        {
            if (entity.Team != _attackerTeam)
            {
                entity.GetCharacterComponent<EntityHealth>().GetDamage(_attacker.Data.Damage, _attacker);
                DestroyProjectile();

                _isLaunched = false;
            }
        }
    }

    void OnDisable()
    {
        StopCoroutine(_autoDestroyCoroutine);
    }
    #endregion

    private void DestroyProjectile()
    {
        _isLaunched = true;

        ObjectPooler.Instance.EnqueueGameObject(_attacker.Data.PrefabProjectile, gameObject);

        StopCoroutine(_autoDestroyCoroutine);
    }

    public void Throw(Entity victim, Entity attacker)
    {
        _isLaunched = true;
        _attacker = attacker;
        _victim = victim;
        _attackerTeam = attacker.Team;

        _rigidbody.velocity = Vector3.zero;
        Vector3 initialVector = CalcBallisticVelocityVector(transform.position, victim.transform.position + Vector3.up, 45);
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

    public void OnObjectSpawn()
    {
        _autoDestroyCoroutine = this.ExecuteAfterTime(_projectileLifetime, () =>
        {
            if (gameObject.activeInHierarchy)
            {
                DestroyProjectile();
            }
        });
    }
    #endregion
}
