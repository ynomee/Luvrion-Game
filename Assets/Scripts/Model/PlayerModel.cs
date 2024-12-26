using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerModel
{
    public float Speed { get; private set; }
    public bool IsJumping { get; private set; }
    public bool IsDoubleJumping { get; private set; }
    public bool IsDashing { get; private set; }
    public bool IsOnWall { get; private set; }

    public void UpdateSpeed(float speed)
    {
        Speed = speed;
    }

    public void Jump()
    {
        IsJumping = true;
        IsDoubleJumping = false;
    }
}
