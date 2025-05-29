using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public TutorialStepSO[] tutorialSteps;
    public AudioClip[] emSounds;

    private static TutorialManager instance;
    private int currentStep = 0;


    public static TutorialManager Instance
    {
        get { return instance; }
    }

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CheckAndNextPhase()
    {
        Debug.Log("Cheking objective reached");
        //tutorialSteps[currentStep].
        PlayRandomEmSound();
    }

    void PlayRandomEmSound()
    {
        AudioManager.Instance.Play3dFx(transform.position, emSounds[Random.Range(0, emSounds.Length)]);
    }
}
