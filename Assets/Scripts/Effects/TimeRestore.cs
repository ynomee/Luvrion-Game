using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TimeRestore : MonoBehaviour
{
    private bool _restoreTime;
    private float _restoreTimeSpeed;

    private void Update()
    {
        RestoreTimeScale();
    }

    private void RestoreTimeScale()
    {
        if (_restoreTime) 
        {
            if (Time.timeScale < 1)
            {
                Time.timeScale += Time.unscaledDeltaTime * _restoreTimeSpeed;
            }
            else 
            {
                Time.timeScale = 1;
                _restoreTime = false;
            }
        }
    }

    public void HitStopTime(float newTimeScale, int restoreSpeed, float delay)
    {
        _restoreTimeSpeed = restoreSpeed;
        Time.timeScale = newTimeScale;

        if (delay > 0)
        {
            StopCoroutine(StartTimeAgain(delay));
            StartCoroutine(StartTimeAgain(delay));
        }
        else
        {
            _restoreTime = true;
        }
    }

    private IEnumerator StartTimeAgain(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        _restoreTime = true;
    }
}
