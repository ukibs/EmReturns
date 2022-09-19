using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BossPhase", menuName = "ScriptableObjects/BossPhase", order = 1)]
public class BossPhaseSO : ScriptableObject
{
    public BossBehaviourSO[] behaviours;
}
