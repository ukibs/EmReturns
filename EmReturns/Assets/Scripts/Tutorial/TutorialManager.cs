using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public TutorialStepSO[] tutorialSteps;
    public GameObject tutorialStepsParent;
    public Transform[] tutorialStepsObjects;
    public AudioClip[] emSounds;
    public TMP_Text tutorialText;

    private static TutorialManager instance;
    private int currentStep = 0;
    private int currentObjetiveNumber = 0;

    public static TutorialManager Instance
    {
        get { return instance; }
    }

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        tutorialText.text = tutorialSteps[currentStep].text;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CheckAndNextPhase()
    {
        Debug.Log("Cheking objective reached");
        //tutorialSteps[currentStep].
        //PlayRandomEmSound();
        EM_VoiceController.Instance.PlayVoiceGroup("misc");

        currentObjetiveNumber++;
        if(currentObjetiveNumber >= tutorialSteps[currentStep].objectiveNumber)
        {
            NextPhase();
        }
    }

    void NextPhase()
    {
        tutorialStepsParent.transform.GetChild(currentStep).gameObject.SetActive(false);
        currentStep++;
        currentObjetiveNumber = 0;
        tutorialText.text = tutorialSteps[currentStep].text;
        tutorialStepsParent.transform.GetChild(currentStep).gameObject.SetActive(true);
    }

    void PlayRandomEmSound()
    {
        AudioManager.Instance.Play3dFx(transform.position, emSounds[Random.Range(0, emSounds.Length)]);
    }
}
