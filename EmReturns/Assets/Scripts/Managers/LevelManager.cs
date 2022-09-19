using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    //
    public GameObject endPanel;
    public GameObject instructionsPanel;
    public TMP_Text endMessage;
    public AudioClip victoryClip;
    public AudioClip defeatClip;

    //
    private static LevelManager instance;

    //
    public static LevelManager Instance { get { return instance; } }

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        //
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        //Gamepad gamepad = Gamepad.current;
        //if (gamepad == null)
        //    return; // No gamepad connected.
        //
        if (InputController.Instance.PausePressed)
        {
            if (endPanel.activeSelf)
            {
                SceneManager.LoadScene(0);
            }
            else
            {
                instructionsPanel.SetActive(!instructionsPanel.activeSelf);
            }
        }
        //
        if(InputController.Instance.ExitPressed && instructionsPanel.activeSelf)
        {
            SceneManager.LoadScene(0);
        }
    }

    //
    public void EndLevel(bool victory)
    {
        endPanel.SetActive(true);
        SetEndMusic(victory);
        string message = victory ? "VICTORY" : "DEFEAT";
        SetEndMessage(message);
    }

    //
    public void SetEndMessage(string message)
    {
        endMessage.text = message;
    }

    public void SetEndMusic(bool victory)
    {
        if (victory)    AudioManager.Instance.PlayMusic(victoryClip, 1f, false);
        else            AudioManager.Instance.PlayMusic(defeatClip, 1f, true);
    }
}
