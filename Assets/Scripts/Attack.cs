using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    [SerializeField] private Transform _sideAttackCheck, _upAttackCheck, _downAttackCheck;
    [SerializeField] private Vector2 _sideAttackArea, _upAttackArea, _downAttackArea;
    [SerializeField] private LayerMask _attackableLayer;
    private float _timeBetweenAttack;
    private float _timeSinceAttack;
    private bool _isAttacking = false;

    public void AttackLogic()
    {
        _timeSinceAttack += Time.deltaTime;
        if (_isAttacking && _timeSinceAttack >= _timeBetweenAttack)
        {
            _timeSinceAttack = 0;
        }
    }

    private void Hit(Transform attackTransform, Vector2 attackArea)
    {
        Collider2D[] objectsToHit = Physics2D.OverlapBoxAll(attackTransform.position, attackArea, 0, _attackableLayer);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(_sideAttackCheck.position, _sideAttackArea);
        Gizmos.DrawWireCube(_upAttackCheck.position, _upAttackArea);
        Gizmos.DrawWireCube(_downAttackCheck.position, _downAttackArea);
    }
}
