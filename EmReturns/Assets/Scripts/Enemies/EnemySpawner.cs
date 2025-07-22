using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public AudioClip spawnClip;
    //public GameObject enemyPrefab;
    public float timeToSpawn;
    //public float spawnHeight;

    // Start is called before the first frame update
    void Start()
    {
        //StartCoroutine(WaitAndSpawnEnemy());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnEnemyWave(EnemyWave enemyWave)
    {
        for (int i = 0; i < enemyWave.amount; i++) {
            Vector3 offset = new Vector3(Random.Range(-50,50), Random.Range(-50, 50), Random.Range(-50, 50));
            Instantiate(enemyWave.enemyPrefab, transform.position + offset, Quaternion.identity);
        }
        AudioManager.Instance.Play2dFx(transform.position, spawnClip, 1);
    }

    //IEnumerator WaitAndSpawnEnemy()
    //{
    //    while (true)
    //    {
    //        yield return new WaitForSeconds(timeToSpawn);
    //        Instantiate(enemyPrefab, transform.position, Quaternion.identity);
    //    }
    //}
}
