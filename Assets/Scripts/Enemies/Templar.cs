
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Templar : Enemy
{
    [SerializeField] private Vector2 _spawnPoint;
    [SerializeField] private float _distance;

    private bool _isInCombat = false;
    private bool _isReturning = false;

    private Coroutine _combatCoroutine;
    private Coroutine _exitTimerCoroutine;



    protected override void Start()
    {
        base.Start();
        //rb.gravityScale = 12f;

        _spawnPoint = transform.position;

        ChangeState(EnemyStates.Templar_IDLE);
    }

    protected override void UpdateEnemyState()
    {
        _distance = Vector2.Distance(transform.position, playerObj.transform.position);

        switch(currentEnemyState)
        {
            case EnemyStates.Templar_IDLE:
                {           
                    if (_distance < 12)                  
                        EnterCombat();                   
                    break;
                }
            case EnemyStates.Templar_Combat:
                {
                    if (_distance > 12)
                    {
                        if (_exitTimerCoroutine == null)
                        {
                            _exitTimerCoroutine = StartCoroutine(ExitCombatTimer());
                        }
                        else
                        {
                            if (_exitTimerCoroutine != null)
                            {
                                StopCoroutine(ExitCombatTimer());
                                _exitTimerCoroutine = null;
                            }
                        }
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

    private void EnterCombat()
    {
        ChangeState(EnemyStates.Templar_Combat);

        _isInCombat = true;

        if (_combatCoroutine == null)
            _combatCoroutine = StartCoroutine(CombatBehaviour());
    }

    private IEnumerator CombatBehaviour()
    {
        while(_isInCombat)
        {
            FacePlayer();

            float waitTime = Random.Range(3, 5);
            yield return new WaitForSeconds(waitTime);

            if (_distance < 5)
            {
                Attack();
                Instantiate(_splashEffect, _EnemySideAttackCheck);
                timeRestore.HitStopTime(0, 5, 0.5f);
            }

            if (_distance > 5 && _distance <= 12)
            {
                yield return StartCoroutine(ChargeAttack());
            }
            else
            {
                yield return StartCoroutine(RandomMovement());
            }
        }
    }

    private IEnumerator RandomMovement()
    {
        ChangeState(EnemyStates.Templar_Combat);
        FacePlayer();

        float moveTime = Random.Range(1f, 1.5f);
        float direction = Random.Range(0, 2) == 0 ? -1f : 1f; // Лево или право

    // Если враг слева от игрока, направление движения сохраняется (+1 вперед, -1 назад)
    if (playerObj.transform.position.x > transform.position.x)
    {
        direction = (Random.Range(0, 2) == 0) ? 1f : -1f; // 50% вперед, 50% назад
    }
    else
    {
        direction = (Random.Range(0, 2) == 0) ? -1f : 1f; // 50% назад, 50% вперед
    }
        //
        rb.velocity = new Vector2(direction * speed * 0.5f, rb.velocity.y);

        yield return new WaitForSeconds(moveTime);

        rb.velocity = Vector2.zero;
    }

    private IEnumerator ChargeAttack()
    {
        ChangeState(EnemyStates.Templar_Charge);
        FacePlayer();

        float chargeSpeed = speed * Random.Range(2f, 3f);
        Vector2 direction = (playerObj.transform.position - transform.position).normalized;

        //
        rb.velocity = new Vector2(direction.x * chargeSpeed, rb.velocity.y);

        //Charge time
        yield return new WaitForSeconds(1.5f);

        rb.velocity = Vector2.zero;
        ChangeState(EnemyStates.Templar_Combat);
    }

    private IEnumerator ExitCombatTimer()
    {
        yield return new WaitForSeconds(3f);

        if(_distance > 10)
            ExitCombat();

    }

    private void ExitCombat()
    {
        _isInCombat = false;

        if(_combatCoroutine != null)
        {
            StopCoroutine(_combatCoroutine);
            _combatCoroutine = null;
        }

        ChangeState(EnemyStates.Templar_LeaveFight);
    }

    private void ReturnToSpawn()
    {
        if (_isReturning) return;
        _isReturning = true;
        StartCoroutine(MoveToSpawn());
    }

    private IEnumerator MoveToSpawn()
    {
        Debug.Log("Starting return to spawn");

        while (Vector2.Distance(transform.position, _spawnPoint) > 0.1f)
        {
        //После возврата проверить, если игрок снова рядом, сразу перейти в бой
            if (playerObj != null && Vector2.Distance(transform.position, playerObj.transform.position) < 12)
            {
                Debug.Log("Player is near, fight state again");
                //_isReturning = false;
                EnterCombat();
            }

            Vector2 direction = (_spawnPoint - (Vector2)transform.position).normalized;
            //
            rb.velocity = direction * (speed / 2);
            yield return null;
        }

        rb.velocity = Vector2.zero;
        _isReturning = false;
        ChangeState(EnemyStates.Templar_IDLE);

        Debug.Log("Enemy reached spawn point");
    }

    private void FacePlayer()
    {
        if (playerObj == null) return;

        Vector3 scale = transform.localScale;
        scale.x = (playerObj.transform.position.x > transform.position.x) ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
        transform.localScale = scale;
    }

}
