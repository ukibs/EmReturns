using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinisherLazerSphereController : MonoBehaviour
{
    //
    public GameObject impactParticlesPrefab;
    //
    public float movementSpeed = 50;
    public float force = 50;
    public int damage = 200;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * movementSpeed * Time.deltaTime);
    }

    private void OnTriggerStay(Collider other)
    {
        ApplyPower(other);
    }

    private void OnCollisionEnter(Collision collision)
    {
        ContactPoint contactPoint = collision.GetContact(0);
        GameObject impactParticles = Instantiate(impactParticlesPrefab, transform.position, Quaternion.LookRotation(contactPoint.normal));
        ApplyPower(collision.collider);
        Destroy(gameObject);

    }

    void ApplyPower(Collider other)
    {
        //
        DestructibleObject destructibleObject = other.GetComponent<DestructibleObject>();
        if (destructibleObject)
        {
            destructibleObject.DestroyObject();
        }
        //
        Boss1MegaLazerSegment boss1MegaLazerSegment = other.GetComponent<Boss1MegaLazerSegment>();
        if(boss1MegaLazerSegment != null)
        {
            Destroy(boss1MegaLazerSegment.gameObject);
        }
        //
        Boss1Controller boss1Controller = other.GetComponentInParent<Boss1Controller>();
        if (boss1Controller)
        {
            boss1Controller.SufferDamage(damage);
            boss1Controller.GetComponent<FakeRigidbody>().AddForce(transform.forward * 10);
            EM_PlayerController.Instance.SpendFinisherEnergy();
        }
        //
        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb)
        {
            //Vector3 direction = other.transform.position - transform.position;
            //rb.AddExplosionForce(force, transform.position, transform.localScale.x);
            rb.AddForce(transform.forward * force, ForceMode.Force);
        }
    }
}
