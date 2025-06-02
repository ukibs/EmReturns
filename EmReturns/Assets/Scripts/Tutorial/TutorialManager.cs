using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    //public TutorialStepSO[] tutorialSteps;
    public GameObject tutorialStepsParent;
    //public Transform[] tutorialStepsObjects;
    public AudioClip[] emSounds;
    public TMP_Text tutorialText;

    private static TutorialManager instance;
    private TutorialStep[] tutorialSteps;
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
        tutorialSteps = GetComponentsInChildren<TutorialStep>();
        tutorialText.text = tutorialSteps[currentStep].text;

        for(int i = 1; i < tutorialSteps.Length; i++)
        {
            tutorialSteps[i].gameObject.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CheckAndNextPhase()
    {
        Debug.Log("Cheking objective reached");
        EM_VoiceController.Instance.PlayVoiceGroup("misc");

        currentObjetiveNumber++;
        if(currentObjetiveNumber >= tutorialSteps[currentStep].objectiveNumber)
        {
            //NextPhase();
            MaterialDissolver[] materialDissolvers = tutorialSteps[currentStep].GetComponentsInChildren<MaterialDissolver>();
            for(int j = 0; j < materialDissolvers.Length; j++)
            {
                materialDissolvers[j].StartDissolution(-1);
            }
            StartCoroutine(WaitAndNextPhase());
        }
    }

    void NextPhase()
    {
        //
        Debug.Log("Current step: " + currentStep + ", Tutorial steps length: " + tutorialSteps.Length);
        if (currentStep >= tutorialSteps.Length - 1)
            return;
        //
        tutorialStepsParent.transform.GetChild(currentStep).gameObject.SetActive(false);
        currentStep++;
        currentObjetiveNumber = 0;
        tutorialText.text = tutorialSteps[currentStep].text;
        tutorialStepsParent.transform.GetChild(currentStep).gameObject.SetActive(true);
    }

    IEnumerator WaitAndNextPhase()
    {
        yield return new WaitForSeconds(2);
        NextPhase();
    }
}
