using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField]
    private Transform Player;

    [SerializeField]
    private float smoothSpeed = 0.05f;
    [SerializeField]
    private float videoSmoothSpeed = 0.05f;

    [SerializeField]
    private Vector3 Offset = new Vector3(0, 2.75f, -10);

    private Vector3 VideoPlayerLocation;

    private enum CameraState
    {
        Default,
        VideoPlayer
    };

    private CameraState cameraState;

    private void Awake()
    {
        SetCameraState(0);
    }

    private void FixedUpdate()
    {
        switch(cameraState)
        {
            case CameraState.Default:
                {
                    LerpCamera(Player.transform.position, smoothSpeed);
                    break;
                }
            case CameraState.VideoPlayer:
                {
                    LerpCamera(GetVideoPlayerLocation(), videoSmoothSpeed);
                    break;
                }
            default:
                {
                    LerpCamera(Player.transform.position, smoothSpeed);
                    break;
                }
        }
    }

    public void SetCameraState(int index)
    {
        switch (index)
        {
            case 0:
                {
                    cameraState = CameraState.Default;
                    break;
                }
            case 1:
                {
                    cameraState = CameraState.VideoPlayer;
                    break;
                }
            default:
                {
                    cameraState = CameraState.Default;
                    break;
                }
        }

    }

    public void SetVideoPlayerLocation(Vector3 _VideoPlayerLocation)
    {
        VideoPlayerLocation = _VideoPlayerLocation;
    }

    private Vector3 GetVideoPlayerLocation()
    {
        return VideoPlayerLocation;
    }

    private void LerpCamera(Vector3 newPos, float newSmooth)
    {
        Vector3 targetPos = newPos + Offset;
        Vector3 smoothedPos = Vector3.Lerp(transform.position, targetPos, newSmooth);
        transform.position = smoothedPos;
    }
}
