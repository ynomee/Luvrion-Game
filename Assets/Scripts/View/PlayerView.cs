using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerView : MonoBehaviour
{
    [SerializeField] private Animator _animator;

    public void UpdateMovement(float speed)
    {
        _animator.SetFloat("Speed", speed);
    }

    public void SetJumpState(bool isJumpung)
    {
        _animator.SetBool("IsJumping", isJumpung);
    }

    public void SetDoubleJumpState(bool isDoubleJumping)
    {
        _animator.SetBool("IsDoubleJumping", isDoubleJumping);
    }
    public void TriggerDash()
    {
        _animator.SetBool("IsDashing", true);
    }

    public void StopDash()
    {
        _animator.SetBool("IsDashing", false);
    }
    public void TriggerWallJump()
    {
        _animator.SetTrigger("WallJumpTrigger");
    }
}
