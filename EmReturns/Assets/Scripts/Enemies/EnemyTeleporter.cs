using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTeleporter : MonoBehaviour
{
    public enum TeleportationTrigger
    {
        External,
        PlayerNearerThanX,

        Count
    }

    public enum TeleportationState
    {
        None,
        Dissapearing,
        Reapearing,

        Count
    }

    [Header("Components")]
    public AudioClip dissapearingClip;
    public AudioClip reapearingClip;
    [Header("Parameters")]
    public TeleportationTrigger teleportationTrigger;
    public float fadeTime = 0.25f;

    private TeleportationState currentTeleportationState = TeleportationState.None;
    private float currentFadeTime = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Teleportation  triggers
        if(currentTeleportationState == TeleportationState.None)
        {
            switch (teleportationTrigger)
            {
                case TeleportationTrigger.PlayerNearerThanX:
                    Vector3 playerDirection = EM_PlayerController.Instance.transform.position - transform.position;
                    if (playerDirection.sqrMagnitude < Mathf.Pow(50, 2))
                    {
                        StartTeleportation();
                    }
                    break;
            }
        }        

        // Proper teleportation
        float dt = Time.deltaTime;
        if (currentTeleportationState != TeleportationState.None)
            currentFadeTime += dt;
        switch (currentTeleportationState)
        {
            case TeleportationState.Dissapearing:
                transform.localScale = new Vector3(Mathf.Lerp(1, 10, currentFadeTime/fadeTime), currentFadeTime / fadeTime, Mathf.Lerp(1, 10, currentFadeTime / fadeTime));
                if(currentFadeTime >= fadeTime)
                {
                    currentTeleportationState = TeleportationState.Reapearing;
                    currentFadeTime = 0;
                    transform.position = new Vector3(Random.Range(-1000f, 1000f), 200, Random.Range(-1000f, 1000f));
                    AudioManager.Instance.Play2dFx(transform.position, reapearingClip, 0.6f);
                }
                break;
            case TeleportationState.Reapearing:
                transform.localScale = new Vector3(currentFadeTime / fadeTime, Mathf.Lerp(10, 1, currentFadeTime / fadeTime), currentFadeTime / fadeTime);
                if (currentFadeTime >= fadeTime)
                {
                    currentTeleportationState = TeleportationState.None;
                    currentFadeTime = 0;
                }
                break;
        }
    }

    public void StartTeleportation()
    {
        currentFadeTime = 0;
        currentTeleportationState = TeleportationState.Dissapearing;
        AudioManager.Instance.Play2dFx(transform.position, dissapearingClip, 0.6f);
    }

}
