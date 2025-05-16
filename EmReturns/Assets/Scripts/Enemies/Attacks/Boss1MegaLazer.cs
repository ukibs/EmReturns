using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss1MegaLazer : MonoBehaviour
{
    //
    public GameObject segmentPrefab;
    public float rateOfFire = 10;
    public LineRenderer lineRenderer;

    //
    private float currentFireTime = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        currentFireTime += Time.deltaTime;
        if(currentFireTime >= 1 / rateOfFire)
        {
            currentFireTime -= 1 / rateOfFire;
            GameObject nextSegment = Instantiate(segmentPrefab, transform.position, transform.rotation);
            //Rigidbody rb = nextSegment.GetComponent<Rigidbody>();
            //rb.velocity = transform.forward * 50;
        }
    }
}
