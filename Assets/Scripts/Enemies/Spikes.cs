using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Spikes : MonoBehaviour
{
    [SerializeField] private PlayerStateList _pstate;
    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private HealthComponent _health;
    [SerializeField] private PlayerView _pv;

    [Tooltip("Коллайдер, к которому телепортируется игрок после столкновения с шипами.")]
    [SerializeField] private Collider2D _respawnCollider;
    
    private GameObject _playerObj;

    private void Start()
    {
        _playerObj = PlayerSingleton.Instance.player;

        _rb = _playerObj.GetComponent<Rigidbody2D>();
        _pstate = _playerObj.GetComponent<PlayerStateList>();
        _health = _playerObj.GetComponent<HealthComponent>();
        _pv = _playerObj.GetComponent<PlayerView>();

        //if (_respawnCollider == null)
        //{
        //    Debug.LogError("Не назначен коллайдер для телепортации игрока.");
        //}
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && _respawnCollider != null)
        {
            StartCoroutine(RespawnPoint());
        }
    }

    IEnumerator RespawnPoint()
    {
        _pstate.cutScene = true;
        _pstate.invinsible = true;
        
        // Забираем контроль над персонажем
        PlayerInput playerInput = _playerObj.GetComponent<PlayerInput>();
        if (playerInput != null)
        {
            playerInput.enabled = false;
        }
        
        PlayerMovement pm = _playerObj.GetComponent<PlayerMovement>();
        if (pm != null)
        {
            if (pm.DashCoroutine != null)
            {
                StopCoroutine(pm.DashCoroutine);
            }
            pm._isDashAttacking = false;
            pm.SetGravityScale(pm.Data.gravityScale);
            pm.IsDashing = false;
            _pv.OnStopDash();
        }
        else
        {
            Debug.LogError("PlayerMovement not found.");
        }
        
        _rb.velocity = Vector2.zero;
        Time.timeScale = 0;

        StartCoroutine(UIManager.Instance.sceneFader.Fade(SceneFader.FadeDirection.In));
        _health.TakeDamage(1);

        yield return new WaitForSecondsRealtime(0.5f);
        Time.timeScale = 1;

        _rb.transform.position = _respawnCollider.bounds.center;
        
        
        
        if (_pv != null)
        {
            _pv.OnWallJumpEnd();
            _pv.OnLand();
            _pv.OnRespawn();
        }
        else
        {
            Debug.LogError("Animator не найден!");
        }

        StartCoroutine(UIManager.Instance.sceneFader.Fade(SceneFader.FadeDirection.Out));
        yield return new WaitForSecondsRealtime(UIManager.Instance.sceneFader.fadeTime);

        _pstate.cutScene = false;
        _pstate.invinsible = false;
        
        // Возвращаем контроль над персонажем
        if (playerInput != null)
        {
            playerInput.enabled = true;
        }

    }
}