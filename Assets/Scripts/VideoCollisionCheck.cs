using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VideoCollisionCheck : MonoBehaviour
{
    private VideoController videoController;

    private void Awake()
    {
        videoController = GetComponentInChildren<VideoController>();
    }

    private void OnMouseEnter()
    {
        videoController.HoveringVideo();
    }

    private void OnMouseExit()
    {
        videoController.StoppedHovering();
    }

    private void OnMouseDown()
    {
        videoController.PressedVideo();
    }
}
