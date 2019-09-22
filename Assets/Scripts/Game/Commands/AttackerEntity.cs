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

    private bool _isAttacking;
    private Transform _target;

    private MovableEntity _movableEntity;
    private float _attackTimer = 0;
    #endregion

    #region Methods
    void Awake()
    {
        _movableEntity = GetComponent<MovableEntity>();
        _attackTimer = _data.AttackSpeed;
    }

    void Update()
    {
        if (!_isAttacking)
            return;

        _attackTimer += Time.deltaTime;

        // is in attackrange ?
        if (Vector3.Distance(transform.position, _target.position) <= _data.AttackRange)
        {
            _movableEntity?.Stop();

            // can attack ?
            if (_attackTimer >= _data.AttackSpeed)
            {
                _attackTimer = 0;
                Debug.Log(transform.name + " do " + _data.Damage + " to " + _target.name + ".");
            }
        }
        else
        {
            _movableEntity?.GoTo(_target.position);
        }
    }

    public void StartAttacking(Transform target)
    {
        _isAttacking = true;
        _target = target;
    }

    public void StopAttack()
    {
        _isAttacking = false;
    }
    #endregion
}
