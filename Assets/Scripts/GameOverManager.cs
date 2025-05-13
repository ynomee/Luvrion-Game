using UnityEngine;
using UnityEngine.UI; // Нужно для работы с UI элементами
using TMPro; // Подключаем пространство имен TextMeshPro
using UnityEngine.SceneManagement; // Нужно для перезапуска сцены
using UnityEngine.InputSystem;

public class GameOverManager : MonoBehaviour
{
    [SerializeField] private GameObject _gameOverPanel; // Ссылка на панель GameOver
    [SerializeField] private TextMeshProUGUI _timerText; // Ссылка на TextMeshPro для отображения таймера
    [SerializeField] private TextMeshProUGUI _deathCountText; // Ссылка на TextMeshPro для отображения количества смертей
    [SerializeField] private TextMeshProUGUI _scoreText;
    [SerializeField] private Button _restartButton; // Ссылка на кнопку "Restart"
    [SerializeField] private Button _exitButton; // Ссылка на кнопку "Exit"
    [SerializeField] private Timer _timer;  // Ссылка на скрипт Timer
    [SerializeField] private HealthComponent _health; // Ссылка на скрипт HealthComponent
    [SerializeField] private GameObject _player; //Ссылка на игрока
    [SerializeField] private PlayerStateList _pState;
    [SerializeField] private float base_score;
    [SerializeField] private float timeWeight;
    [SerializeField] private float deathWeight;
    
    private Vector3 _startPosition; // Стартовая позиция игрока
    private float _startTimeScale;

    private void Start()
    {
        // Находим ссылки на кнопки, если они не назначены в инспекторе
        if (_restartButton == null)
        {
            _restartButton = transform.Find("GameOverPanel/RestartButton").GetComponent<Button>();
        }
        if (_exitButton == null)
        {
            _exitButton = transform.Find("GameOverPanel/ExitButtonGameOver").GetComponent<Button>();
        }
        if (_timer == null)
        {
            _timer = FindObjectOfType<Timer>();
        }
        if (_health == null)
        {
            _health = _player.GetComponent<HealthComponent>();
        }
        // Добавляем обработчики событий для кнопок
        if (_restartButton != null)
        {
            _restartButton.onClick.AddListener(RestartGame);
        }
        else
        {
            Debug.LogError("RestartButton не назначен!");
        }

        if (_exitButton != null)
        {
            _exitButton.onClick.AddListener(ExitGame);
        }
        else
        {
            Debug.LogError("ExitButton не назначен!");
        }

        // Скрываем панель в начале игры
        HideGameOverPanel();
        if(_player != null)
        {
            _startPosition = _player.transform.position;
        }
        _startTimeScale = Time.timeScale;
    }

    // Метод для отображения окна GameOver
    public void ShowGameOver(float gameTime, int deathCount)
    {
        _startTimeScale = Time.timeScale;
        Time.timeScale = 0f;
        _timer.PauseTimer(true);
        
        // Забираем контроль над персонажем
        PlayerInput playerInput = _player.GetComponent<PlayerInput>();
        if (playerInput != null)
        {
            playerInput.enabled = false;
        }
        
        if (_gameOverPanel != null)
        {
            _gameOverPanel.SetActive(true);
        }
        else
        {
            Debug.LogError("GameOverPanel не назначен!");
        }

        PauseMenu pMenu = FindObjectOfType<PauseMenu>();
        if (pMenu != null)
        {
            pMenu.setGameOverStatus(true);
        }
        
        if (_timerText != null)
        {
            _timerText.text = _timer.FormatTime(gameTime); // Отображаем время
        }
        else
        {
            Debug.LogError("TimerText не назначен!");
        }

        if (_deathCountText != null)
        {
            _deathCountText.text = "" + deathCount; // Отображаем количество смертей
        }
        else
        {
            Debug.LogError("DeathCountText не назначен!");
        }
        
        if (_scoreText != null)
        {
            float score = base_score /
                          (1 + timeWeight * Mathf.Log(1 + gameTime) + deathWeight * Mathf.Log(1 + deathCount));
            score = Mathf.Round(score * 100.0f) / 0.01f;
            _scoreText.text = "" + score;
        }
        else
        {
            Debug.LogError("scoreText не назначен!");
        }
    }

    // Метод для скрытия окна GameOver
    private void HideGameOverPanel()
    {
        if (_gameOverPanel != null)
        {
            _gameOverPanel.SetActive(false);
        }
    }

    // Метод для перезапуска игры
    private void RestartGame()
    {
        Time.timeScale = _startTimeScale;
        // Обнуляем таймер
        if (_timer != null)
        {
            _timer.PauseTimer(false);
            _timer.ClearTimer();  // Сбрасываем таймер
        }
        
        PauseMenu pMenu = FindObjectOfType<PauseMenu>();
        if (pMenu != null)
        {
            pMenu.setGameOverStatus(false);
        }
        
        // Сбрасываем жизни
        if (_health != null)
        {
            _health.RestoreFullHealth();
        }

        // Возвращаем игрока на стартовую позицию
        if(_player != null)
        {
            _player.transform.position = _startPosition;
        }

        HideGameOverPanel(); // скрываем окно итогов
        
        if(_pState != null)
        {
            //_pState.ResetPlayerState(); TODO ???
        }
        
        // Возвращаем контроль над персонажем
        PlayerInput playerInput = _player.GetComponent<PlayerInput>();
        if (playerInput != null)
        {
            playerInput.enabled = true;
        }
        
        SceneManager.LoadScene("1stRoom"); // Загружаем первую сцену
        _pState.lookingRight = true; // Разворачиваем игрока вправо
    }

    // Метод для выхода из игры
    private void ExitGame()
    {
        // В редакторе Unity
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif

        // В билде
        Application.Quit();
    }

}