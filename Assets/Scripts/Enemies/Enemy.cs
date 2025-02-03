using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    protected PlayerStateList pState;
    protected HealthComponent healthComponent;
    protected TimeRestore timeRestore;

    [SerializeField] protected float health;

    [SerializeField] protected float recoilLenght;
    [SerializeField] protected float recoilFactor;
    [SerializeField] protected bool isRecoiling = false;

    [SerializeField] protected GameObject playerObj;

    [SerializeField] protected float speed;
    [SerializeField] protected float damage;

    //[SerializeField] protected float distance;
    //[SerializeField] protected float distanceBetween = 4;

    protected Rigidbody2D rb;
    protected float recoilTimer;

    protected enum EnemyStates
    {
        //Templar
        Templar_IDLE,
        Templar_Charge,
        Templar_Combat,
        Templar_LeaveFight,
        Templar_Death

    }

    protected EnemyStates currentEnemyState;

    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        healthComponent = playerObj.GetComponent<HealthComponent>();
        pState = playerObj.GetComponent<PlayerStateList>();
        timeRestore = playerObj.GetComponent<TimeRestore>();
    }

    protected virtual void Update()
    {
        UpdateEnemyState();
        EnemyDeath();
        RecoilCheck();
    }

    public virtual void EnemyHit(float damageDone, Vector2 hitDirection, float hitForce)
    {
        health -= damageDone;

        if (!isRecoiling)
        {
            // Enemy recoil in the direction that the hit comes from
            rb.AddForce(-hitForce * recoilFactor * hitDirection);
            isRecoiling = true;
        }

    }

    protected virtual void EnemyDeath()
    {
        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    protected virtual void RecoilCheck()
    {
        if (isRecoiling)
        {
            if (recoilTimer < recoilLenght)
            {
                recoilTimer += Time.deltaTime;
            }
            else
            {
                isRecoiling = false;
                recoilTimer = 0;
            }
        }
    }

    protected virtual void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !pState.invinsible)
        {
            Attack();
            timeRestore.HitStopTime(0, 5, 0.5f);
        }
    }

    protected virtual void Attack()
    {
        healthComponent.TakeDamage(damage);
    }

    protected virtual void UpdateEnemyState() { }

    protected virtual void ChangeState(EnemyStates newState)
    {
        currentEnemyState = newState;
    }
}
