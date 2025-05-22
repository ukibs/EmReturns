using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss1ForceField : MonoBehaviour
{
    //
    public float shieldForce = 200;
    public float maxSize = 100f;
    [Header("Components")]
    public AudioClip startingClip;
    public AudioClip activationClip;

    //
    [HideInInspector] public bool working = false;

    //
    private MeshRenderer meshRenderer;

    private void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    private void OnTriggerStay(Collider other)
    {
        //
        if (working)
        {
            ApplyPower(other);
        }        
    }

    public void StartCycle(Material material)
    {
        gameObject.SetActive(true);
        if (meshRenderer)
            meshRenderer.material = material;
        AudioManager.Instance.Play3dFx(transform.position, startingClip, 0.1f);
    }

    public void UpdateSize(float amount)
    {
        transform.localScale = Vector3.one * amount * maxSize;
    }

    public void Activate(Material material)
    {
        working = true;
        meshRenderer.material = material;
        AudioManager.Instance.Play3dFx(transform.position, activationClip, 0.15f);
    }

    public void Deactivate()
    {
        working = false;
        gameObject.SetActive(false);
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
            playerController.ApplyDamage((int) shieldForce);
        }
        //
        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb)
        {
            Vector3 direction = other.transform.position - transform.position;
            //rb.AddExplosionForce(50, transform.position, 50);
            rb.AddForce(direction.normalized * shieldForce, ForceMode.Force);
        }
    }
}
