using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallSpawner : MonoBehaviour
{
    //
    public GameObject ballPrefab;
    public Vector3 centralPoint = new Vector3(0,50,0);
    public int numBalls = 500;

    // Start is called before the first frame update
    void Start()
    {
        SpawnBalls();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SpawnBalls()
    {
        for(int i = 0; i < numBalls; i++)
        {
            //Vector3 nextPosition = new Vector3(Random.Range(-200, 200), Random.Range(10, 300), Random.Range(-200, 200) );
            Vector3 nextPosition = new Vector3(Random.Range(-50, 50), Random.Range(10, 100), Random.Range(-50, 50));
            GameObject nextBall = Instantiate(ballPrefab, nextPosition, Quaternion.identity);
        }
    }
}
