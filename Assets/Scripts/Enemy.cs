using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [SerializeField] private float _health;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (_health <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void EnemyHit(float damageDone)
    {
        _health -= damageDone;
    }
}
