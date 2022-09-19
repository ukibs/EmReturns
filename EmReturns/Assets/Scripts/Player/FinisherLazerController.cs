using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FinisherLazerController : MonoBehaviour
{
    //
    public LineRenderer lazerLineRenderer;
    public AudioClip firingClip;

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
        if(EM_ShovelController.Instance.LoadAmount == 1)
        {
            lazerLineRenderer.SetPosition(0, transform.position);
            lazerLineRenderer.SetPosition(1, EM_PlayerController.Instance.currentObjective.position);
            EM_PlayerController.Instance.SpendFinisherEnergy();
            //
            if(previousLoadAmount != 1)
                AudioManager.Instance.PlayLoadFx(firingClip, true, 1);
        }
        //
        previousLoadAmount = EM_ShovelController.Instance.LoadAmount;
    }

    private void OnDestroy()
    {
        AudioManager.Instance.StopLoadFx();
    }
}
