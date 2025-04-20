using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTransition : MonoBehaviour
{
    [SerializeField] private string _transitionTo;
    [SerializeField] private Transform _startPoint;

    [SerializeField] private PlayerStateList pState;
    [SerializeField] private Rigidbody2D RB;
    [SerializeField] private PlayerMovement pm;
    
    //[SerializeField] private Vector3 _cameraNewPosition;

    private void Start()
    {
        if (PlayerSingleton.Instance != null && PlayerSingleton.Instance.player != null)
        {
            GameObject player = PlayerSingleton.Instance.player;
            pState = player.GetComponent<PlayerStateList>();    
            pm = player.GetComponent<PlayerMovement>(); 
            RB = player.GetComponent<Rigidbody2D>();

            if (GameManager.Instance.transitionedFromScene == _transitionTo)
            {
                StartCoroutine(HandleSceneLoad());
            } else
            {
                StartCoroutine(UIManager.Instance.sceneFader.Fade(SceneFader.FadeDirection.Out));
            }
            

        }
        else
        {
            Debug.LogError("Игрок не найден!");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.transitionedFromScene = SceneManager.GetActiveScene().name;

            //pState.cutScene = true;
            //pState.invinsible = true;

            StartCoroutine(UIManager.Instance.sceneFader.FadeAndLoadScene(SceneFader.FadeDirection.In, _transitionTo));
        }
    }

    private IEnumerator HandleSceneLoad()
    {
        GameObject player = PlayerSingleton.Instance.player;
        player.transform.position = _startPoint.position;
        RB.velocity = Vector2.zero;
        
        /*if (Camera.main != null)
        {
            Camera.main.transform.position = _cameraNewPosition;
        }
        else
        {
            Debug.LogWarning("Main Camera не найдена!");
        }*/
        
        
        
        Debug.Log("Игрок телепортирован и остановлен в " + _startPoint.position);
        
        yield return StartCoroutine(UIManager.Instance.sceneFader.Fade(SceneFader.FadeDirection.Out));

        //pState.cutScene = false;
        //pState.invinsible = false;
    }
}
/*
public class SceneTransition : MonoBehaviour
{
    [SerializeField] private string _transitionTo;

    [SerializeField] private Transform _startPoint;
    [SerializeField] private Vector2 _exitDir;

    [SerializeField] private float _exitTime;

    //[SerializeField] private GameObject _playerObj;
    [SerializeField] private PlayerStateList pState;
    [SerializeField] private PlayerMovement pm;
    [SerializeField] private Rigidbody2D RB;
    public PlayerData Data;


    private void Start()
    {
        if (PlayerSingleton.Instance != null && PlayerSingleton.Instance.player != null)
        {
            GameObject player = PlayerSingleton.Instance.player;
            pm = player.GetComponent<PlayerMovement>(); // Заново получаем PlayerMovement
            pState = player.GetComponent<PlayerStateList>(); // Заново получаем PlayerStateList
            RB = player.GetComponent<Rigidbody2D>();

            Debug.Log("Игрок найден: " + player.name);

            if (GameManager.Instance.transitionedFromScene == _transitionTo)
            {
                player.transform.position = _startPoint.position;
                StartCoroutine(WalkIntoNewScene(_exitDir, _exitTime));
            }

            StartCoroutine(UIManager.Instance.sceneFader.Fade(SceneFader.FadeDirection.Out));
        }

        else
        {
            Debug.LogError("Игрок не найден!");
        }

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            GameManager.Instance.transitionedFromScene = SceneManager.GetActiveScene().name;

            pState.cutScene = true;
            pState.invinsible = true;

            StartCoroutine(UIManager.Instance.sceneFader.FadeAndLoadScene(SceneFader.FadeDirection.In, _transitionTo));
        }
    }

    public IEnumerator WalkIntoNewScene(Vector2 exitDir, float delay)
    {
        yield return new WaitForEndOfFrame();
        if (pState == null)
        {
            Debug.LogError("PlayerStateList не найден!");
            yield break;
        }

        if (RB == null)
        {
            Debug.LogError("RigidBody не найден!");
            yield break;
        }

        Debug.Log("⏳ [WalkIntoNewScene] Запуск корутины. invinsible = true");
        pState.invinsible = true;

        if (exitDir.y != 0)
        {
            RB.velocity = Data.jumpForce * exitDir;
        }


        if (exitDir.x != 0)
        {
            RB.velocity = new Vector2(exitDir.x > 0 ? 1 : -1, RB.velocity.y);
        }


        yield return new WaitForSeconds(delay);

        Debug.Log("✅ [WalkIntoNewScene] Завершение. invinsible = false, cutScene = false");

        pState.invinsible = false;
        pState.cutScene = false;
    }

}
*/