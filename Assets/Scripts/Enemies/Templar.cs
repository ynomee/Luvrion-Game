
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Templar : Enemy
{
    [SerializeField] private Vector2 _spawnPoint;
    [SerializeField] private float _distance;
    [SerializeField] private float _combatExitTimer;

    private bool _movingRight = true;

    private float _attackCooldown = 1.5f; // Перезарядка атаки (в секундах)
    private float _lastAttackTime = 0;

    private Vector2 _leftLimit, _rightLimit; // Границы движения в бою
    private float _patrolDistance = 2f; // Расстояние патрулирования

    protected override void Start()
    {
        base.Start();
        //rb.gravityScale = 12f;

        _spawnPoint = transform.position;

        ChangeState(EnemyStates.Templar_IDLE);

        _leftLimit = new Vector2(_spawnPoint.x - _patrolDistance, transform.position.y);
        _rightLimit = new Vector2(_spawnPoint.x + _patrolDistance, transform.position.y);
    }

    //protected override void Update()
    //{
    //    base.Update();

    //    EnemyFollow();
    //}

    //private void EnemyFollow()
    //{
    //    if (!isRecoiling)
    //    {
    //        distance = Vector2.Distance(transform.position, playerObj.transform.position);
    //        Vector2 direction = playerObj.transform.position - transform.position;
    //        direction.Normalize();

    //        if (distance < distanceBetween)
    //        {
    //            transform.position = Vector2.MoveTowards(this.transform.position, new Vector2(playerObj.transform.position.x, transform.position.y), speed * Time.deltaTime);
    //        }

    //        // transform.position = Vector2.MoveTowards
    //        //     (transform.position, new Vector2(PlayerMovement.Instance.transform.position.x, transform.position.y), speed * Time.deltaTime);
    //    }
    //}

    protected override void UpdateEnemyState()
    {
        _distance = Vector2.Distance(transform.position, playerObj.transform.position);

        switch(currentEnemyState)
        {
            case EnemyStates.Templar_IDLE:
                {
                    
                    if (_distance < 7)
                    {
                        Debug.Log("Player entered combat range, switching to Templar_Combat");
                        ChangeState(EnemyStates.Templar_Combat);
                    }
                    break;
                }
            case EnemyStates.Templar_Combat:
                {
                    if (_distance <= 5 && Time.time > _lastAttackTime + _attackCooldown)
                    {
                        Attack();
                        _lastAttackTime = Time.time;
                    }
                    else
                    {
                        CombatMovement();
                    }

                    if (_distance > 10)
                    {
                        if (_combatExitTimer == 0)
                        {
                            _combatExitTimer = Time.time;
                        }

                        if (Time.time - _combatExitTimer >= 3f)
                        {
                            Debug.Log("Combat exit timer finished, switching to Templar_LeaveFight");
                            ChangeState(EnemyStates.Templar_LeaveFight);
                        }
                    }
                    else
                    {
                        _combatExitTimer = 0f;
                    }
                    break;
                }
            case EnemyStates.Templar_LeaveFight:
                {
                    ReturnToSpawn();
                    break;
                }
            case EnemyStates.Templar_Death:
                {
                    EnemyDeath();
                    break;
                }
        }
    }

    private void CombatMovement()
    {
        if (!isRecoiling)
        {
            float moveDirection = _movingRight ? 1 : -1;

            rb.velocity = new Vector2(moveDirection * speed, rb.velocity.y);

            Vector2 targetPos = _movingRight ? _rightLimit : _leftLimit;

            if (Mathf.Abs(transform.position.x - targetPos.x) < 0.1f)
            {
                _movingRight = !_movingRight;
            }

        }
    }

    private void ReturnToSpawn()
    {
        Vector2 targetPos = new Vector2(_spawnPoint.x, transform.position.y);
        transform.position = Vector2.MoveTowards(transform.position, targetPos, speed / 2f * Time.deltaTime);

        if (Vector2.Distance(transform.position, _spawnPoint) == _spawnPoint.x)
        {
            ChangeState(EnemyStates.Templar_IDLE);
            
            _combatExitTimer = 0;
        }
    }
}
