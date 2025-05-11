using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private GameObject _pauseMenuPanel; // Ссылка на панель меню паузы
    [SerializeField] private Button _resumeButton; // Ссылка на кнопку "Продолжить"
    [SerializeField] private Button _exitButton;   // Ссылка на кнопку "Выйти"
    [SerializeField] private Timer _timer;
    [SerializeField] private Button _pauseButton;
    [SerializeField] private Sprite pauseSprite;
    [SerializeField] private Sprite resumeSprite;

    private bool _isPaused = false; // Флаг, показывающий, находится ли игра в паузе
    private bool _isGameOvered = false;
    
    private void Start()
    {
        if (_resumeButton == null)
        {
            _resumeButton = transform.Find("PauseMenuPanel/ResumeButton").GetComponent<Button>();
        }
        if (_exitButton == null)
        {
            _exitButton = transform.Find("PauseMenuPanel/ExitButton").GetComponent<Button>();
        }
        if (_pauseButton == null)
        {
            _pauseButton = transform.Find("PauseButtonUI").GetComponent<Button>();
        }
        
        if (_resumeButton != null)
        {
            _resumeButton.onClick.AddListener(ResumeGame);
        }
        else
        {
            Debug.LogError("ResumeButton не назначен!");
        }

        if (_exitButton != null)
        {
            _exitButton.onClick.AddListener(ExitGame);
        }
        else
        {
            Debug.LogError("ExitButton не назначен!");
        }
        
        if (_pauseButton != null)
        {
            _pauseButton.onClick.AddListener(TogglePause);
        }
        else
        {
            Debug.LogError("ExitButton не назначен!");
        }
        
        HidePauseMenu();
    }


    private void Update()
    {
        // Обработка нажатия клавиши Esc
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        if (!_isGameOvered)
        {
            if (_isPaused)
            {
                ResumeGame(); // Если игра в паузе, возобновляем
            }
            else
            {
                PauseGame(); // Иначе ставим на паузу
            }
        }
    }
    
    // Метод для постановки игры на паузу
    private void PauseGame()
    {
        _isPaused = true;
        Time.timeScale = 0f; // Останавливаем время
        GameObject playerObj = PlayerSingleton.Instance.player;
        PlayerInput playerInput = playerObj.GetComponent<PlayerInput>();
        _pauseButton.image.sprite = pauseSprite;
        EventSystem.current.SetSelectedGameObject(null);
        if (playerInput != null)
        {
            playerInput.enabled = false;
        }
        
        ShowPauseMenu();  // Показываем меню паузы
        _timer.PauseTimer(true); // Ставим таймер на паузу
        
    }

    // Метод для возобновления игры
    private void ResumeGame()
    {
        _isPaused = false;
        Time.timeScale = 1f; // Восстанавливаем время
        GameObject playerObj = PlayerSingleton.Instance.player;
        PlayerInput playerInput = playerObj.GetComponent<PlayerInput>();
        _pauseButton.image.sprite = resumeSprite;
        EventSystem.current.SetSelectedGameObject(null);
        if (playerInput != null)
        {
            playerInput.enabled = true;
        }
        HidePauseMenu();  // Скрываем меню паузы
        _timer.PauseTimer(false); // Возобновляем таймер
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

    // Метод для отображения меню паузы
    private void ShowPauseMenu()
    {
        if (_pauseMenuPanel != null)
        {
            _pauseMenuPanel.SetActive(true);
        }
        else
        {
            Debug.LogError("PauseMenuPanel не назначен!");
        }
    }

    // Метод для скрытия меню паузы
    private void HidePauseMenu()
    {
        if (_pauseMenuPanel != null)
        {
            _pauseMenuPanel.SetActive(false);
        }
        else
        {
            Debug.LogError("PauseMenuPanel не назначен!");
        }
    }

    public void setGameOverStatus(bool isGameOver)
    {
        _isGameOvered = isGameOver;
    }
    
}