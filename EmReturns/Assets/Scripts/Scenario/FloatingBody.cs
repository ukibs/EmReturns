using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingBody : MonoBehaviour
{
    //
    public float atractionForce = 2;

    //
    private Rigidbody rb;
    private bool active = false;
    private Vector3 desiredPosition = Vector3.zero;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (active)
        {
            Vector3 offset = desiredPosition - transform.position;
            rb.AddForce(offset.normalized * atractionForce);
        }
    }

    public void Activate()
    {
        active = true;
        desiredPosition = new Vector3(transform.position.x, Random.Range(100,300), transform.position.z);
        rb.useGravity = false;
    }
}
