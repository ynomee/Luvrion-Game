using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro; // Если используете TextMeshPro для текста

public class SceneTransitionTimer : MonoBehaviour
{
    [Tooltip("Время в секундах, которое должно пройти до разблокировки хоткея.")]
    public float unlockDelay = 30f;

    [Tooltip("Текст подсказки, отображаемый игроку.")]
    public string unlockHint = "Нажмите 'T', чтобы телепортироваться.";

    [Tooltip("Ссылка на UI элемент, где будет отображаться подсказка (TextMeshPro или Text).")]
    public TextMeshProUGUI hintText; // Или public Text hintText;

    [Tooltip("Координаты для телепортации игрока.")]
    public Vector3 teleportPosition = Vector3.zero; // Начальная позиция - начало координат
    
    [SerializeField] private GameObject _teleportHintPanel; //подложка для текста
    
    [Tooltip("Тег игрока, чтобы скрипт знал, кого телепортировать")]
    public string playerTag = "Player";
    
    private bool hotkeyUnlocked = false;
    private bool timerStarted = false;

    // Ключ для хранения времени начала сцены в PlayerPrefs
    private const string SceneStartTimeKey = "SceneStartTime";

    void Start()
    {
        // Проверяем, загружена ли сцена впервые. Если да, сохраняем время начала.
        if (!PlayerPrefs.HasKey(SceneStartTimeKey + SceneManager.GetActiveScene().name))
        {
            PlayerPrefs.SetFloat(SceneStartTimeKey + SceneManager.GetActiveScene().name, Time.time);
            PlayerPrefs.Save(); // Важно сохранить изменения
            timerStarted = true; // Timer started only on scene start.
        }
        else
        {
            timerStarted = true; // Timer started before.
        }
        
        hotkeyUnlocked = false;

        // Убедимся, что UI элемент не активен при старте
        if (hintText != null)
        {
            hintText.gameObject.SetActive(false);
        }
        else
        {
            Debug.LogError("Необходимо назначить UI элемент для отображения подсказки!");
        }
    }

    void Update()
    {
        if (Time.timeScale == 0f) // Если игра в паузе, ничего не делаем
        {
            return;
        }
        
        if (!timerStarted)
        {
            return; // Ensure timer only runs once.
        }

        // Вычисляем прошедшее время с момента начала сцены
        float timeSinceSceneStart = Time.time - PlayerPrefs.GetFloat(SceneStartTimeKey + SceneManager.GetActiveScene().name);

        if (!hotkeyUnlocked && timeSinceSceneStart >= unlockDelay)
        {
            UnlockHotkey();
        }

        // Пример использования хоткея после разблокировки
        if (hotkeyUnlocked && Input.GetKeyDown(KeyCode.T))
        {
            TeleportPlayer();
        }
    }

    void UnlockHotkey()
    {
        hotkeyUnlocked = true;

        // Отображаем подсказку на экране
        if (hintText != null)
        {
            hintText.text = unlockHint;
            hintText.gameObject.SetActive(true);
        }
        if (_teleportHintPanel != null)
        {
            _teleportHintPanel.SetActive(true);
        }

        Debug.Log("Хоткей скипа разблокирован!");
    }

    void TeleportPlayer()
    {
        // Находим игрока по тегу
        GameObject player = GameObject.FindGameObjectWithTag(playerTag);

        if (player != null)
        {
            // Телепортируем игрока
            player.transform.position = teleportPosition;
            Debug.Log("Игрок телепортирован в координаты: " + teleportPosition);
        }
        else
        {
            Debug.LogError("Игрок с тегом '" + playerTag + "' не найден!");
        }
    }

    // Функция, вызываемая при переходе на другую сцену
    void OnDestroy()
    {
        // Очищаем данные из PlayerPrefs при уничтожении скрипта.
        // Это позволяет таймеру запуститься заново при повторном возвращении на эту сцену.
        PlayerPrefs.DeleteKey(SceneStartTimeKey + SceneManager.GetActiveScene().name);
        PlayerPrefs.Save();
    }
}