using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] protected float _health;

    [SerializeField] protected float _recoilLenght;
    [SerializeField] protected float _recoilFactor;
    [SerializeField] protected bool _isRecoiling = false;

    [SerializeField] protected PlayerMovement _player;
    [SerializeField] protected float _speed;

    protected Rigidbody2D _rb;
    protected float _recoilTimer;

    protected virtual void Start()
    {
        _player = PlayerMovement.Instance;
        _rb = GetComponent<Rigidbody2D>();
    }

    protected virtual void Update()
    {
        if (_health <= 0)
        {
            Destroy(gameObject);
        }

        RecoilCheck();
    }

    public virtual void EnemyHit(float damageDone, Vector2 hitDirection, float hitForce)
    {
        _health -= damageDone;

        if (!_isRecoiling)
        {
            // Enemy recoil in the direction that the hit comes from
            _rb.AddForce(-hitForce * _recoilFactor * hitDirection);
            _isRecoiling = true;
        }

    }

    protected virtual void RecoilCheck()
    {
        if (_isRecoiling)
        {
            if (_recoilTimer < _recoilLenght)
            {
                _recoilTimer += Time.deltaTime;
            }
            else
            {
                _isRecoiling = false;
                _recoilTimer = 0;
            }
        }
    }
}
