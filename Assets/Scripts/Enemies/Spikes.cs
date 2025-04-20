using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spikes : MonoBehaviour
{
    [SerializeField] private PlayerStateList _pstate;
    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private HealthComponent _health;

    [Tooltip("Коллайдер, к которому телепортируется игрок после столкновения с шипами.")]
    [SerializeField] private Collider2D _respawnCollider;
    
    private GameObject _playerObj;

    private void Start()
    {
        _playerObj = PlayerSingleton.Instance.player;

        _rb = _playerObj.GetComponent<Rigidbody2D>();
        _pstate = _playerObj.GetComponent<PlayerStateList>();
        _health = _playerObj.GetComponent<HealthComponent>();

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

        _rb.velocity = Vector2.zero;
        Time.timeScale = 0;

        StartCoroutine(UIManager.Instance.sceneFader.Fade(SceneFader.FadeDirection.In));
        _health.TakeDamage(1);

        yield return new WaitForSecondsRealtime(0.5f);
        Time.timeScale = 1;

        _rb.transform.position = _respawnCollider.bounds.center;

        StartCoroutine(UIManager.Instance.sceneFader.Fade(SceneFader.FadeDirection.Out));
        yield return new WaitForSecondsRealtime(UIManager.Instance.sceneFader.fadeTime);

        _pstate.cutScene = false;
        _pstate.invinsible = false;

    }
}