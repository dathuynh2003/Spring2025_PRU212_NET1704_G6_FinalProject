using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class VideoEnd : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public VideoPlayer videoPlayer;
    public string mainMenuScene = "MenuScene";

    void Start()
    {
        videoPlayer.loopPointReached += OnVideoEnd;
    }

    void OnVideoEnd(VideoPlayer vp)
    {
        SceneManager.LoadScene(mainMenuScene);
    }
}
