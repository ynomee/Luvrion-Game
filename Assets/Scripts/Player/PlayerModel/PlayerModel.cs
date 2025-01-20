using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerObserver
{
    void OnSpeedChanged(float speed);
    void OnJump();
    void OnLand();
    void OnJumpVelChanged(float verticalVelocity);
    void OnStartDash();
    void OnStopDash();
    void OnWallJumpStart();
    void OnWallJumpEnd();
    void OnAttack();
}

public class PlayerModel
{
    private readonly List<IPlayerObserver> _observers = new List<IPlayerObserver>();

    private float _speed;
    private float _verticalVelocity;

    private bool _isJumping;
    private bool _isDashing;
    private bool _isWallJumping;

    public void RegisterObserver(IPlayerObserver observer)
    {
        if (!_observers.Contains(observer))
        {
            _observers.Add(observer);
        }
    }

    public void UnregisterObserver(IPlayerObserver observer)
    {
        _observers.Remove(observer);
    }

    public void UpdateSpeed(float newSpeed)
    {
        if (Mathf.Approximately(_speed, newSpeed)) return;

        _speed= newSpeed;

        foreach (var observer in _observers)
        {
            observer.OnSpeedChanged(_speed);
        }
    }

    public void UpdateJumpState(bool isJumping)
    {
        if (_isJumping == isJumping) return; // �������� ������ �����������

        _isJumping = isJumping;

        foreach (var observer in _observers)
        {
            if (_isJumping)
            {
                observer.OnJump();
            }
            else
            {
                observer.OnLand();
            }
        }
    }

    public void UpdateDashState(bool isDashing)
    {
        if (_isDashing == isDashing) return;

        _isDashing = isDashing;

        foreach (var observer in _observers)
        {
            if (_isDashing)
            {
                observer.OnStartDash();
            }
            else
            {
                observer.OnStopDash();
            }
        }

    }

    public void UpdateWallJumpState(bool isWallJumping)
    {
        if (_isWallJumping == isWallJumping) return;

        _isWallJumping = isWallJumping;

        foreach (var observer in _observers)
        {
            if (_isWallJumping)
            {
                observer.OnWallJumpStart();
            }
            else
            {
                observer.OnWallJumpEnd();
            }
        }

    }

    // ���������� ������������ ��������
    public void UpdateVerticalVelocity(float newVerticalVelocity)
    {
        if (Mathf.Approximately(_verticalVelocity, newVerticalVelocity)) return;

        _verticalVelocity = newVerticalVelocity;

        foreach (var observer in _observers)
        {
            observer.OnJumpVelChanged(_verticalVelocity);
        }
    }

    public void UpdateAttackState()
    {   
        foreach (var observer in _observers)
        {
            observer.OnAttack();
        }
    }

}
