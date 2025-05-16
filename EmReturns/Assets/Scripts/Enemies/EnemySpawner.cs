using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public float timeToSpawn;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(WaitAndSpawnEnemy());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator WaitAndSpawnEnemy()
    {
        while (true)
        {
            yield return new WaitForSeconds(timeToSpawn);
            Instantiate(enemyPrefab, transform.position, Quaternion.identity);
        }
    }
}
