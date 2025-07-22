using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Video;

public class MainMenuController : MonoBehaviour
{
    //
    [Header("Videos")]
    public VideoPlayer videoPlayer;
    public VideoClip[] clips;
    public bool randomOrder;
    [Header("Components")]
    public GameObject titleMenu;
    public GameObject levelSelector;

    //
    private int currentClipIndex = 0;
    private bool checkVideo = false;
    private Gamepad gamepad;
    private Keyboard keyboard;

    // Start is called before the first frame update
    void Start()
    {
        //
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        //
        if (clips.Length > 0)
            StartCoroutine(WaitAndCheck());
    }

    // Update is called once per frame
    void Update()
    {
        //
        gamepad = Gamepad.current;
        keyboard = Keyboard.current;
        //
        if(gamepad != null)
        {
            //
            if (gamepad.startButton.wasPressedThisFrame)
            {
                // SceneManager.LoadScene(1);
                titleMenu.SetActive(false);
                levelSelector.SetActive(true);
            }
            if (gamepad.selectButton.wasPressedThisFrame)
            {
                Application.Quit();
            }
        }
        //
        if(keyboard != null)
        {
            //
            if (keyboard.enterKey.wasPressedThisFrame || Mouse.current.leftButton.IsPressed())
            {
                // SceneManager.LoadScene(1);
                titleMenu.SetActive(false);
                levelSelector.SetActive(true);
            }
            if (keyboard.escapeKey.wasPressedThisFrame)
            {
                Application.Quit();
            }
        }        
        //
        if (checkVideo && !videoPlayer.isPlaying)
        {
            //
            if (randomOrder)
            {
                currentClipIndex = Random.Range(0, clips.Length);
            }
            else
            {
                currentClipIndex++;
                if (currentClipIndex > clips.Length - 1)
                {
                    currentClipIndex = 0;
                }
            }            
            videoPlayer.clip = clips[currentClipIndex];
            videoPlayer.Play();
            //
            StartCoroutine(WaitAndCheck());
            //
            Debug.Log("Setting video - " + currentClipIndex);
        }
    }

    IEnumerator WaitAndCheck()
    {
        checkVideo = false;
        yield return new WaitForSeconds(1);
        checkVideo = true;
    }

    public void OpenScene(int sceneIndex)
    {
        SceneManager.LoadScene(sceneIndex);
    }

    public void OpenLevel(int levelIndex)
    {
        GameManager.Instance.SetLevel(levelIndex);
        SceneManager.LoadScene("MainScene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
