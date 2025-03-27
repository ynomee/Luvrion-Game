using System.Collections;
using System.Collections.Generic;
//using UnityEditor.UI;
using UnityEngine;
using UnityEngine.InputSystem;

public class CheckPoint : MonoBehaviour
{
    [SerializeField] private HealthComponent _health;
    [SerializeField] private PlayerStateList _pstate;

    private PlayerModel _playerModel;
    private GameObject _playerObj;
    public bool _interacted;
    public bool inRange = false;

    public void Initialize(PlayerModel model)
    {
        _playerModel = model;
    }

    private void Start()
    {
        _playerObj = PlayerSingleton.Instance.player;

        _health = _playerObj.GetComponent<HealthComponent>();
        _pstate = _playerObj.GetComponent<PlayerStateList>();
        GameManager.Instance.checkpoint = GetComponent<CheckPoint>();
    }

    public void Respawned()
    {
        if (!_pstate.alive)
        {
           _pstate.alive = true;
           _health.Health = _health.maxHealth; 
           _playerModel.Respawn();
        }
    }

    public void Interact()
    {
        _interacted = true;
        Debug.Log("Чекпоинт активирован!");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            if (GameManager.Instance.checkpoint != this)
            {
                GameManager.Instance.checkpoint = this;
                inRange = true;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            if (GameManager.Instance.checkpoint != this)
            {
                GameManager.Instance.checkpoint = this;
                inRange = false;
            }
        }
    }
}
