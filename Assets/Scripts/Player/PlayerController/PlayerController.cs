using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerView _playerView;
    [SerializeField] private PlayerMovement _playerMovement;

    private PlayerModel _playerModel;

    private void Awake()
    {
        _playerModel = new PlayerModel();

        _playerView.Initialize(_playerModel);
        _playerMovement.Initialize(_playerModel);
    }
}
