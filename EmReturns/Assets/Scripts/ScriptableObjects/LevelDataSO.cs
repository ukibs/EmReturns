using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelData", menuName = "ScriptableObjects/LevelData", order = 1)]
public class LevelDataSO : ScriptableObject
{
    public ObjectGroupSO objectGroup;
    public bool useTerrain;
    public GameObject enemyToSpawn;
    public Material skyboxMaterial;
    public AudioClip musicClip;
    public EnemyWave[] enemyWaves;
}

[Serializable]
public class EnemyWave
{
    public GameObject enemyPrefab;
    public int amount;
}