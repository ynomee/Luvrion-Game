using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject _deathScreen;
    public static UIManager Instance { get; private set; }
    public SceneFader sceneFader;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
        DontDestroyOnLoad(gameObject);

        sceneFader = GetComponentInChildren<SceneFader>();
    }

    public IEnumerator ActivateDeathScreen()
    {
         yield return new WaitForSeconds(0.8f);

         StartCoroutine(sceneFader.Fade(SceneFader.FadeDirection.In));

         yield return new WaitForSeconds(0.8f);
         _deathScreen.SetActive(true);
    }

    public IEnumerator DeactivateFadeScreen()
    {
        yield return new WaitForSeconds(0.5f);
        _deathScreen.SetActive(false);
        StartCoroutine(sceneFader.Fade(SceneFader.FadeDirection.Out));
    }
}
