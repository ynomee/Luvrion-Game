using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.UI;

public class HintManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _hintText;
    [SerializeField] private Image _hintBackground;
    [SerializeField] private float _fadeDuration = 0.5f;

    private CanvasGroup _textCanvasGroup;
    private CanvasGroup _backgroundCanvasGroup;

    private void Start()
    {
        if (_hintText == null)
        {
            _hintText = GetComponentInChildren<TextMeshProUGUI>();
            if (_hintText == null)
            {
                Debug.LogError("TextMeshProUGUI не найден!");
                enabled = false;
            }
        }

        if (_hintBackground == null)
        {
            _hintBackground = GetComponentInChildren<Image>();
            if (_hintBackground == null)
            {
                Debug.LogError("HintBackground не найден!");
                enabled = false;
            }
        }

        // Получаем или создаем CanvasGroup для текста
        _textCanvasGroup = _hintText.GetComponent<CanvasGroup>();
        if (_textCanvasGroup == null)
        {
            _textCanvasGroup = _hintText.gameObject.AddComponent<CanvasGroup>();
        }

        // Получаем или создаем CanvasGroup для фона
        _backgroundCanvasGroup = _hintBackground.GetComponent<CanvasGroup>();
        if (_backgroundCanvasGroup == null)
        {
            _backgroundCanvasGroup = _hintBackground.gameObject.AddComponent<CanvasGroup>();
        }

        // Начальная прозрачность
        _textCanvasGroup.alpha = 0f;
        _backgroundCanvasGroup.alpha = 0f;
    }

    public void ShowHint(string hint)
    {
        if (_hintText != null && _hintBackground != null)
        {
            _hintText.text = hint;
            StopAllCoroutines();
            StartCoroutine(Fade(_textCanvasGroup, 1f));
            StartCoroutine(Fade(_backgroundCanvasGroup, 1f));
        }
    }

    public void HideHint()
    {
        StopAllCoroutines();
        StartCoroutine(Fade(_textCanvasGroup, 0f));
        StartCoroutine(Fade(_backgroundCanvasGroup, 0f));
    }

    private IEnumerator Fade(CanvasGroup canvasGroup, float targetAlpha)
    {
        float startAlpha = canvasGroup.alpha;
        float time = 0f;

        while (time < _fadeDuration)
        {
            time += Time.deltaTime;
            canvasGroup.alpha = Mathf.Lerp(startAlpha, targetAlpha, time / _fadeDuration);
            yield return null;
        }

        canvasGroup.alpha = targetAlpha;
    }
}