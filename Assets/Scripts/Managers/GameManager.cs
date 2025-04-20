using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public Vector2 respawnPoint;
    [HideInInspector] public Vector2 platformingRespawnPoint;
    [HideInInspector] public string transitionedFromScene;

    public CheckPoint checkpoint;
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        SaveData.Instance.Initialize();

        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        SaveScene();
        DontDestroyOnLoad(gameObject);
        
        checkpoint = FindObjectOfType<CheckPoint>();
    }

    private void Start()
    {
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursor.visible = false;
    }

    public void SaveScene()
    {
        string currentSceneName = SceneManager.GetActiveScene().name;
        SaveData.Instance.sceneNames.Add(currentSceneName);
    }

    public void RespawnPlayer()
    {
        Debug.Log("Кнопка респавна нажата");
        if (checkpoint != null)
        {
            if (checkpoint._interacted)   
                respawnPoint = checkpoint.transform.position;           
        }
        else
            respawnPoint = platformingRespawnPoint;

        PlayerSingleton.Instance.player.transform.position = respawnPoint;
        StartCoroutine(UIManager.Instance.DeactivateFadeScreen());
        checkpoint.Respawned();
    } 

}
