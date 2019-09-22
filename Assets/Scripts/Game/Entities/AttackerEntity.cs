using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AttackerEntity : MonoBehaviour
{
    #region Enums
    private enum State
    {
        GoTo,
        Attack
    }
    #endregion

    #region Fields
    [SerializeField] private AttackerEntityDatabase _data;

    private bool _isAttacking = false;
    private Transform _target = null;

    private float _attackTimer = 0;

    private MovableEntity _movableEntity;
    private Entity _entity;
    #endregion

    #region Methods
    void Awake()
    {
        _entity = GetComponent<Entity>();
        _movableEntity = GetComponent<MovableEntity>();

        _attackTimer = _data.AttackSpeed;
    }

    void Update()
    {
        _attackTimer += Time.deltaTime;

        if (!_isAttacking || _target == null)
            return;

        // is in attackrange ?
        if (Vector3.Distance(transform.position, _target.position) <= _data.AttackRange)
        {
            _movableEntity?.Stop();

            // can attack ?
            if (_attackTimer >= _data.AttackSpeed)
            {
                _attackTimer = 0;
                _target.GetComponent<Entity>().GetDamage(_data.Damage, _entity);
            }
        }
        else
        {
            _movableEntity?.GoTo(_target.position);
        }
    }

    public void StartAttacking(Transform target)
    {
        if (target != transform)
        {
            _isAttacking = true;
            _target = target;
        }
    }

    public void StopAttack()
    {
        _isAttacking = false;
    }
    #endregion
}
