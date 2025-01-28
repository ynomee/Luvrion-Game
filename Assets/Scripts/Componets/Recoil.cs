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
    private float _defaultGravity;

    private void Awake()
    {

    }

    private void Start()
    {
    
    }

    private void FixedUpdate()
    {

    }

    public void HandleRecoil(float yAxis, float bonusJumpsLeft, float gravityScale, float lastOnGroundTime)
    {
        gravityScale = rb.gravityScale;

        if (pState.recoilingX)
        {
            if (pState.lookingRight)
            {
                rb.velocity = new Vector2( -_recoilXSpeed,0);
            }
            else
            {
                rb.velocity = new Vector2( _recoilXSpeed, 0);
            }
        }

        if (pState.recoilingY)
        {
            gravityScale = 0;
            if (yAxis < 0)
            {
                rb.velocity = new Vector2(rb.velocity.x, _recoilYSpeed);
            }
            else
            {
                rb.velocity = new Vector2(rb.velocity.x, -_recoilYSpeed);
            }
            bonusJumpsLeft = 0;
            
        }
        else
        {
            gravityScale = rb.gravityScale;
        }

        if (pState.recoilingX && stepsXRecoiled < _recoilXSteps) 
        {
            stepsXRecoiled++;
        }
        else
        {
            StopRecoilX();
            //StopRecoiling(stepsXRecoiled = 0, pState.recoilingX = false);
        }

        if (pState.recoilingY && stepsYRecoiled < _recoilYSteps) 
        {
            stepsYRecoiled++;
        }
        else
        {
            //StopRecoiling(stepsYRecoiled = 0, pState.recoilingY = false);
            StopRecoilY();
        }

        if (lastOnGroundTime > 0f)
        {
            //StopRecoiling(stepsYRecoiled = 0, pState.recoilingY = false);
            StopRecoilY();
        }
    }

    private void StopRecoilX()
    {
        stepsXRecoiled = 0;
        pState.recoilingX = false;
    }

    private void StopRecoilY()
    {
        stepsYRecoiled = 0;
        pState.recoilingY = false;
    }

    private void StopRecoiling (int stepsRecoiled, bool recoiling) {return;}
}
