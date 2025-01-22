
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Templar : Enemy
{
    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        _rb.gravityScale = 12f;
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();

        if (!_isRecoiling)
        {
            transform.position = Vector2.MoveTowards
                (transform.position, new Vector2(PlayerMovement.Instance.transform.position.x, transform.position.y), _speed * Time.deltaTime);
        }
    }

    public override void EnemyHit(float damageDone, Vector2 hitDirection, float hitForce)
    {
        base.EnemyHit(damageDone, hitDirection, hitForce);
    }
}
