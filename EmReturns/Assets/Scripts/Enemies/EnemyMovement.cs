using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    public enum BehaviourType 
    {
        GoingToPlayer,
        GoingUp,
        EncirclingPlayer,

        Count
    }

    [Header("Parameters")]
    public BehaviourType behaviour;
    public float movementSpeed = 20;
    public float rotationSpeed = 180;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float dt = Time.deltaTime;
        UpdateMovement(dt);
    }

    void UpdateMovement(float dt)
    {
        // Rotación - Solo cuando no está out
        //if (currentStunnedTime <= 0)
        if (true)
        {
            //
            //Debug.Log("Not sutnned, acting normal");
            //
            Vector3 playerDirection = EM_PlayerController.Instance.transform.position - transform.position;
            Vector3 playerCross = Vector3.Cross(playerDirection, Vector3.up);
            Vector3 newDirection = transform.forward;
            //
            switch (behaviour)
            {
                case BehaviourType.GoingToPlayer:
                    newDirection = Vector3.RotateTowards(transform.forward, playerDirection,
                        rotationSpeed * dt * Mathf.Deg2Rad, 0f);
                    break;
                case BehaviourType.GoingUp:
                    newDirection = Vector3.RotateTowards(transform.forward, Vector3.up,
                        rotationSpeed * dt * Mathf.Deg2Rad, 0f);
                    break;
                case BehaviourType.EncirclingPlayer:
                    newDirection = Vector3.RotateTowards(transform.forward, playerCross,
                        rotationSpeed * dt * Mathf.Deg2Rad, 0f);
                    break;
            }

            //
            transform.rotation = Quaternion.LookRotation(newDirection);
        }

        transform.Translate(Vector3.forward * Time.deltaTime * movementSpeed);
    }
}
