using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rigidbody2DAdapter : MonoBehaviour, IRigidbodyAdapter
{
    [SerializeField] private Rigidbody2D _rigidbody;

    private void Awake()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    public Vector2 Velocity => _rigidbody.velocity;

    public float GravityScale
    {
        get => _rigidbody.gravityScale;
        set => _rigidbody.gravityScale = value;
    }

    public void SetVelocity(Vector2 velocity)
    {
        _rigidbody.velocity = velocity;
    }
}
