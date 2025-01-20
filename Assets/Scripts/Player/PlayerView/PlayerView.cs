using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerView : MonoBehaviour, IPlayerObserver
{
    [SerializeField] private Animator _animator;
    
    public void Initialize(PlayerModel model)
    {
        model.RegisterObserver(this);
    }

    //public PlayerModel _playerModel;

    // private void Awake()
    // {
    //     // ������������ ���� ������ ��� �����������
    //     if (_playerModel != null)
    //     {
    //         _playerModel.RegisterObserver(this);
    //     }
    // }

    // private void OnDestroy()
    // {
    //     // ������������ �� ������, ����� �������� ������
    //     if (_playerModel != null)
    //     {
    //         _playerModel.UnregisterObserver(this);
    //     }
    // }

    public void OnSpeedChanged(float speed)
    {
        _animator.SetFloat("Move", Mathf.Abs(speed));
    }
    public void OnJump()
    {
        _animator.SetBool("IsJumping", true);
    }

    public void OnLand()
    {
        
        _animator.SetBool("IsJumping", false);
    }

    public void OnJumpVelChanged(float verticalVelocity)
    {
        _animator.SetFloat("JumpState", verticalVelocity);
    }

    public void OnStartDash()
    {
        _animator.SetBool("IsDashing", true);
    }

    public void OnStopDash()
    {
        _animator.SetBool("IsDashing", false);
    }
    public void OnWallJumpStart()
    {
        _animator.SetBool("WallGrabbing", true);
    }

    public void OnWallJumpEnd()
    {
        _animator.SetBool("WallGrabbing", false);
    }

    public void OnAttack()
    {
        _animator.SetTrigger("Melee");
    }
}
