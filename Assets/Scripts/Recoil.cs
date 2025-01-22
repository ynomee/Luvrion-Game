using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Recoil : MonoBehaviour
{
    [Header("Components")]
    private IPlayerMovement _playerMovement;
    private IRigidbodyAdapter _rigidbodyAdapter;

    [Header("Recoil")]
    [SerializeField] private int _recoilXSteps = 5;
    [SerializeField] private int _recoilYSteps = 5;

    [SerializeField] public float _recoilXSpeed = 100;
    [SerializeField] public float _recoilYSpeed = 100;
    int stepsXRecoiled, stepsYRecoiled;

    public PlayerStateList pState;
    [SerializeField] private Rigidbody2D rb;

    public bool _recoilingX, _recoilingY;

    private float _defaultGravity;

    private void Awake()
    {
        _playerMovement = GetComponent<IPlayerMovement>();
        _rigidbodyAdapter = GetComponent<IRigidbodyAdapter>();

        //if (_rigidbodyAdapter != null)
        //{
        //    _defaultGravity = _rigidbodyAdapter.GravityScale;
        //}
    }

    private void Update()
    {
        HandleRecoil();
    }

    private void HandleRecoil()
    {
        Debug.Log("Recoil");

        //// Horizontal recoil
        //if (_recoilingX)
        //{
        //    float direction = _playerMovement.FacingRight ? -1 : 1;
        //    _rigidbodyAdapter.SetVelocity(new Vector2(direction * _recoilXSpeed, _rigidbodyAdapter.Velocity.y));
        //}
        if (pState.recoilingX)
        {
            rb.gravityScale = _defaultGravity;
            if (pState.lookingRight)
            {
                rb.velocity = new Vector2(-_recoilXSpeed,0);
            }
            else
            {
                rb.velocity = new Vector2( _recoilXSpeed, 0);
            }
        }
        if (pState.recoilingX && stepsXRecoiled < _recoilXSteps) 
        {
            stepsXRecoiled++;
        }
        else
        {
            StopRecoilX();
        }
        //StartCoroutine(ResetRecoil());
        // Vertical recoil
        //if (_recoilingY)
        //{
        //    _rigidbodyAdapter.GravityScale = 0;
        //    float verticalDirection = _recoilYSteps > 0 ? 1 : -1;
        //    _rigidbodyAdapter.SetVelocity(new Vector2(_rigidbodyAdapter.Velocity.x, verticalDirection * _recoilYSpeed));
        //}
        //else
        //{
        //    //_rigidbodyAdapter.GravityScale = _defaultGravity;
        //}
    }
    //public void ApplyRecoil(Vector2 direction)
    //{
    //    if (_rigidbodyAdapter == null || _playerMovement == null) return;

    //    // Расчёт вектора отдачи
    //    var velocity = new Vector2(
    //        direction.x * _recoilXSpeed * (_playerMovement.FacingRight ? -1 : 1),
    //        direction.y * _recoilYSpeed
    //    );

    //    // Применение скорости через адаптер
    //    _rigidbodyAdapter.SetVelocity(velocity);
    //}

    //public void TriggerRecoil(bool isHorizontal, bool isVertical)
    //{
    //    _recoilingX = isHorizontal;
    //    _recoilingY = isVertical;

    //    if (isHorizontal)
    //    {
    //        ApplyRecoil(new Vector2(1, 0)); // Отдача только по X
    //    }
    //    if (isVertical)
    //    {
    //        ApplyRecoil(new Vector2(0, 1)); // Отдача только по Y
    //    }

    //    // Сбрасываем отдачу через 0.2 секунды
    //    StartCoroutine(ResetRecoil());
    //}
    private void StopRecoilX()
    {
        stepsXRecoiled = 0;
        pState.recoilingX = false;
    }

    private IEnumerator ResetRecoil()
    {
        yield return new WaitForSeconds(0.2f);
        _recoilingX = false;
        _recoilingY = false;
    }
}
