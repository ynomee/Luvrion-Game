using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float _health;

    [SerializeField] private float _recoilLenght;
    [SerializeField] private float _recoilFactor;
    [SerializeField] private bool _isRecoiling = false;

    private Rigidbody2D _rb;
    private float _recoilTimer;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (_health <= 0)
        {
            Destroy(gameObject);
        }

        RecoilCheck();
    }

    public void EnemyHit(float damageDone, Vector2 hitDirection, float hitForce)
    {
        _health -= damageDone;

        if(!_isRecoiling)
        {
            // Enemy recoil in the direction that the hit comes from
            _rb.AddForce(-hitForce * _recoilFactor * hitDirection);
        }

    }

    private void RecoilCheck()
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
