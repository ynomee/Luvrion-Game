using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthComponent : MonoBehaviour
{
    public PlayerStateList pState;

    [Header("Health Settings")]
    [SerializeField] private int _health;
    [SerializeField] private int _maxHealth;

    private void Awake()
    {
        Health = _maxHealth;
    }

    public void TakeDamage(float damage)
    {
        Health -= Mathf.RoundToInt(damage);
        Debug.Log($"Player damaged, health: {Health}");
        StartCoroutine(StopTakingDamage());
        if (Health <= 0)
        {
            Debug.Log("Player dead");
        }
    }

    public int Health
    {
        get {return _health; }
        set
        {
            if (_health != value)
            {
                _health = Mathf.Clamp(value, 0, _maxHealth);
            }
        }
    }

    private IEnumerator StopTakingDamage()
    {
        pState.invinsible = true;

        yield return new WaitForSeconds(1f);

        pState.invinsible = false;
    }
}
