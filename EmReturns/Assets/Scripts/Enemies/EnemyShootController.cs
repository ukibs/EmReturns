using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShootController : MonoBehaviour
{
    [Header("Components")]
    public Transform shootingPointPivot;
    public Transform shootingPoint;
    public GameObject rapidFirePrefab;
    public AudioClip fireClip;

    [Header("Parameters")]
    public float fireRate = 1; // In rounds per second
    public float fireForce = 30;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(RapidFireCoroutine());
    }

    // Update is called once per frame
    void Update()
    {
        Rigidbody rapidFireRb = rapidFirePrefab.GetComponent<Rigidbody>();
        Rigidbody hazardRb = EM_PlayerController.Instance.gameObject.GetComponent<Rigidbody>();
        float timeToReach = GeneralFunctions.EstimateTimeBetweenTwoPoints(
            transform.position, hazardRb.position, fireForce / rapidFireRb.mass);
        //Debug.Log("Time to reach: " + timeToReach);
        Vector3 hazardFuturePosition = GeneralFunctions.EstimateFuturePosition(hazardRb.position, hazardRb.velocity, timeToReach);
        shootingPointPivot.LookAt(hazardFuturePosition);
    }


    IEnumerator RapidFireCoroutine()
    {
        //Debug.Log("Starting rapid fire coroutine: " + currentShovelsState.ToString());
        while (true)
        {
            GameObject newRapidFireBullet = Instantiate(rapidFirePrefab, shootingPoint.position, shootingPoint.rotation);
            Rigidbody bulletRB = newRapidFireBullet.GetComponent<Rigidbody>();
            bulletRB.AddForce(shootingPoint.forward * fireForce, ForceMode.Impulse);
            AudioManager.Instance.Play3dFx(shootingPoint.position, fireClip, 0.6f);
            //Debug.Log("Shooting rapid fire proyectile");
            yield return new WaitForSeconds(1f / fireRate);
        }
    }

}
