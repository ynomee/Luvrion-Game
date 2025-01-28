using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashEffect : MonoBehaviour
{
    [SerializeField] public PlayerStateList _pState;

    [SerializeField] private float _hitFlashSpeed;
    [SerializeField] private SpriteRenderer _spriteRenderer;

    private void Start()
    {
        _pState = GetComponent<PlayerStateList>();
        _spriteRenderer = GetComponentInChildren<SpriteRenderer>();
    }
    private void Update()
    {
        FlashWhileInvinsible();
    }

    private void FlashWhileInvinsible()
    {
        _spriteRenderer.material.color = _pState.invinsible ? Color.Lerp(Color.white, Color.black, Mathf.PingPong(Time.time * _hitFlashSpeed, 1f)) : Color.white;
    }
}
