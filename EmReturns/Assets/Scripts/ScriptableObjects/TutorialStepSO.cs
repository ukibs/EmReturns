using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "TutorialStep", menuName = "ScriptableObjects/TutorialStep", order = 1)]

public class TutorialStepSO : ScriptableObject
{
    public string text;
    public int objectiveNumber;
}
