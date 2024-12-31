using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerView : MonoBehaviour, IPlayerObserver
{
    [SerializeField] private Animator _animator;

    public PlayerView(Animator animator)
    {
        _animator = animator;
    }

    //public void UpdateMovement(float speed)
    //{
    //    _animator.SetFloat("Speed", speed);

    //    bool isMoving = Mathf.Abs(speed) > 0.1f;
    //    bool isFacingRight = speed > 0;

    //    _animator.SetBool("IsMoving", isMoving);
    //    _animator.SetBool("IsFacingRight", isFacingRight);
    //}

    public void OnSpeedChanged(float speed)
    {
        _animator.SetBool("IsMoving", Mathf.Abs(speed) > 0.1f);
        _animator.SetBool("IsFacingRight", speed > 0);
    }
    public void OnJump()
    {
        _animator.SetTrigger("Jump");
    }

    public void OnLand()
    {
        _animator.SetTrigger("Land");
    }

    //public void SetJumpState(bool isJumpung)
    //{
    //    _animator.SetBool("IsJumping", isJumpung);
    //}

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
