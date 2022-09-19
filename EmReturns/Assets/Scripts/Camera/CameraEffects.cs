using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraEffects : MonoBehaviour
{
    //
    public float shakeDuration = 0.3f;
    public float shakesAmount = 1;
    public float maxShakeAngle = 15f;
    //
    private static CameraEffects instance;
    private Camera cameraComponent;

    private bool shaking = false;
    private float currentShakingDuration = 0;
    private int currentShakingDirection = 1;
    private int currentShakesAmount = 0;

    private float initialFov;
    private bool updatingFov;

    private float maxFovChangeDuration;
    private float maxFovVariation;

    private float currentFovChangeDuration;
    private float currentFovChangeDirection = 1;

    //
    public static CameraEffects Instance { get { return instance; } }   

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        //
        cameraComponent = GetComponent<Camera>();
        //
        initialFov = cameraComponent.fieldOfView;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        UpdateShaking();
        UpdateFovEffect();
    }

    #region Shake Methods
    void UpdateShaking()
    {
        if (shaking)
        {
            //
            Debug.Log("Shaking camera");
            //
            float dt = Time.deltaTime;
            //
            currentShakingDuration += dt * currentShakingDirection;
            transform.localEulerAngles = new Vector3(currentShakingDuration / shakeDuration * maxShakeAngle, 0, 0);
            //
            switch (currentShakingDirection)
            {
                case 1:
                    if (currentShakingDuration >= shakeDuration)
                    {
                        currentShakingDirection = -1;
                    }
                    break;
                case -1:
                    if (currentShakingDuration <= 0)
                    {
                        currentShakingDirection = 1;
                        currentShakesAmount++;
                        currentShakingDuration = 0;
                        if (currentShakesAmount >= shakesAmount)
                        {
                            shaking = false;
                            transform.localEulerAngles = Vector3.zero;
                        }
                    }
                    break;
            }
        }
    }

    public void ShakeEffect()
    {
        shaking = true;
        currentShakingDuration = 0;
        currentShakesAmount = 0;
    }

    public void ShakeEffect(float shakingDuration, int shakesAmount, float maxShakeAngle)
    {
        if (!shaking)
        {
            //
            shakeDuration = shakingDuration;
            this.shakesAmount = shakesAmount;
            this.maxShakeAngle = maxShakeAngle;
            //
            shaking = true;
            currentShakingDuration = 0;
            currentShakesAmount = 0;
        }        
    }
    #endregion

    #region FOV Methods

    void UpdateFovEffect()
    {
        if (updatingFov)
        {
            float dt = Time.deltaTime;
            currentFovChangeDuration += dt * currentFovChangeDirection;
            cameraComponent.fieldOfView = initialFov + ((currentFovChangeDuration/maxFovChangeDuration) * maxFovVariation * -1);

            switch (currentFovChangeDirection)
            {
                case 1:
                    if(currentFovChangeDuration > maxFovChangeDuration)
                    {
                        currentFovChangeDirection = -1;
                    }
                    break;
                case -1:
                    if (currentFovChangeDuration <= 0)
                    {
                        currentFovChangeDirection = 1;
                        //currentShakesAmount++;
                        currentFovChangeDuration = 0;
                        //if (currentShakesAmount >= shakesAmount)
                        //{
                        updatingFov = false;
                        cameraComponent.fieldOfView = initialFov;
                        //}
                    }
                    break;
            }
        }
    }

    public void FovEffect(float fovEffectDuration, float maxFovVariation)
    {
        if (!updatingFov)
        {
            //
            maxFovChangeDuration = fovEffectDuration;
            this.maxFovVariation = maxFovVariation;
            //
            updatingFov = true;
            currentFovChangeDuration = 0;
        }
    }

    #endregion
}
