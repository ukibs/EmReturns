using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyExplosionOnDeath : MonoBehaviour
{
    //
    public AudioClip onImpactSound;
    public float explosionDuration = 0.8f;
    public float maxSizeBeforeDisapearing = 20;
    public float explosionForce = 100;

    //
    private SphereCollider sphereCollider;
    private Rigidbody rb;
    private bool exploding = false;
    private float currentExplosionDuration = 0;
    private EnemyHealth enemyHealth;

    // Start is called before the first frame update
    void Start()
    {
        sphereCollider = GetComponent<SphereCollider>();
        rb = GetComponent<Rigidbody>();  
        enemyHealth = GetComponent<EnemyHealth>();
    }

    // Update is called once per frame
    void Update()
    {
        //
        float dt = Time.deltaTime;
        //
        if(enemyHealth.isDead && !exploding)
        {
            Explode();
        }
        //
        if (exploding)
        {
            currentExplosionDuration += dt;
            transform.localScale = Vector3.one * (1 + (currentExplosionDuration / explosionDuration * maxSizeBeforeDisapearing));
            if (currentExplosionDuration >= explosionDuration)
            {
                Destroy(gameObject);
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (exploding)
        {
            ApplyPower(other);
        }
    }

    void Explode()
    {
        AudioManager.Instance.Play3dFx(transform.position, onImpactSound, 0.5f);
        //Destroy(gameObject);
        exploding = true;
        rb.velocity = Vector3.zero;
        rb.isKinematic = true;
        sphereCollider.isTrigger = true;
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
            playerController.ApplyDamage((int)explosionForce);
        }
        //
        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb)
        {
            Vector3 direction = other.transform.position - transform.position;
            rb.AddExplosionForce(explosionForce, transform.position, transform.localScale.x);
            //rb.AddForce(direction.normalized * explosionForce, ForceMode.Impulse);
        }
    }
}
