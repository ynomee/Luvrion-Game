using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowObject : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _playerTransform;

    [Header("Flip Rotation Stats")]
    [SerializeField] private float _flipYRotationTime = 0.5f;

    private Coroutine _turnCoroutine;

    private PlayerMovement _player;

    private bool _isFacingRight;

    private void Awake()
    {
        _player = _playerTransform.GetComponent<PlayerMovement>();

        _isFacingRight = _player.IsFacingRight;
    }

    private void Update()
    {
        //make the cameraFollowObj follow the player's pos
        transform.position = _playerTransform.position;
    }

    public void CallTurn()
    {
        _turnCoroutine = StartCoroutine(FlipYLerp());
    }

    private IEnumerator FlipYLerp()
    {
        float startRotation = transform.localEulerAngles.y;
        float endRotationAmount = DetermineEndRotation();
        float yRotation = 0f;

        //the passed time
        float elapsedTime = 0f;
        while (elapsedTime < _flipYRotationTime) 
        {
            elapsedTime += Time.deltaTime;

            //lerp the y rotation
            yRotation = Mathf.Lerp(startRotation, endRotationAmount, (elapsedTime/_flipYRotationTime));
            transform.rotation = Quaternion.Euler(0, yRotation, 0);

            yield return null;
        }
    }

    private float DetermineEndRotation()
    {
        _isFacingRight = !_isFacingRight;
        if (_isFacingRight)
        {
            return 180f;
        }
        else
        {
            return 0f;
        }
    }
}
