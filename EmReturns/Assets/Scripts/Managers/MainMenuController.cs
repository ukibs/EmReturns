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
    public GameObject bestTimesPanel;
    public TMP_Text[] bestTimesTexts;

    public CanvasGroup logoGroup;
    public CanvasGroup menuGroup;
    public float fadeDuration = 1f;

    //
    private int currentClipIndex = 0;
    private bool checkVideo = false;
    //private Gamepad gamepad;
    //private Keyboard keyboard;

    // Start is called before the first frame update
    void Start()
    {
        //sets values of both main screens (logo and menu)

        logoGroup.gameObject.SetActive(true);
        logoGroup.alpha = 1;
        logoGroup.interactable = true;
        logoGroup.blocksRaycasts = true;

        menuGroup.gameObject.SetActive(false);
        menuGroup.alpha = 0;
        menuGroup.interactable = false;
        menuGroup.blocksRaycasts = false;

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
        if (InputController.Instance.PausePressed || Mouse.current.leftButton.wasPressedThisFrame)
        {
            StartCoroutine(Transition());
            // SceneManager.LoadScene(1);
            //titleMenu.SetActive(false);
            //levelSelector.SetActive(true);
        }
        if (InputController.Instance.ExitPressed)
        {
            Application.Quit();
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
        if (Keyboard.current.mKey.wasPressedThisFrame)
        {
            PlayerPrefs.SetString("HighScores", "");
            Debug.Log("Scores deleted");
        }
        if (Keyboard.current.pKey.wasPressedThisFrame)
        {
            if (bestTimesPanel.activeSelf) bestTimesPanel.SetActive(false);
            else bestTimesPanel.SetActive(true);
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
        //SceneManager.LoadScene(sceneIndex);
        ImageFadeController.Instance.FadeAndGoToScene(sceneIndex);
    }

    public void OpenLevel(int levelIndex)
    {
        //GameManager.Instance.SetLevel(levelIndex);
        //SceneManager.LoadScene("MainScene");
        ImageFadeController.Instance.FadeAndGoToLevel(levelIndex);
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

    private IEnumerator Transition()
    {
        // Fade out logo
        yield return StartCoroutine(FadeCanvasGroup(logoGroup, 1, 0));
        logoGroup.gameObject.SetActive(false);

        // Activate menu before fade in
        menuGroup.gameObject.SetActive(true);
        yield return StartCoroutine(FadeCanvasGroup(menuGroup, 0, 1));

        // Activate interactability
        menuGroup.interactable = true;
        menuGroup.blocksRaycasts = true;
    }

    private IEnumerator FadeCanvasGroup(CanvasGroup group, float start, float end)
    {
        float elapsed = 0f;
        group.alpha = start;

        // Deactivate interactability while fade
        group.interactable = false;
        group.blocksRaycasts = false;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            group.alpha = Mathf.Lerp(start, end, elapsed / fadeDuration);
            yield return null;
        }

        group.alpha = end;
    }
}
