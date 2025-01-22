using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IRigidbodyAdapter
{
    Vector2 Velocity { get; }
    float GravityScale { get; set; }
    void SetVelocity(Vector2 velocity);
}
