using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnergyPulseAttack : MonoBehaviour
{
    //
    public int stepsDuration = 20;  // Un segundo en fixed delta time
    public float startingForce = 100f;
    public float finalForce = 10f;
    public float finalSize = 50;

    //
    private int currentStep = 0;
    private bool launched = false;
    //private int startingStep = 0;
    //private float maxPossible = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void Launch(float loadAmount)
    {
        Debug.Log("Launching energy pulse attack");
        launched = true;
        //startingStep = (int)(stepsDuration * (1 - loadAmount));
        //maxPossible = ((float)stepsDuration - (float)startingStep) / (float)stepsDuration;
        //stepsDuration -= startingStep;

        //
        stepsDuration = (int)((float)stepsDuration * loadAmount);
        startingForce = startingForce * loadAmount;
        finalSize = finalSize * loadAmount;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (launched)
        {
            //transform.Translate(Vector3.forward * 1);
            transform.localScale = Vector3.one * Mathf.Lerp(1, finalSize, (float)currentStep / (float)stepsDuration);
            //
            currentStep++;
            if (currentStep >= stepsDuration)
            {
                Destroy(gameObject);
            }
        }        
    }

    private void OnTriggerStay(Collider other)
    {
        //
        if (!launched) return;
        //
        Vector3 direction = other.transform.position - transform.position;
        //
        EM_PlayerController playerController = other.gameObject.GetComponent<EM_PlayerController>();
        if (playerController != null)
            return;
        // Para cuerpos con rigidbody
        Rigidbody rb = other.attachedRigidbody;
        if (rb)
        {
            //rb.AddForce(direction.normalized * Mathf.Lerp(startingForce, finalForce, (float)currentStep / (float)stepsDuration), ForceMode.Impulse);
            rb.AddForce(transform.forward * Mathf.Lerp(startingForce, finalForce, (float)currentStep / (float)stepsDuration), ForceMode.Impulse);            
        }
        // Cheuqeo de fake rigidbody
        FakeRigidbody fakeRigidbody = other.GetComponentInParent<FakeRigidbody>();
        if (fakeRigidbody)
        {
            //Debug.Log("Has fake RB");
            fakeRigidbody.AddForce(transform.forward * Mathf.Lerp(startingForce, finalForce, (float)currentStep / (float)stepsDuration));
        }
        // Para elementos destruibles
        DestructibleObject destructibleObject = other.GetComponent<DestructibleObject>();
        if (destructibleObject)
        {
            destructibleObject.ApplyForce(direction.normalized * Mathf.Lerp(startingForce, finalForce, (float)currentStep / (float)stepsDuration));
            return;
        }
        //
        Boss1SegmentController boss1SegmentController = other.GetComponentInParent<Boss1SegmentController>();
        if (boss1SegmentController)
        {
            boss1SegmentController.SufferDamage((int)Mathf.Lerp(startingForce, finalForce, (float)currentStep / (float)stepsDuration));
            return;
        }
        //
        Boss1Controller boss1Controller = other.GetComponentInParent<Boss1Controller>();
        if (boss1Controller)
        {
            boss1Controller.SufferDamage(Time.deltaTime * 1);
            return;
        }
        // Chequeo extra - Si no es ninguna de las anteriores, le metemos para que pueda dañar al boss
        if (rb)
        {
            rb.gameObject.AddComponent<EnemyDamager>();
        }        
    }
}
