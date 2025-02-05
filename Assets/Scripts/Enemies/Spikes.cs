using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spikes : MonoBehaviour
{
    [SerializeField] private PlayerStateList _pstate;
    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private HealthComponent _health;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
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
        _pstate.invinsible = true;
        _rb.velocity = Vector2.zero;
        Time.timeScale = 0;
        _health.TakeDamage(1);

        yield return new WaitForSecondsRealtime(1);
        _rb.transform.position = GameManager.Instance.platformingRespawnPoint;
        _pstate.invinsible = false;
        Time.timeScale = 1;

    }
}
