using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IPlayerObserver
{
    void OnSpeedChanged(float speed);
    void OnJump();
    void OnLand();
}

public class PlayerModel
{
    private readonly List<IPlayerObserver> _observers = new List<IPlayerObserver>();

    private float _speed;

    public void RegisterObserver(IPlayerObserver observer)
    {
        if (!_observers.Contains(observer))
        {
            _observers.Add(observer);
        }
    }

    public void UnregisterObserver(IPlayerObserver observer)
    {
        _observers.Remove(observer);
    }

    public void UpdateSpeed(float newSpeed)
    {
        if (Mathf.Approximately(_speed, newSpeed)) return;
        _speed = newSpeed;

        foreach (var observer in _observers)
        {
            observer.OnSpeedChanged(_speed);
        }
    }

    public void TriggerJump()
    {
        foreach (var observer in _observers)
        {
            observer.OnJump();
        }
    }

    public void TriggerLand()
    {
        foreach (var observer in _observers)
        {
            observer.OnLand();
        }
    }

}
