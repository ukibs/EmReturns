using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss1PointingLazer : MonoBehaviour
{
    //
    public enum Status
    {
        Loading,
        Shooting
    }
    //
    public float shootMaxOffset = 10f;
    public float preparingRotationSpeed = 45f;
    public float attackingRotationSpeed = 10f;
    //
    private LineRenderer lineRenderer;
    private Vector3 offsetToPlayer;
    private Status currentStatus = Status.Loading;

    // Start is called before the first frame update
    void OnEnable()
    {
        //
        if(!lineRenderer)
            lineRenderer = GetComponent<LineRenderer>();
        //
        SetOffsetToPlayer();
    }

    // Update is called once per frame
    void Update()
    {
        //
        float dt = Time.deltaTime;
        //
        if(currentStatus == Status.Loading)
        {
            //transform.LookAt(EM_PlayerController.Instance.transform.position + offsetToPlayer);
            FollowPlayer(dt);
        }
        else
        {
            // TODO: Aplicaremos raycast y daño
        }
        
    }

    private void LateUpdate()
    {
        //
        DrawLineRenderer();
    }

    void SetOffsetToPlayer()
    {
        float smo = shootMaxOffset;
        offsetToPlayer = new Vector3(Random.Range(-smo, smo), Random.Range(-smo, smo), Random.Range(-smo, smo));
    }

    void FollowPlayer(float dt)
    {
        //
        float currentRotationSpeed = preparingRotationSpeed;
        //
        Vector3 playerDirection = EM_PlayerController.Instance.transform.position - transform.position + offsetToPlayer;
        Vector3 newDirection = transform.forward;
        newDirection = Vector3.RotateTowards(transform.forward, playerDirection,
                        currentRotationSpeed * dt * Mathf.Deg2Rad, 0f);
        transform.rotation = Quaternion.LookRotation(newDirection);

    }

    void DrawLineRenderer()
    {
        //
        lineRenderer.SetPosition(0, transform.position);
        //
        RaycastHit hitInfo;
        if(Physics.Raycast(transform.position, transform.forward, out hitInfo, 1000f))
        {
            //Vector3 distance = hitInfo.point - transform.position;
            lineRenderer.SetPosition(1, hitInfo.point);
        }
        else
        {            
            lineRenderer.SetPosition(1, transform.position + (transform.forward * 1000f));
        }
    }

    void DeActivate(bool activate)
    {
        if (activate)
        {
            currentStatus = Status.Shooting;
        }
        else
        {
            currentStatus = Status.Loading;
        }
    }
}
