using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamager : MonoBehaviour
{
    //
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Destroy(this, 10);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        Boss1SegmentController boss1SegmentController = collision.collider.GetComponentInParent<Boss1SegmentController>();
        if (boss1SegmentController && rb)
        {
            Debug.Log("Applying damage to boss");
            boss1SegmentController.SufferDamage((int)rb.velocity.magnitude);
        }
        //
        Boss1Controller boss1Controller = collision.collider.GetComponentInParent<Boss1Controller>();
        if (boss1Controller)
        {
            boss1Controller.SufferDamage(1);
        }
        // Cheuqeo de fake rigidbody
        FakeRigidbody fakeRigidbody = collision.collider.GetComponentInParent<FakeRigidbody>();
        if (fakeRigidbody && rb)
        {
            //Debug.Log("Enemy damager - Aplying force: " + rb.velocity);
            //Debug.Log("Has fake RB");
            //Debug.Log(collision.collider.name);
            fakeRigidbody.AddForce(rb.velocity * 2);
        }
        //
        //Debug.Log("Collision with: " + collision.collider + " - " + boss1Controller + " - " + boss1Controller + " - " + fakeRigidbody);
    }
}
