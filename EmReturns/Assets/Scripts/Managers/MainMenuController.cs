using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    public TMP_Text[] bestTimesTexts;

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
        //
        SetBestTimes();
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
        //
        if (Keyboard.current.pKey.wasPressedThisFrame)
        {
            PlayerPrefs.SetString("HighScores", "");
            Debug.Log("Scores deleted");
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

    void SetBestTimes()
    {
        // Ge the existant scores
        string scoresTextRaw = PlayerPrefs.GetString("HighScores", "");
        //Debug.Log(scoresTextRaw);
        string[] scores = scoresTextRaw.Split(';');
        //Debug.Log("Scores length: " + scores.Length);
        List<Score> scoreList = new List<Score>();
        // Check that it is not empty
        if (scores[0] != "")
        {
            for (int i = 0; i < scores.Length && i < bestTimesTexts.Length; i++)
            {
                //Debug.Log("Score " + i + ": " + scores[i]);
                string[] scoreDisected = scores[i].Split('-');
                //Debug.Log("Score dissected: " + scoreDisected);
                Score score = new Score(scoreDisected[0], float.Parse(scoreDisected[1]));

                int minutes = Mathf.FloorToInt(score.value / 60f);
                int seconds = Mathf.FloorToInt(score.value % 60f);
                int hundredths = Mathf.FloorToInt((score.value * 100f) % 100f);
                string timeFormatted = string.Format("{0:00}:{1:00}.{2:00}", minutes, seconds, hundredths);

                bestTimesTexts[i].text = (i + 1) + " - " + score.letters + " - " + timeFormatted;
            }
        }
    }
}
