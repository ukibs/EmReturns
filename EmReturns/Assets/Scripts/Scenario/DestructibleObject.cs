using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructibleObject : MonoBehaviour
{
    //
    public float resistance = 50;
    public GameObject destroyedVersion;
    public AudioClip destructionClip;
    //
    private TutorialDestructibleObjective tutorialDestructibleObjective;

    //
    [HideInInspector] public bool isDestroyed = false;

    // Start is called before the first frame update
    void Start()
    {
        //tutorialDestructibleObjective = GetComponent<TutorialDestructibleObjective>();
        //Debug.Log(tutorialDestructibleObjective);
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
                DestroyObject();
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
            DestroyObject();
        }
    }

    public void DestroyObject()
    {
        tutorialDestructibleObjective = gameObject.GetComponent<TutorialDestructibleObjective>();
        //Debug.Log(tutorialDestructibleObjective);
        if (tutorialDestructibleObjective != null)
        {
            Debug.Log("Has componet");
            TutorialManager.Instance.CheckAndNextPhase();
        }
        //else
        //{
        //    Debug.Log("Has not componet");
        //}

        isDestroyed = true;
        gameObject.SetActive(false);
        destroyedVersion.SetActive(true);
        
    }
}
