using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthComponent : MonoBehaviour
{
    public PlayerStateList pState;

    [Header("Health Settings")]
    [SerializeField] private int _health;
    public delegate void OnHealthChangedDelegate();
    [HideInInspector] public OnHealthChangedDelegate onHealthChangeCallback;
    
    public int maxHealth;

    [Header("Visual Effects")]
    [SerializeField] GameObject _bloodSpurt;

    private void Awake()
    {
        Health = maxHealth;
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
                _health = Mathf.Clamp(value, 0, maxHealth);

                if (onHealthChangeCallback != null)
                {
                    onHealthChangeCallback.Invoke();
                }
            }
        }
    }

    private IEnumerator StopTakingDamage()
    {
        pState.invinsible = true;
        
        // Blood particle system instantiation
        GameObject bloodSpurtParticles = Instantiate(_bloodSpurt, transform.position, Quaternion.identity);
        Destroy(bloodSpurtParticles, 1.5f);

        yield return new WaitForSeconds(1f);

        pState.invinsible = false;
    }
}
