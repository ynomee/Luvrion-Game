using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraManager : MonoBehaviour
{
    public static CameraManager instance;

    [SerializeField] private CinemachineVirtualCamera[] _allVirtualsCameras;

    [Header("Controls for lerping the Y Damping during player jump/fall")]
    [SerializeField] private float _fallPanAmount = 0.25f;
    [SerializeField] private float _fallYPanTime = 0.35f;
    public float fallSpeedYDampingChangeThreshold = -15f;

    public bool IsLerpingYDamping { get; private set; }
    public bool LerpedFromPlayerFalling { get; set; }

    private Coroutine _lerpYPanCoroutine;
    private Coroutine _panCameraCoroutine;

    private CinemachineVirtualCamera _currentCamera;
    private CinemachineFramingTransposer _framingTransposer;

    private float _normYPanAmount;

    private Vector2 _startingTrackedObjectOffset;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        for (int i = 0; i < _allVirtualsCameras.Length; i++)
        {
            if (_allVirtualsCameras[i].enabled)
            {
                //set the current active camera
                _currentCamera = _allVirtualsCameras[i];

                //set the framing transposer
                _framingTransposer = _currentCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
            }
        }

        //Set the YDamping amount so it is based on the inspector value
        _normYPanAmount = _framingTransposer.m_YDamping;

        //Set the starting position of the tracked object offset
        _startingTrackedObjectOffset = _framingTransposer.m_TrackedObjectOffset;
    }
    #region LERP THE Y DAMPING
    public void LerpYDamping(bool isPlayerFalling)
    {
        _lerpYPanCoroutine = StartCoroutine(LerpYAction(isPlayerFalling));
    }

    public IEnumerator LerpYAction(bool isPlayerFalling)
    {
        IsLerpingYDamping = true;

        //grab the sturting dumping amount
        float startDampAmount = _framingTransposer.m_YDamping;
        float endDampAmount = 0f;

        //determine the end damping amount
        if (isPlayerFalling)
        {
            endDampAmount = _fallPanAmount;
            LerpedFromPlayerFalling = true;
        }
        else
        {
            endDampAmount = _normYPanAmount;
        }

        //lerp the pan amount
        float elapsedTime = 0f;
        while(elapsedTime < _fallYPanTime)
        {
            elapsedTime += Time.deltaTime;

            float lerpedTimeAmount = Mathf.Lerp(startDampAmount, endDampAmount, (elapsedTime/_fallYPanTime));
            _framingTransposer.m_YDamping = lerpedTimeAmount;

            yield return null;
        }

        IsLerpingYDamping = false;
    }
    #endregion

    #region PAN CAMERA

    public void PanCameraOnContact(float panDistance, float panTime, PanDirection panDirection, bool panToStarting)
    {
        _panCameraCoroutine = StartCoroutine(PanCamera(panDistance, panTime, panDirection, panToStarting));
    }

    private IEnumerator PanCamera(float panDistance, float panTime, PanDirection panDirection, bool panToStartingPos)
    {
        Vector2 endPos = Vector2.zero;
        Vector2 startingPos = Vector2.zero;

        //handle pan from trigger
        if (!panToStartingPos)
        {
            //set the direction and distance
            switch(panDirection)
            {
                case PanDirection.Up:
                    endPos = Vector2.up;
                    break;
                case PanDirection.Down:
                    endPos = Vector2.down;
                    break;
                case PanDirection.Left:
                    endPos = Vector2.left;
                    break;
                case PanDirection.Right:
                    endPos = Vector2.right;
                    break;
            }

            endPos *= panDistance;

            startingPos = _startingTrackedObjectOffset;

            endPos += startingPos;
        }

        //handle the direction settings when moving back to the starting position
        else
        {
            startingPos = _framingTransposer.m_TrackedObjectOffset;
            endPos = _startingTrackedObjectOffset;
        }

        //handle the actual panning of the camera
        float elapsedTime = 0f;
        while (elapsedTime < panTime)
        {
            elapsedTime += Time.deltaTime;

            Vector3 panLerp = Vector3.Lerp(startingPos, endPos, (elapsedTime/panTime));
            _framingTransposer.m_TrackedObjectOffset = panLerp;

            yield return null;
        }
    }

    #endregion

    #region SWAP CAMERAS

    public void SwapCameras(CinemachineVirtualCamera cameraFromLeft, CinemachineVirtualCamera cameraFromRight, Vector2 triggerExitDirection)
    {
        //if the current camera om the left and our trigger exit direction was on the right
        if (_currentCamera == cameraFromLeft && triggerExitDirection.x > 0)
        {
            //activate the new camera
            cameraFromRight.enabled = true;

            //deactivate the old camera
            cameraFromLeft.enabled = false;

            //set the new camera as the current camera
            _currentCamera = cameraFromRight;

            //update out composer visible
            _framingTransposer = _currentCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        }

        //if the current camera om the right and our trigger exit direction was on the left
        else if (_currentCamera == cameraFromRight && triggerExitDirection.x < 0)
        {
            //activate the new camera
            cameraFromLeft.enabled = true;

            //deactivate the old camera
            cameraFromRight.enabled = false;

            //set the new camera as the current camera
            _currentCamera = cameraFromLeft;

            //update out composer visible
            _framingTransposer = _currentCamera.GetCinemachineComponent<CinemachineFramingTransposer>();
        }
    }
    #endregion
}
