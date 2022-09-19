using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BossBehaviour", menuName = "ScriptableObjects/BossBehaviour", order = 1)]
public class BossBehaviourSO : ScriptableObject
{
    public BossBehaviourType type;
    public BossAttackType attackType;
    public float movementSpeed = 10;
    public float rotationSpeed = 10;
}

public enum BossBehaviourType
{
    GoingToPlayer,
    GoingUp,
    GoingDown,
    EncirclingPlayer,

    Count
}

//
public enum BossAttackType
{
    EnergyBalls,
    MegaLazer,
    Lunge,

    Count
}