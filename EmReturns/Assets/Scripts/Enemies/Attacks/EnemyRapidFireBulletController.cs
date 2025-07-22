using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyRapidFireBulletController : MonoBehaviour
{
    //
    public GameObject impactParticlesPrefab;

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
        //CheckRaycastCollissions();
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

                //Boss1EnergyBall boss1EnergyBall = collider.GetComponent<Boss1EnergyBall>();
                //if(boss1EnergyBall != null)
                //    boss1EnergyBall.OnCollisionEnter(fakeCollission);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("Collision with: " + collision.collider);
        //
        ContactPoint contactPoint = collision.GetContact(0);
        GameObject impactParticles = Instantiate(impactParticlesPrefab, contactPoint.point, Quaternion.LookRotation(contactPoint.normal));
        //Debug.Log(impactParticles);
        //Debug.Log("Collission with: " + collision.gameObject.name);
        //
        //Boss1SegmentController segmentController = collision.collider.GetComponentInParent<Boss1SegmentController>();
        //if(segmentController != null)
        //{
        //    int damageToApply = (int)(rb.velocity.magnitude * rb.mass);
        //    //Debug.Log("Rapid fire - Damage to apply: " + damageToApply);
        //    segmentController.SufferDamage(damageToApply);   
        //}
        //Boss1EnergyBall boss1EnergyBall = collision.collider.GetComponent<Boss1EnergyBall>();
        //if (boss1EnergyBall != null)
        //    boss1EnergyBall.OnCollisionEnter(collision);
        //
        EM_PlayerController em_PlayerController = collision.collider.gameObject.GetComponent<EM_PlayerController>();
        Debug.Log("EM Player controller: ", em_PlayerController);
        if(em_PlayerController)
        {
            em_PlayerController.ApplyDamage(50, false);
        }
        //
        Destroy(gameObject);
    }
}
