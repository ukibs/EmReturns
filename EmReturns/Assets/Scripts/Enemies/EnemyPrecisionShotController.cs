using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPrecisionShotController : MonoBehaviour
{
    public enum ShootingStatus
    {
        LoadingShot,
        Cooldown,

        Count
    }

    [Header("Components")]
    public Transform shootingPointPivot;
    public Transform shootingPoint;
    //public GameObject rapidFirePrefab;
    public AudioClip loadingFireClip;
    public AudioClip fireClip;
    public AudioClip ìmpactClip;
    public LineRenderer lineRenderer;
    public GameObject impactParticlesPrefab;
    public Transform appendixesAxis;
    public EnemyTeleporter teleporter;

    [Header("Parameters")]
    public float preparingShootTime = 5; // In rounds per second
    public float fireForce = 200;
    public float cooldownTime = 3;
    public float maxReach = 5000f;

    private ShootingStatus currentShootingStatus = ShootingStatus.Cooldown;
    private float currentPreparingShootTime = 0;
    //private float currentCooldownTime = 0;

    // Start is called before the first frame update
    void Start()
    {
        //StartCoroutine(PrecisionShotCoroutine());

        if (lineRenderer)
        {
            lineRenderer.transform.SetParent(null);
            lineRenderer.transform.position = Vector3.zero;
        }
    }

    // Update is called once per frame
    void Update()
    {        
        shootingPointPivot.LookAt(EM_PlayerController.Instance.transform.position);
        if (lineRenderer != null)
        {
            DrawLineRenderer();
        }
        //
        float dt = Time.deltaTime;
        currentPreparingShootTime += dt;
        switch (currentShootingStatus)
        {
            case ShootingStatus.Cooldown:
                if (currentPreparingShootTime >= cooldownTime)
                {
                    AudioManager.Instance.Play2dFx(shootingPoint.position, loadingFireClip, 0.6f);
                    currentPreparingShootTime = 0;
                    currentShootingStatus = ShootingStatus.LoadingShot;
                    lineRenderer.gameObject.SetActive(true);
                }
                break;
            case ShootingStatus.LoadingShot:
                if (currentPreparingShootTime >= preparingShootTime)
                {
                    Shoot();
                    currentPreparingShootTime = 0;
                    currentShootingStatus = ShootingStatus.Cooldown;
                    lineRenderer.gameObject.SetActive(false);
                }
                else
                {
                    lineRenderer.startWidth = currentPreparingShootTime / preparingShootTime;
                    lineRenderer.endWidth = currentPreparingShootTime / preparingShootTime;
                    appendixesAxis.localEulerAngles += Vector3.forward * (dt * 3600 * (currentPreparingShootTime / preparingShootTime));
                }
                break;
        }        
    }

    void DrawLineRenderer()
    {
        //
        lineRenderer.SetPosition(0, shootingPoint.position);
        //
        RaycastHit hitInfo;
        if (Physics.Raycast(shootingPoint.position, shootingPoint.forward, out hitInfo, maxReach))
        {
            //Vector3 distance = hitInfo.point - transform.position;
            lineRenderer.SetPosition(1, hitInfo.point);
        }
        else
        {
            lineRenderer.SetPosition(1, shootingPoint.position + (shootingPoint.forward * maxReach));
        }
    }

    void Shoot()
    {
        AudioManager.Instance.Play2dFx(shootingPoint.position, fireClip, 0.6f);
        //
        RaycastHit hitInfo;
        if (Physics.Raycast(shootingPoint.position, shootingPoint.forward, out hitInfo))
        {
            GameObject impactParticles = Instantiate(impactParticlesPrefab, hitInfo.point, Quaternion.LookRotation(hitInfo.normal));
            AudioManager.Instance.Play2dFx(hitInfo.point, ìmpactClip, 0.6f);
            //
            EM_PlayerController em_PlayerController = hitInfo.collider.gameObject.GetComponent<EM_PlayerController>();
            //Debug.Log("EM Player controller: ", em_PlayerController);
            if (em_PlayerController)
            {
                em_PlayerController.ApplyDamage((int)fireForce, true);
                em_PlayerController.Rb.AddForce(shootingPoint.forward * fireForce, ForceMode.Impulse);
            }
            //
            Rigidbody rigidbody = hitInfo.collider.gameObject.GetComponent<Rigidbody>();
            if (rigidbody != null)
            {
                rigidbody.AddForce(shootingPoint.forward * fireForce, ForceMode.Impulse);
            }
            //
            DestructibleObject destructibleObject = hitInfo.collider.gameObject.GetComponent<DestructibleObject>();
            if (destructibleObject != null)
            {
                destructibleObject.ApplyForce(shootingPoint.forward * fireForce);
            }
        }
        //
        if(teleporter != null)
        {
            teleporter.StartTeleportation();
        }
    }

    //IEnumerator PrecisionShotCoroutine()
    //{
    //    while (true)
    //    {
    //        AudioManager.Instance.Play3dFx(shootingPoint.position, loadingFireClip, 0.6f);
    //        //Debug.Log("Shooting rapid fire proyectile");
    //        yield return new WaitForSeconds(1f / fireRate);
    //        Shoot();
    //    }
    //}
}
