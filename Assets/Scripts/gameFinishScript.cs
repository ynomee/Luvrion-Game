using UnityEngine;

public class Finish : MonoBehaviour
{
    [SerializeField] private GameOverManager _gameOverManager; // Ссылка на GameOverManager
    [SerializeField] private GameObject _player;
    [SerializeField] private HealthComponent _health;
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        _gameOverManager = FindObjectOfType<GameOverManager>();
        _player = other.gameObject;
        if (other.CompareTag("Player"))
        {
            // Получаем время игры
            float gameTime = FindObjectOfType<Timer>().GetElapsedTime();
            
            // Получаем количество смертей
            _health = _player.GetComponent<HealthComponent>();
            int deathCount = _health.GetCurrentDeath();

            // Отображаем окно GameOver
            _gameOverManager.ShowGameOver(gameTime, deathCount);
        }
    }
}