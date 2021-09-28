using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoController : MonoBehaviour
{
    private VideoPlayer videoPlayer;
    private Material material;
    private Color originalColour;
    private Color DarkColour = new Color(0.8f, 0.8f, 0.8f);

    [SerializeField]
    private GameObject EntryPlayObject;
    [SerializeField]
    private GameObject PlayObject;
    [SerializeField]
    private GameObject PauseObject;
    [SerializeField]
    private Transform progressBar;
    [SerializeField]
    private GameObject progressBarBG;

    private bool CanPlay = false;
    private bool CanPause = false;

    private float percentageFinished = 0f;

    // First value is X Position Value, Second Value is X Scale Value
    private Vector2 StartingProgressBar = new Vector2(-0.5f, 0f);
    private Vector2 FinishedProgressBar = new Vector2(0f, 1f);

    private float defaultVolume = 0.1f;

    private void Awake()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        material = GetComponent<MeshRenderer>().material;
        originalColour = material.color;

        videoPlayer.time = 0;
        videoPlayer.Play();
        StartCoroutine(GetThumbnail());
    }

    private void Update()
    {
        if (videoPlayer.isPlaying)
        {
            percentageFinished = (float)videoPlayer.time / (float)videoPlayer.length;
            float value1 = Mathf.Lerp(StartingProgressBar.x, FinishedProgressBar.x, percentageFinished);
            float value2 = Mathf.Lerp(StartingProgressBar.y, FinishedProgressBar.y, percentageFinished);

            progressBar.localPosition = new Vector3(value1, progressBar.localPosition.y, progressBar.localPosition.z);
            progressBar.localScale = new Vector3(value2, progressBar.localScale.y, progressBar.localScale.z);
        } 
    }

    public void HoveringVideo()
    {
        bool CheckPlaying = videoPlayer.isPlaying;

        CanPlay = !CheckPlaying ? true : false;
        CanPause = CheckPlaying ? true : false;
        material.color = originalColour * DarkColour;

        SetMediaButtons(CheckPlaying, !CheckPlaying);
        progressBar.gameObject.SetActive(true);
        progressBarBG.SetActive(true);
    }

    public void StoppedHovering()
    {
        CanPlay = false;
        CanPause = false;
        material.color = originalColour;
        SetMediaButtons(false, false);
        progressBar.gameObject.SetActive(false);
        progressBarBG.SetActive(false);
    }

    public void PressedVideo()
    {
        if (CanPlay)
        {
            EntryPlayObject.SetActive(false);
            videoPlayer.Play();
            CanPlay = false;
            CanPause = true;
            videoPlayer.SetDirectAudioMute(0, false);
            SetMediaButtons(true, false);
            progressBar.gameObject.SetActive(true);
        }
        else if (CanPause)
        {
            videoPlayer.Pause();
            CanPlay = true;
            CanPause = false;
            SetMediaButtons(false, true);
            progressBar.gameObject.SetActive(true);
        }
    }

    void SetMediaButtons(bool btnCheck1, bool btnCheck2)
    {
        PauseObject.SetActive(btnCheck1);
        PlayObject.SetActive(btnCheck2);
    }

    private IEnumerator GetThumbnail()
    {
        yield return new WaitForSeconds(0.5f);

        material.mainTexture = videoPlayer.texture;
        videoPlayer.Pause();
        videoPlayer.time = 0;
        progressBar.localPosition = new Vector3(StartingProgressBar.x, progressBar.localPosition.y, progressBar.localPosition.z);
        progressBar.localScale = new Vector3(StartingProgressBar.y, progressBar.localScale.y, progressBar.localScale.z);
    }
}
