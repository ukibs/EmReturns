using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss1EnergyBall : MonoBehaviour
{
    //
    public AudioClip onImpactSound;
    public float explosionDuration = 0.8f;
    public float maxSizeBeforeDisapearing = 20;
    public float explosionForce = 100;
    public float forceTowardsPlayer = 0;
    public bool checkDistance = false;
    public float timeToExplode = 15;
    //
    private SphereCollider sphereCollider;
    private Rigidbody rb;
    private bool exploding = false;
    private float currentExplosionDuration = 0;
    private float currentTimeToExplode = 0;

    // Start is called before the first frame update
    void Start()
    {
        sphereCollider = GetComponent<SphereCollider>();
        rb = GetComponent<Rigidbody>();
        // Por si acaso
        // Destroy(gameObject, 60);
    }

    //
    private void FixedUpdate()
    {
        if (!sphereCollider.isTrigger && !exploding && forceTowardsPlayer > 0)
        {
            Vector3 playerDirection = EM_PlayerController.Instance.transform.position - transform.position;
            rb.AddForce(playerDirection.normalized * forceTowardsPlayer);
        }
    }

    // Update is called once per frame
    void Update()
    {
        //
        float dt = Time.deltaTime;
        //
        if (exploding)
        {
            currentExplosionDuration += dt;
            transform.localScale = Vector3.one * (1 + (currentExplosionDuration/explosionDuration * maxSizeBeforeDisapearing));
            if(currentExplosionDuration >= explosionDuration)
            {
                Destroy(gameObject);
            }
        }
        else
        {
            currentTimeToExplode += dt;
            if (currentTimeToExplode >= timeToExplode)
                Explode();
        }
        //else if(timeToExplode != -1)
        //{
        //    timeToExplode -= dt;
        //    if (timeToExplode <= 0)
        //        Explode();
        //}
        //
        //
        if (checkDistance && !sphereCollider.isTrigger && !exploding)
        {
            Vector3 playerDistance = EM_PlayerController.Instance.transform.position - transform.position;
            if(playerDistance.sqrMagnitude < Mathf.Pow(maxSizeBeforeDisapearing, 2) / 4f)
            {
                Explode();
            }
        }
    }

    public void Activate()
    {
        sphereCollider.isTrigger = false;
    }

    public void Activate(float lifeTime)
    {
        sphereCollider.isTrigger = false;
        currentTimeToExplode = lifeTime;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Explode();
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
