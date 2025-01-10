using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerView _playerView;
    [SerializeField] private PlayerMovement _playerMovement;

    public PlayerModel _playerModel;

    private void Awake()
    {
        // Создаем модель
        _playerModel = new PlayerModel();

        // Передаем модель в View и Movement
        _playerView._playerModel = _playerModel;
        _playerModel.RegisterObserver(_playerView);

        _playerMovement._playerModel = _playerModel;
    }

    private void OnDestroy()
    {
        // Освобождаем ресурсы
        if (_playerModel != null)
        {
            _playerModel.UnregisterObserver(_playerView);
        }
    }
}
