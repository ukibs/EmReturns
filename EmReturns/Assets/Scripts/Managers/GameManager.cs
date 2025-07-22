using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public LevelDataSO[] levels;

    [HideInInspector] public int currentLevelIndex = 0;

    private static GameManager instance;

    public static GameManager Instance
    {
        get { return instance; }
    }

    // Start is called before the first frame update
    void Start()
    {
        if(instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetLevel(int index)
    {
        currentLevelIndex = index;
    }
}
