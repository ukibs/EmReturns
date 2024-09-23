using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RapidFireBulletController : MonoBehaviour
{
    //
    private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        Destroy(gameObject, 15);
    }

    // Update is called once per frame
    void Update()
    {
        CheckRaycastCollissions();
    }

    private void CheckRaycastCollissions()
    {
        RaycastHit hitInfo;
        float dt = Time.deltaTime;
        float bulletTravelDistance = rb.velocity.sqrMagnitude * dt;
        if (Physics.Raycast(transform.position, transform.forward, out hitInfo, bulletTravelDistance))
        {
            Collider collider = hitInfo.collider;
            if (collider != null)
            {
                Collision fakeCollission = new Collision();
                //fakeCollission.
                //DestructibleObject destructibleObject = collider.GetComponent<DestructibleObject>();
                //destructibleObject.OnCollisionEnter(fakeCollission);

                Boss1EnergyBall boss1EnergyBall = collider.GetComponent<Boss1EnergyBall>();
                if(boss1EnergyBall != null)
                    boss1EnergyBall.OnCollisionEnter(fakeCollission);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Boss1SegmentController segmentController = collision.collider.GetComponentInParent<Boss1SegmentController>();
        if(segmentController != null)
        {
            segmentController.SufferDamage(2);
            Destroy(gameObject);
        }
    }
}
