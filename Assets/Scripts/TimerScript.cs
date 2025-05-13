using UnityEngine;
using TMPro; // Подключаем пространство имен TextMeshPro

public class Timer : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _timerText; // Ссылка на UI Text для отображения таймера
    [SerializeField] private bool _isPaused = false;   // Флаг паузы таймера

    private float _elapsedTime = 0f; // Прошедшее время

    private void Start()
    {
        // Находим ссылку на TextMeshPro, если она не назначена в инспекторе
        if (_timerText == null)
        {
            _timerText = GetComponentInChildren<TextMeshProUGUI>();
            if (_timerText == null)
            {
                Debug.LogError("TextMeshProUGUI не найден!");
            }
        }
        _elapsedTime = 0f;
        UpdateTimerText(); // Обновляем текст таймера при старте
    }

    private void Update()
    {
        if (!_isPaused)
        {
            _elapsedTime += Time.unscaledDeltaTime; // Используем Time.unscaledDeltaTime, чтобы таймер работал во время Time.timeScale = 0
            UpdateTimerText(); // Обновляем текст таймера
        }
    }

    // Метод для форматирования времени в строку
    public string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60);
        int seconds = Mathf.FloorToInt(time % 60);
        int milliseconds = Mathf.FloorToInt((time * 1000) % 1000); // Добавляем миллисекунды

        // Форматируем строку
        return string.Format("{0:00}:{1:00}:{2:000}", minutes, seconds, milliseconds);
    }

    // Метод для обновления текста таймера
    private void UpdateTimerText()
    {
        if (_timerText != null)
        {
            _timerText.text = FormatTime(_elapsedTime);
        }
        else
        {
            Debug.LogError("TextMeshProUGUI не назначен!");
        }
    }

    // Метод для постановки таймера на паузу/возобновления
    public void PauseTimer(bool pause)
    {
        _isPaused = pause;
    }

    public void ClearTimer()
    {
        _elapsedTime = 0f;
    }

    public float GetElapsedTime()
    {
        return _elapsedTime;
    }
}