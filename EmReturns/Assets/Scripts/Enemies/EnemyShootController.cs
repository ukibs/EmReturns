using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShootController : MonoBehaviour
{
    [Header("Components")]
    public Transform shootingPointPivot;
    public Transform shootingPoint;
    public GameObject rapidFirePrefab;
    public AudioClip rapidFireClip;

    [Header("Parameters")]
    public float fireRate;
    public float rapidFireForce = 30;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(RapidFireCoroutine());
    }

    // Update is called once per frame
    void Update()
    {
        shootingPointPivot.LookAt(EM_PlayerController.Instance.transform.position);
    }

    IEnumerator RapidFireCoroutine()
    {
        //Debug.Log("Starting rapid fire coroutine: " + currentShovelsState.ToString());
        yield return new WaitForSeconds(0.1f);
        while (true)
        {
            GameObject newRapidFireBullet = Instantiate(rapidFirePrefab, shootingPoint.position, shootingPoint.rotation);
            Rigidbody bulletRB = newRapidFireBullet.GetComponent<Rigidbody>();
            bulletRB.AddForce(shootingPoint.forward * rapidFireForce, ForceMode.Impulse);
            AudioManager.Instance.Play3dFx(shootingPoint.position, rapidFireClip, 0.6f);
            //Debug.Log("Shooting rapid fire proyectile");
            yield return new WaitForSeconds(0.3f);
        }
    }
}
