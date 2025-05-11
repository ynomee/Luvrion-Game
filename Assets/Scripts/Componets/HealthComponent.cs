using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HealthComponent : MonoBehaviour
{
    public PlayerStateList pState;
    public DeathComponent deathComponent;

    [Header("Health Settings")]
    [SerializeField] private int _health;
    public delegate void OnHealthChangedDelegate();
    [HideInInspector] public OnHealthChangedDelegate onHealthChangeCallback;
    
    public int maxHealth;

    [Header("Visual Effects")]
    [SerializeField] private GameObject _bloodSpurt;

    private void Awake()
    {
        Health = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        if(pState.alive)
        {
            Health -= Mathf.RoundToInt(damage);
            if (Health <= 0)
            {
                Health = 0;
                StartCoroutine(deathComponent.Death());
            }
            else
            {
                StartCoroutine(StopTakingDamage()); 
            }
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

    public int GetCurrentDeath()
    {
        return maxHealth - Health;
    }

    public void RestoreFullHealth()
    {
        Health = maxHealth;
    }
}
