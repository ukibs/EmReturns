using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestBallController : MonoBehaviour
{
    //
    public float atractionForce = 2;
    //
    private BallSpawner ballSpawner;
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        ballSpawner = FindObjectOfType<BallSpawner>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 offset = ballSpawner.centralPoint - transform.position;
        rb.AddForce(offset.normalized * atractionForce);
    }
}
