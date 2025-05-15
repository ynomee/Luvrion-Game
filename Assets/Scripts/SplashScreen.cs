using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;
using System.Collections; // Для IEnumerator

public class SplashScreen : MonoBehaviour
{
    [SerializeField] private VideoPlayer _videoPlayer;
    [SerializeField] private SceneFader _sceneFader;
    [SerializeField] private float _fadeInTime = 1.5f; // Время осветления
    [SerializeField] private float _fadeOutTime = 1.5f; // Время затемнения перед переходом

    private void Start()
    {
        // Начинаем с затемнённого экрана
        //_videoPlayer.url = System.IO.Path.Combine(Application.streamingAssetsPath, "your_video.mp4");
        _videoPlayer.loopPointReached += OnVideoEnd;
        
        // Запускаем осветление -> воспроизведение видео
        StartCoroutine(PlaySplashWithFade());
    }

    private IEnumerator PlaySplashWithFade()
    {
        // Осветление (Fade In)
        yield return new WaitForSeconds(0.5f); // Пауза перед Fade In
        yield return _sceneFader.Fade(SceneFader.FadeDirection.Out);
        
        // Запуск видео после осветления
        _videoPlayer.Play();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StopAllCoroutines();
            StartCoroutine(_sceneFader.FadeAndLoadScene(SceneFader.FadeDirection.In, "1stRoom"));
        }
    }
    
    private void OnVideoEnd(VideoPlayer vp)
    {
        // Затемнение (Fade Out) и переход на следующую сцену
        StartCoroutine(_sceneFader.FadeAndLoadScene(SceneFader.FadeDirection.In, "1stRoom"));
    }
}