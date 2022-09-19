using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinisherEndSphere : MonoBehaviour
{
    //
    public AudioClip explosionSound;
    public float explosionDuration = 0.8f;
    public float maxSizeBeforeDisapearing = 20;
    public float explosionForce = 100;

    //
    private float currentExplosionDuration = 0;

    // Start is called before the first frame update
    void Start()
    {
        //AudioManager.Instance.Play3dFx(transform.position, explosionSound, 1);
        AudioManager.Instance.Play2dFx(transform.position, explosionSound, 1);
    }

    // Update is called once per frame
    void Update()
    {
        currentExplosionDuration += Time.deltaTime;
        transform.localScale = Vector3.one * (1 + (currentExplosionDuration / explosionDuration * maxSizeBeforeDisapearing));
        if (currentExplosionDuration >= explosionDuration)
        {
            Destroy(gameObject);
        }
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
            playerController.ApplyDamage(0);
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
