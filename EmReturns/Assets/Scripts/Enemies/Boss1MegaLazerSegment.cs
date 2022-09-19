using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss1MegaLazerSegment : MonoBehaviour
{
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

    void ApplyPower(Collider other)
    {
        //
        DestructibleObject destructibleObject = other.GetComponent<DestructibleObject>();
        if (destructibleObject)
        {
            destructibleObject.DestroyObject();
        }
        //
        EM_PlayerController playerController = other.GetComponent<EM_PlayerController>();
        if (playerController)
        {
            playerController.ApplyDamage(damage);
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
