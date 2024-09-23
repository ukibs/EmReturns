using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 
/// </summary>
public class FakeRigidbody : MonoBehaviour
{
    //
    public float drag = 0.5f;
    public bool moves = true;
    public bool rotates = false;

    //
    [HideInInspector] public Vector3 currentVelocity;

    //
    private Rigidbody rb;
    
    private float currentRotationForce;
    private bool forceReceivedThisFrame = false;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {        
        //
        if (rotates)
        {
            //
            if(currentVelocity != Vector3.zero)
            {
                //
                //float amountToRotate = 90f * (1f / 50f) * Mathf.Deg2Rad;
                float amountToRotate = currentVelocity.magnitude * (1f / 50f) * Mathf.Deg2Rad * 50f;
                //Debug.Log("Rotating " + amountToRotate);
                Vector3 newDirection = transform.forward;
                 newDirection = Vector3.RotateTowards(transform.forward, currentVelocity, amountToRotate, 0f);
                //newDirection = Vector3.RotateTowards(transform.forward, currentVelocity, currentVelocity.magnitude/10f, 0f);
                transform.rotation = Quaternion.LookRotation(newDirection);
            }            
        }
        //
        if(moves)
        {
            transform.position += currentVelocity;            
        }
        currentVelocity *= (50f - drag) / 50f;
    }

    private void LateUpdate()
    {
        forceReceivedThisFrame = false;
    }

    public void AddForce(Vector3 force)
    {
        // Vamos a acotarlo a una vez por frame
        if (!forceReceivedThisFrame)
        {
            //Debug.Log("Recibing force on fake rb - force: " + force + " - mass: " + rb.mass + " - total: " + force / rb.mass);
            currentVelocity += force / rb.mass;
            forceReceivedThisFrame = true;
        }        
    }
}
