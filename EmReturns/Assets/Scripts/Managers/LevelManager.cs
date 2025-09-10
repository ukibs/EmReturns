using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public GameObject terrainGenerator;
    public LevelDataSO levelDataSO;
    public EnemySpawner enemySpawner;
    public ScorePanelController scorePanelController;

    //
    public GameObject enemyWavesPanel;
    public TMP_Text waveText;
    public TMP_Text remainigEnemiesText;
    public GameObject timerGroup;
    public TMP_Text timerText;
    public GameObject scorePanel;
    public TMP_Text scorePanelTimerText;

    //
    private static LevelManager instance;
    private int currentEnemyWave = 0;
    private int enemiesDefeated = 0;
    private float fightStartTime = 0;
    private float fightEndTime = 0;

    //
    public static LevelManager Instance { get { return instance; } }

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        //
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        // If we have the GameManager load the correspondat level
        if(GameManager.Instance != null)
        {
            //Debug.Log("Game manager present");
            levelDataSO = GameManager.Instance.levels[GameManager.Instance.currentLevelIndex];
        }
        //
        if (levelDataSO.enemyToSpawn)
        {
            Instantiate(levelDataSO.enemyToSpawn, new Vector3(0, 500, 1000), Quaternion.identity);
        }
        //
        if(levelDataSO.musicClip)
            AudioManager.Instance.PlayMusic(levelDataSO.musicClip);
        //
        if (levelDataSO.enemyWaves.Length > 0) {
            enemySpawner.SpawnEnemyWave(levelDataSO.enemyWaves[currentEnemyWave]);
            enemyWavesPanel.SetActive(true);
            waveText.text = "Wave " + 1;
            remainigEnemiesText.text = "Enemies 0/" + levelDataSO.enemyWaves[currentEnemyWave].amount;
        }
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
            if (endPanel.activeSelf || scorePanel.activeSelf)
            {
                // Save score
                SaveScore();

                // Go out
                //SceneManager.LoadScene(1);
                ImageFadeController.Instance.FadeAndGoToScene(1);
            }
            else
            {
                instructionsPanel.SetActive(!instructionsPanel.activeSelf);
            }
        }
        //
        if(InputController.Instance.ExitPressed && instructionsPanel.activeSelf)
        {
            //SceneManager.LoadScene(1);
            ImageFadeController.Instance.FadeAndGoToScene(1);
        }
        //
        if (timerText.gameObject.activeSelf && fightEndTime == 0)
        {
            float currentFightTime = Time.time - fightStartTime;
            int minutes = Mathf.FloorToInt(currentFightTime / 60f);
            int seconds = Mathf.FloorToInt(currentFightTime % 60f);
            int hundredths = Mathf.FloorToInt((currentFightTime * 100f) % 100f);
            timerText.text = string.Format("{0:00}:{1:00}.{2:00}", minutes, seconds, hundredths);
        }
    }

    //
    public void EndLevel(bool victory)
    {
        if(fightEndTime == 0)
        {
            endPanel.SetActive(true);
        }        
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

    public void EnemyDefeated()
    {
        enemiesDefeated++;
        remainigEnemiesText.text = "Enemies " + enemiesDefeated + "/" + levelDataSO.enemyWaves[currentEnemyWave].amount;
        if(enemiesDefeated >= levelDataSO.enemyWaves[currentEnemyWave].amount)
        {
            Debug.Log("Checking wave " + currentEnemyWave + " - " + levelDataSO.enemyWaves.Length);
            if(currentEnemyWave + 1 < levelDataSO.enemyWaves.Length)
            {
                currentEnemyWave++;
                enemiesDefeated = 0;
                enemySpawner.SpawnEnemyWave(levelDataSO.enemyWaves[currentEnemyWave]);
                waveText.text = "Wave " + (currentEnemyWave + 1);
                remainigEnemiesText.text = "Enemies 0/" + levelDataSO.enemyWaves[currentEnemyWave].amount;
            }
            else
            {
                EndLevel(true);
            }
        }
    }

    public void StartTimedFight()
    {
        fightStartTime = Time.time;
        timerGroup.SetActive(true);
    }

    public void EndTimedFight()
    {
        fightEndTime = Time.time;

        float totalFightTime = fightEndTime - fightStartTime;
        int minutes = Mathf.FloorToInt(totalFightTime / 60f);
        int seconds = Mathf.FloorToInt(totalFightTime % 60f);
        int hundredths = Mathf.FloorToInt((totalFightTime * 100f) % 100f);
        timerText.text = string.Format("{0:00}:{1:00}.{2:00}", minutes, seconds, hundredths);

        scorePanel.SetActive(true);
        scorePanelTimerText.text = string.Format("{0:00}:{1:00}.{2:00}", minutes, seconds, hundredths);
    }

    void SaveScore()
    {
        // Ge the existant scores
        string scoresTextRaw = PlayerPrefs.GetString("HighScores", "");
        string[] scores = scoresTextRaw.Split(';');
        //Debug.Log("Scores length: " + scores.Length);
        List<Score> scoreList = new List<Score>();
        // Check that it is not empty
        if (scores[0] != "")
        {
            for (int i = 0; i < scores.Length; i++)
            {
                //Debug.Log("Score " + i + ": " + scores[i]);
                string[] scoreDisected = scores[i].Split('-');
                //Debug.Log("Score dissected: " + scoreDisected);
                Score score = new Score(scoreDisected[0], float.Parse(scoreDisected[1]));
                scoreList.Add(score);
            }
        }        
        // Add the new one
        float totalFightTime = fightEndTime - fightStartTime;
        Score newScore = new Score(scorePanelController.Initials, totalFightTime);
        scoreList.Add(newScore);
        // Order them
        scoreList = scoreList.OrderBy(s => s.value).ToList();
        // Save them
        string scoreListString = string.Join(";", scoreList.Select(s => s.GetString()));
        PlayerPrefs.SetString("HighScores", scoreListString);
    }
}

public class Score
{
    public string letters;
    public float value;

    public Score(string letters, float value)
    {
        this.letters = letters;
        this.value = value;
    }

    public string GetString()
    {
        return letters + "-" + value;
    }
}
