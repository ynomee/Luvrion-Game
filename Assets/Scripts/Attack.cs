using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAttack
{
    void HandleAttack(float yAxis, float groundTime);
}

public class Attack : MonoBehaviour, IAttack
{
    [SerializeField] private Transform _sideAttackCheck, _upAttackCheck, _downAttackCheck;
    [SerializeField] private Vector2 _sideAttackArea, _upAttackArea, _downAttackArea;
    [SerializeField] private LayerMask _attackableLayer;
    [SerializeField] private GameObject _splashEffect;
    [SerializeField] private float _damage;
    [SerializeField] private float _timeBetweenAttack;
    private float _timeSinceAttack;
    private bool _isAttacking = false;

    public void HandleAttack(float yAxis, float groundTime)
    {
        _timeSinceAttack += Time.deltaTime;

        if (_timeSinceAttack < _timeBetweenAttack || _isAttacking)
            return;

        _isAttacking = true;
        _timeSinceAttack = 0;

        if (yAxis == 0 || yAxis < 0 && groundTime > 0)
        {
            PlayerHit(_sideAttackCheck, _sideAttackArea);
            Instantiate(_splashEffect, _sideAttackCheck);
        }
        else if (yAxis > 0)
        {
            PlayerHit(_upAttackCheck, _upAttackArea);
            SplashEffectAngle(_splashEffect, 80, _upAttackCheck);
        }
        else if (yAxis < 0 && groundTime < 0)
        {
            PlayerHit(_downAttackCheck, _downAttackArea);
            SplashEffectAngle(_splashEffect, -90, _downAttackCheck);
        }
        
        StartCoroutine(EndAttack());
    }

    private void PlayerHit(Transform attackTransform, Vector2 attackArea)
    {
        Collider2D[] objectsToHit = Physics2D.OverlapBoxAll(attackTransform.position, attackArea, 0, _attackableLayer);

        if(objectsToHit.Length > 0)
            Debug.Log("Hit"); 

        for (int i = 0; i < objectsToHit.Length; i++)
        {
            if (objectsToHit[i].GetComponent<Enemy>() != null)
            {
                objectsToHit[i].GetComponent<Enemy>().EnemyHit(_damage);
            }
        }
    }

    private IEnumerator EndAttack()
    {
        yield return new WaitForSeconds(0.1f);
        _isAttacking = false;
    }

    private void SplashEffectAngle(GameObject splashEffect, int effectAngle, Transform attackTransfrom)
    {
        splashEffect = Instantiate(splashEffect, attackTransfrom);
        splashEffect.transform.eulerAngles = new Vector3(0 ,0 ,effectAngle);
        splashEffect.transform.localScale = new Vector2(transform.localScale.x, transform.localScale.y);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(_sideAttackCheck.position, _sideAttackArea);
        Gizmos.DrawWireCube(_upAttackCheck.position, _upAttackArea);
        Gizmos.DrawWireCube(_downAttackCheck.position, _downAttackArea);
    }

    private void Update()
    {
        _timeSinceAttack += Time.deltaTime;
    }
}
