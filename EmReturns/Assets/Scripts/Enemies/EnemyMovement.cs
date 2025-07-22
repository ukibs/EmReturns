using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : MonoBehaviour
{    

    [Header("Parameters")]
    //public BehaviourType behaviour;
    [SerializeField] public MovementBehaviour[] movementBehaviours;
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
            bool behaviourSelected = false;
            for(int i = 0 ; i < movementBehaviours.Length && !behaviourSelected; i++)
            {
                switch (movementBehaviours[i].behaviourCondition)
                {
                    case BehaviourConditions.None:
                        newDirection = ApplyBehaviour(movementBehaviours[i].behaviourType, playerDirection, playerCross, dt);
                        behaviourSelected = true;
                        break;
                    case BehaviourConditions.PlayerNearerThanX:
                        if(playerDirection.sqrMagnitude < Mathf.Pow(200, 2))
                        {
                            newDirection = ApplyBehaviour(movementBehaviours[i].behaviourType, playerDirection, playerCross, dt);
                            behaviourSelected = true;
                        }
                        break;
                    case BehaviourConditions.PlayerFarerThanX:
                        if (playerDirection.sqrMagnitude > Mathf.Pow(200, 2))
                        {
                            newDirection = ApplyBehaviour(movementBehaviours[i].behaviourType, playerDirection, playerCross, dt);
                            behaviourSelected = true;
                        }
                        break;
                }
            }
            

            //
            transform.rotation = Quaternion.LookRotation(newDirection);
        }

        transform.Translate(Vector3.forward * Time.deltaTime * movementSpeed);
    }

    Vector3 ApplyBehaviour(BehaviourType behaviour, Vector3 playerDirection, Vector3 playerCross, float dt)
    {
        //
        switch (behaviour)
        {
            case BehaviourType.GoingToPlayer:
                return Vector3.RotateTowards(transform.forward, playerDirection,
                    rotationSpeed * dt * Mathf.Deg2Rad, 0f);
            case BehaviourType.GoingUp:
                return Vector3.RotateTowards(transform.forward, Vector3.up,
                    rotationSpeed * dt * Mathf.Deg2Rad, 0f);
            case BehaviourType.EncirclingPlayer:
                return Vector3.RotateTowards(transform.forward, playerCross,
                    rotationSpeed * dt * Mathf.Deg2Rad, 0f);
            default:
                return transform.forward;
        }
    }
}



public enum BehaviourType
{
    GoingToPlayer,
    GoingUp,
    EncirclingPlayer,

    Count
}

public enum BehaviourConditions
{
    None,
    PlayerNearerThanX,
    PlayerFarerThanX,

    Count,
}

[Serializable]
public class MovementBehaviour
{
    public BehaviourType behaviourType;
    public BehaviourConditions behaviourCondition;
}