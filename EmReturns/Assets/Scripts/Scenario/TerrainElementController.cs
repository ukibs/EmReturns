using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainElementController : MonoBehaviour
{
    public float playerDistanceToSpawn = 500;
    public float playerDistanceToDeactivate = 1000;

    [HideInInspector] public GameObject terrainElementPrefab;
    [HideInInspector] public GameObject terrainElement;

    private float playerCheckRate = 1;

    // Start is called before the first frame update
    void Start()
    {
        playerCheckRate = 1 + Random.value;
        CheckPlayerDistance();
        StartCoroutine(PlayerCheckRoutine());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator PlayerCheckRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(playerCheckRate);
            CheckPlayerDistance();
        }        
    }

    void CheckPlayerDistance()
    {
        if (
            (transform.position - EM_PlayerController.Instance.transform.position).sqrMagnitude < Mathf.Pow(playerDistanceToSpawn, 2)
            && terrainElement == null
        )
        {
            terrainElement = Instantiate(terrainElementPrefab, transform);
        }
        else if (
            (transform.position - EM_PlayerController.Instance.transform.position).sqrMagnitude < Mathf.Pow(playerDistanceToSpawn, 2)
            && terrainElement != null
            && terrainElement.activeSelf == false
        )
        {
            Debug.Log("Activating terrain element");
            terrainElement.SetActive(true);
        }
        else if (
            (transform.position - EM_PlayerController.Instance.transform.position).sqrMagnitude > Mathf.Pow(playerDistanceToSpawn, 2)
            && terrainElement != null
            && terrainElement.activeSelf == true
        )
        {
            Debug.Log("Deactivating terrain element");
            terrainElement.SetActive(false);
        }
    }
}
