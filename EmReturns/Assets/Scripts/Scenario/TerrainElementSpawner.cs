using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainElementSpawner : MonoBehaviour
{
    //
    public GameObject terrainElementControllerPrefab;
    public ObjectGroupSO objectGroupSO;

    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("Spawning terrain objects");
        //SpawnObjects();
        SpawnObjectGroups();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void SpawnObjectGroups()
    {
        for(int i = 0; i < objectGroupSO.objectGroups.Length; i++)
        {
            SpawnObjects(objectGroupSO.objectGroups[i]);
        }
    }


    void SpawnObjects(ObjectGroup objectGroup)
    {
        //
        for (int i = 0; i < objectGroup.xObjects; i++)
        {
            for (int j = 0; j < objectGroup.yObjects; j++)
            {
                for (int k = 0; k < objectGroup.zObjects; k++)
                {
                    float valueToDecideSpawn = Random.value;
                    if (valueToDecideSpawn < objectGroup.objectDensity)
                    {
                        //
                        Vector3 nextPosition =
                        new Vector3(
                            (objectGroup.xDistance * i) - (objectGroup.xDistance * objectGroup.xObjects / 2),
                            (objectGroup.yDistance * j) - (objectGroup.yDistance * objectGroup.yObjects / 2),
                            (objectGroup.zDistance * k) - (objectGroup.zDistance * objectGroup.zObjects / 2)
                        );
                        //
                        //GameObject newObject = Instantiate(objectGroup.prefab, transform);
                        //newObject.transform.position = nextPosition;
                        //
                        GameObject newObject = Instantiate(terrainElementControllerPrefab, transform);
                        newObject.transform.position = nextPosition;
                        int prefabToSpawnIndex = Random.Range(0, objectGroup.prefabs.Length);
                        TerrainElementController terrainElementController = newObject.GetComponent<TerrainElementController>();
                        terrainElementController.terrainElementPrefab = objectGroup.prefabs[prefabToSpawnIndex];
                    }
                }
            }
        }
    }
}
