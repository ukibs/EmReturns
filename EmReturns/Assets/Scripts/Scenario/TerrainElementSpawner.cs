using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainElementSpawner : MonoBehaviour
{
    //
    public GameObject prefab;
    public int xObjects = 10;
    public int zObjects = 10;
    public float xDistance = 100;
    public float zDistance = 100;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Spawning terrain objects");
        SpawnObjects();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SpawnObjects()
    {
        //
        for(int i = 0; i < xObjects; i++)
        {
            for(int j = 0; j < zObjects; j++)
            {
                //
                Vector3 nextPosition = new Vector3((xDistance * i) - (xDistance * xObjects / 2), 
                                            0, (zDistance * j) - (zDistance * zObjects / 2));
                //
                GameObject newObject = Instantiate(prefab, transform);
                newObject.transform.position = nextPosition;
            }
        }
    }
}
