using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using Cinemachine;

public class CameraFollowObject : MonoBehaviour
{
    public static CameraFollowObject Instance { get; private set; }

    [Header("References")]
    [SerializeField] private Transform _playerTransform;

    [Header("Flip Rotation Stats")]
    [SerializeField] private float _flipYRotationTime = 0.5f;

    private Coroutine _turnCoroutine;
    private PlayerMovement _player;
    private bool _isFacingRight;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            //DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        Debug.Log("Awake: Initial _isFacingRight = " + _isFacingRight);
    }

    private void Start()
    {
        FindPlayer();
        SceneManager.sceneLoaded += OnSceneLoaded;
        Debug.Log("CameraFollowObject запущен в сцене: " + gameObject.name);
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void LateUpdate()
    {
        if (_playerTransform == null)
        {
            Debug.LogWarning("Игрок не найден, пробуем найти снова...");
            return;
        }

        transform.position = _playerTransform.position;
    }

    private void FindPlayer()
    {
        if (PlayerSingleton.Instance != null)
        {
            _playerTransform = PlayerSingleton.Instance.transform;
            _player = _playerTransform.GetComponent<PlayerMovement>();
            _isFacingRight = _player.IsFacingRight;
        }
    }

    public void CallTurn()
    {
        _turnCoroutine = StartCoroutine(FlipYLerp());
    }

    private IEnumerator FlipYLerp()
    {
        float startRotation = transform.localEulerAngles.y;
        float endRotationAmount = DetermineEndRotation();
        float elapsedTime = 0f;

        while (elapsedTime < _flipYRotationTime)
        {
            elapsedTime += Time.deltaTime;
            float yRotation = Mathf.Lerp(startRotation, endRotationAmount, elapsedTime / _flipYRotationTime);
            transform.rotation = Quaternion.Euler(0, yRotation, 0);
            yield return null;
        }
    }

    private float DetermineEndRotation()
    {
        return _player.IsFacingRight ? 0f : 180f;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Сцена загружена: " + scene.name);
        AssignCinemachineTarget();
    }

    private void AssignCinemachineTarget()
    {
        CinemachineVirtualCamera cinemachineCam = FindObjectOfType<CinemachineVirtualCamera>();

        if (cinemachineCam.Follow != null)
        {
            cinemachineCam.Follow = transform;
            Debug.Log("Cinemachine Virtual Camera обновила Follow -> " + transform.name);
        }
        else
        {
            Debug.LogWarning("Cinemachine Virtual Camera не найдена!");
        }
    }
}

