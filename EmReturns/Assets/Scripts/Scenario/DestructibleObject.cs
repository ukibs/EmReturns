using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleObject : MonoBehaviour
{
    //
    public float resistance = 50;
    public GameObject destroyedVersion;
    public AudioClip destructionClip;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnCollisionEnter(Collision collision)
    {
        //Debug.Log(collision.collider);
        //Rigidbody rb = collision.collider.GetComponent<Rigidbody>();
        Rigidbody rb = collision.rigidbody;
        // Doble check por si es hijo de un objeto más grande
        if (!rb) rb = collision.collider.GetComponentInParent<Rigidbody>();
        //Debug.Log(rb.name);
        //
        if (rb)
        {
            if (rb.isKinematic || rb.velocity.magnitude * rb.mass >= resistance)
            {
                gameObject.SetActive(false);
                destroyedVersion.SetActive(true);
                //
                if (destructionClip)
                {
                    AudioManager.Instance.Play3dFx(collision.contacts[0].point, destructionClip, 1);
                }
            }
        }
        
    }

    public void ApplyForce(Vector3 force)
    {
        if (force.magnitude >= resistance)
        {
            gameObject.SetActive(false);
            destroyedVersion.SetActive(true);
        }
    }

    public void DestroyObject()
    {
        gameObject.SetActive(false);
        destroyedVersion.SetActive(true);
    }
}
