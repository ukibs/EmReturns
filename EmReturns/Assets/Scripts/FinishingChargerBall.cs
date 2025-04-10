using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinishingChargerBall : MonoBehaviour
{
    //
    public float movementSpeed = 100f;
    public float rotationSpeed = 30f;

    //
    //private Rigidbody rb;

    // Start is called before the first frame update
    void Start()
    {
        //rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        float dt = Time.deltaTime;
        CheckRaycastCollissions();
        UpdateMovement(dt);
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 6)
        {
            Destroy(gameObject);
            // TODO: Pawa para el player
            Debug.Log("Loading player finisher");
            EM_PlayerController.Instance.GetFinisherEnergy();
        }
    }

    private void CheckRaycastCollissions()
    {
        RaycastHit hitInfo;
        float dt = Time.deltaTime;
        float bulletTravelDistance = movementSpeed * dt;
        if (Physics.Raycast(transform.position, transform.forward, out hitInfo, bulletTravelDistance))
        {
            Collider collider = hitInfo.collider;
            if (collider.gameObject.layer == 6)
            {
                Destroy(gameObject);
                EM_PlayerController.Instance.GetFinisherEnergy();
            }
        }
    }

    void UpdateMovement(float dt)
    {        
        Vector3 playerDirection = EM_PlayerController.Instance.transform.position - transform.position;
        float rotationToUse = playerDirection.sqrMagnitude < 360f ? rotationSpeed * 4 : rotationSpeed;
        Vector3 newDirection = Vector3.RotateTowards(transform.forward, playerDirection,
                    rotationToUse * dt * Mathf.Deg2Rad, 0f);
        transform.rotation = Quaternion.LookRotation(newDirection);
        transform.Translate(Vector3.forward * dt * movementSpeed);
        // Extra para que no se peguen media vida persiguiendo al player
        movementSpeed += dt * 10;
        rotationSpeed += dt * 5;
    }
}
