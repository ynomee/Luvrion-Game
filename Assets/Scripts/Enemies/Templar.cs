
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Templar : Enemy
{    
    private float distance;
    protected override void Start()
    {
        base.Start();
        rb.gravityScale = 12f;
    }

    protected override void Update()
    {
        base.Update();

        EnemyFollow();
    }
    protected override void EnemyDeath()
    {
        base.EnemyDeath();
    }

    public override void EnemyHit(float damageDone, Vector2 hitDirection, float hitForce)
    {
        base.EnemyHit(damageDone, hitDirection, hitForce);
    }

    protected override void Attack()
    {
        base.Attack();
    }

    private void EnemyFollow()
    {
        if (!isRecoiling)
        {
            distance = Vector2.Distance(transform.position, playerObj.transform.position);
            Vector2 direction = playerObj.transform.position - transform.position;
            direction.Normalize();

            if (distance < distanceBetween)
            {
                transform.position = Vector2.MoveTowards(this.transform.position, new Vector2(playerObj.transform.position.x, transform.position.y), speed * Time.deltaTime);
            }

            // transform.position = Vector2.MoveTowards
            //     (transform.position, new Vector2(PlayerMovement.Instance.transform.position.x, transform.position.y), speed * Time.deltaTime);
        }
    }

    protected override void OnTriggerStay2D(Collider2D other)
    {
        base.OnTriggerStay2D(other);
    }
}
