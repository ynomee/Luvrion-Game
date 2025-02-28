using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spikes : MonoBehaviour
{
    [SerializeField] private PlayerStateList _pstate;
    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private HealthComponent _health;

    private GameObject playerObj;

    private void Start()
    {
        playerObj = PlayerSingleton.Instance.player;

        _rb = playerObj.GetComponent<Rigidbody2D>();
        _pstate = playerObj.GetComponent<PlayerStateList>();
        _health = playerObj.GetComponent<HealthComponent>();

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
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

        _rb.transform.position = GameManager.Instance.platformingRespawnPoint;

        StartCoroutine(UIManager.Instance.sceneFader.Fade(SceneFader.FadeDirection.Out));
        yield return new WaitForSecondsRealtime(UIManager.Instance.sceneFader.fadeTime);

        _pstate.cutScene = false;
        _pstate.invinsible = false;

    }
}
