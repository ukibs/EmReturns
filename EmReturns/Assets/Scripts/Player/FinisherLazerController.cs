using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinisherLazerController : MonoBehaviour
{
    //
    public LineRenderer lazerLineRenderer;
    public AudioClip firingClip;

    //
    public GameObject segmentPrefab;
    public float rateOfFire = 10;
    public LineRenderer lineRenderer;

    //
    private float currentFireTime = 0;

    //
    private float previousLoadAmount = 0;

    // Start is called before the first frame update
    void OnEnable()
    {
        lazerLineRenderer.startWidth = 10;
        lazerLineRenderer.endWidth = 10;
        //lazerLineRenderer.po
        //AudioManager.Instance.PlayLoadFx(firingClip, true, 1);
    }

    // Update is called once per frame
    void Update()
    {
        //
        transform.LookAt(EM_PlayerController.Instance.currentObjective.position);
        //
        if(EM_ShovelController.Instance.LoadAmount == 1)
        {
            //lazerLineRenderer.SetPosition(0, transform.position);
            //lazerLineRenderer.SetPosition(1, EM_PlayerController.Instance.currentObjective.position);
            // EM_PlayerController.Instance.SpendFinisherEnergy();
            //
            if(previousLoadAmount != 1)
                AudioManager.Instance.PlayLoadFx(firingClip, true, 1);
            //
            currentFireTime += Time.deltaTime;
            if (currentFireTime >= 1 / rateOfFire)
            {
                currentFireTime -= 1 / rateOfFire;
                GameObject nextSegment = Instantiate(segmentPrefab, transform.position, transform.rotation);
                //Rigidbody rb = nextSegment.GetComponent<Rigidbody>();
                //rb.velocity = transform.forward * 50;
            }
        }
        //
        previousLoadAmount = EM_ShovelController.Instance.LoadAmount;
    }

    private void OnDestroy()
    {
        AudioManager.Instance.StopLoadFx();
    }
}
