using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class VideoLogoController : MonoBehaviour
{
    private VideoPlayer videoPlayer;

    // Start is called before the first frame update
    void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        videoPlayer.loopPointReached += GoToScene;
    }

    // Update is called once per frame
    void Update()
    {
        //if (videoPlayer.loopPointReached && !videoPlayer.isPlaying) 
        //{
            
        //}
    }

    void GoToScene(VideoPlayer vp)
    {
        SceneManager.LoadScene("MainMenu");
    }
}
