using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class SceneFader : MonoBehaviour
{
    public float fadeTime;

    private Image _fadeOutUIImage;

    public enum FadeDirection
    {
        In,
        Out
    }

    private void Awake()
    {
        _fadeOutUIImage = GetComponent<Image>();
    }

    public IEnumerator Fade(FadeDirection fadeDirection)
    {
        float alpha = fadeDirection == FadeDirection.Out ? 1 : 0;
        float fadeEndValue = fadeDirection == FadeDirection.Out ? 0 : 1;

        if (fadeDirection == FadeDirection.Out)
        {
            while (alpha >= fadeEndValue)
            {
                SetColorImage(ref alpha, fadeDirection);

                yield return null;
            }

            _fadeOutUIImage.enabled = false;
        }
        else
        {
            _fadeOutUIImage.enabled = true;

            while(alpha <= fadeEndValue)
            {
                SetColorImage(ref alpha, fadeDirection);

                yield return null;
            }
        }

    }

    public IEnumerator FadeAndLoadScene(FadeDirection fadeDirection, string sceneToLoad)
    {
        _fadeOutUIImage.enabled = true;

        yield return Fade(fadeDirection);

        SceneManager.LoadScene(sceneToLoad);
    }

    private void SetColorImage(ref float alpha, FadeDirection fadeDirection)
    {
        _fadeOutUIImage.color = new Color(_fadeOutUIImage.color.r, _fadeOutUIImage.color.g,_fadeOutUIImage.color.b, alpha);

        alpha += Time.deltaTime * (1 / fadeTime) * (fadeDirection == FadeDirection.Out ? -1 :  1); 
    }
}
