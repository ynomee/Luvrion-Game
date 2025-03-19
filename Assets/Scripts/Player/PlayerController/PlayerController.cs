using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private PlayerView _playerView;
    [SerializeField] private PlayerMovement _playerMovement;
    [SerializeField] private CheckPoint _checkPoint;
    [SerializeField] private GameObject _checkPointObj;

    private PlayerModel _playerModel;

    private void Awake()
    {
        _playerModel = new PlayerModel();

        _playerView.Initialize(_playerModel);
        _playerMovement.Initialize(_playerModel);
        //_checkPoint.Initialize(_playerModel);
    }
    private void Start()
    {
        FindCheckPoint();
    }

    private void FindCheckPoint()
    {
    _checkPoint = FindObjectOfType<CheckPoint>();

    if (_checkPoint != null)
    {
        _checkPoint.Initialize(_playerModel);
        Debug.Log("CheckPoint найден и инициализирован.");
    }
    else
    {
        Debug.LogWarning("CheckPoint не найден в сцене!");
    }
    }
}
