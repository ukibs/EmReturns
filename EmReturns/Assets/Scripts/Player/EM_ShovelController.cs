using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EM_ShovelController : MonoBehaviour
{
    //
    public enum ShovelsState{
        Idle,
        Hooked,
        HookingRB,
        LoadingPulseShot,
        Sprint,
        VerticalImpulse,
        RapidFire,

        Count
    }

    //
    [Header("Parameters")]
    public float hookForce = 100;
    public float pulseForce = 1000;
    public float sprintForce = 100;
    [Header("Components")]
    public Transform[] shovels;
    //public Transform shovelHookAnchor;
    //public Transform[] anchorPositions;
    public Transform hookedRbPoint;
    public Transform[] shovelPosturesPositions;
    public LineRenderer hookLineRenderer;
    public GameObject energyPulseAttackPrefab;
    public GameObject powerTrail;
    public GameObject finisherEnergyBall;
    [Header("Canvas")]
    public Image loadBar;
    [Header("Feedback")]
    public AudioClip loadingClip;
    public AudioClip propulsionClip;
    public AudioClip shootClip;
    public AudioClip grabClip;
    [Header("Debug")]
    public TMP_Text grabbingDistanceIndicator;

    //
    [HideInInspector] public ShovelsState currentShovelsState = ShovelsState.Idle;
    [HideInInspector] public Rigidbody hookedRb = null;

    //
    private static EM_ShovelController instance;
    private Vector3[] shovelsOriginalPositions;
    private Quaternion[] shovelsOriginalRotations;
    private Rigidbody rb;    
    private Transform currentShovelPosturePositions;
    private float loadAmount = 0;
    private Vector3 hookedRbPointOriginalPosition;

    //^Para chequeo de agarre
    private Vector3 previousGrabbedObjectDistance = Vector3.positiveInfinity;
    private int chekcingNearingGrabDistanceTicks = 0;

    //
    public static EM_ShovelController Instance { get { return instance; } }
    public float LoadAmount { get { return loadAmount; } }
    public LockableObjective HookedObjective
    {
        get
        {
            if (!hookedRb) return null;
            LockableObjective hookedObjective = hookedRb.GetComponent<LockableObjective>();
            return hookedObjective;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //
        instance = this;
        //
        shovelsOriginalPositions = new Vector3[shovels.Length];
        shovelsOriginalRotations = new Quaternion[shovels.Length];
        for (int i = 0; i < shovels.Length; i++)
        {
            shovelsOriginalPositions[i] = shovels[i].localPosition;
            shovelsOriginalRotations[i] = shovels[i].localRotation;
        }
        //
        rb = GetComponent<Rigidbody>();
        //
        hookedRbPointOriginalPosition = hookedRbPoint.localPosition;
    }

    // 
    void FixedUpdate()
    {
        if (!EM_PlayerController.Instance.Ragdolled)
        {
            switch (currentShovelsState)
            {
                case ShovelsState.Hooked:
                    HookAtraction();
                    break;
                case ShovelsState.HookingRB:
                case ShovelsState.LoadingPulseShot:
                    CarryRb();
                    break;
                case ShovelsState.Sprint:
                    ApplySprintForce();
                    break;
                case ShovelsState.VerticalImpulse:
                    ApplyVerticalForce();
                    break;
                case ShovelsState.RapidFire:
                    // TODO: Update rapid fire
                    break;
            }
        }               
    }

    // 
    void Update()
    {
        //
        float dt = Time.deltaTime;
        //
        if (!EM_PlayerController.Instance.Ragdolled)
        {
            //
            switch (currentShovelsState)
            {
                case ShovelsState.Hooked:
                    //
                    UpdateShovelsPositions();
                    break;
                case ShovelsState.LoadingPulseShot:
                    //
                    //currentShovelPosturePositions.Rotate(Vector3.forward * 360f);
                    //currentShovelPosturePositions.localEulerAngles = Vector3.forward * (Time.time * 360);
                    currentShovelPosturePositions.localEulerAngles += Vector3.forward * (dt * 1800 * loadAmount);
                    //Debug.Log("Rotating " + currentShovelPosturePositions.name);
                    UpdateShovelsPositions();
                    //
                    loadAmount += dt;
                    loadAmount = Mathf.Min(loadAmount, 1);
                    loadBar.fillAmount = loadAmount;
                    // TODO: Igual hay que ahcer aqui algo con el finisher
                    break;
            }
        }        
    }

    //
    void UpdateShovelsPositions()
    {
        for (int i = 0; i < shovels.Length; i++)
        {
            shovels[i].position = currentShovelPosturePositions.GetChild(i).position;
            shovels[i].rotation = currentShovelPosturePositions.GetChild(i).rotation;
        }
        //
        hookLineRenderer.SetPosition(0, transform.position);
        hookLineRenderer.SetPosition(1, currentShovelPosturePositions.position);
    }

    //
    public bool CheckGrabbingPoint()
    {
        //
        RaycastHit hitInfo;        
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hitInfo)){
            //
            //Debug.Log("Checking grabbing point: " + hitInfo.transform.gameObject.name + " - layer: " + hitInfo.transform.gameObject.layer);
            //Debug.Log("Checking grabbing point: " + hitInfo.collider.gameObject.name + " - layer: " + hitInfo.collider.gameObject.layer);
            //
            if (hitInfo.collider.gameObject.layer == 8 || hitInfo.collider.gameObject.layer == 9) return false;
            //
            Rigidbody hitRigidbody = hitInfo.transform.GetComponent<Rigidbody>();
            if (hitRigidbody)
            {
                hookedRb = hitRigidbody;
            }
            //
            ChangeShovelsPosition(ShovelsState.Hooked);
            hookLineRenderer.gameObject.SetActive(true);
            //
            currentShovelPosturePositions.position = hitInfo.point;
            currentShovelPosturePositions.forward = -hitInfo.normal;
            //currentShovelsState = ShovelsState.Hooked;
            //
            //for(int i = 0; i < shovels.Length; i++)
            //{
            //    shovels[i].position = currentShovelPosturePositions.GetChild(i).position;
            //}
            //
            currentShovelPosturePositions.parent = hitInfo.transform;
            //previousGrabbedObjectDistance = currentShovelPosturePositions.position - transform.position;
            previousGrabbedObjectDistance = Vector3.positiveInfinity;
            chekcingNearingGrabDistanceTicks = 0;

            AudioManager.Instance.Play2dFx(transform.position, grabClip, 0.5f);
        }
        //
        return false;
    }

    //
    void HookAtraction()
    {
        //
        Vector3 hookDirection = currentShovelPosturePositions.position - transform.position;
        Debug.DrawLine(currentShovelPosturePositions.position, transform.position, Color.blue);
        grabbingDistanceIndicator.text = (int)hookDirection.magnitude + "";
        // TODO: Magnitud segun el tamaño del objeto agarrado
        //if(hookDirection.sqrMagnitude < 100)
        //if (hookDirection.magnitude < 10)
        if(hookDirection.magnitude < 45 && previousGrabbedObjectDistance.sqrMagnitude > hookDirection.sqrMagnitude) //NOTA: Chequeamos que se esté alejando (se pasa de largo)
        {
            chekcingNearingGrabDistanceTicks++;
            // Cambio de estado
            if (chekcingNearingGrabDistanceTicks >= 5 && hookedRb && !hookedRb.isKinematic)
            {
                hookedRbPoint.localPosition = new Vector3(0, 0, (hookedRb.transform.localScale.y + 3) * 0.05f);
                //ChangeShovelsPosition(ShovelsState.HookingRB);
                ChangeShovelsPosition(ShovelsState.LoadingPulseShot);
                EM_PlayerController.Instance.currentObjective = null;
                hookLineRenderer.gameObject.SetActive(false);
            }
        }
        else
        {
            //
            previousGrabbedObjectDistance = hookDirection;
            chekcingNearingGrabDistanceTicks = 0;
            // TODO: Pillar la velocity del rigidbody agarrado y aplicarla al jugador

            //
            rb.AddForce(hookDirection.normalized * hookForce);
            //
            if (hookedRb)
            {
                hookedRb.AddForce(-hookDirection.normalized * hookForce);
            }
        }        
    }

    //
    void ApplySprintForce()
    {
        rb.AddForce(transform.forward * sprintForce);
    }

    //
    void ApplyVerticalForce()
    {
        rb.AddForce(transform.up * sprintForce);
    }

    //
    void CarryRb()
    {
        // Por si se pierde el rb por alguna razón
        if (!hookedRb)
        {
            ReturnShovelsToIdle();
            return;
        }
        //
        //Vector3 hookDirection = hookedRbPoint.position - hookedRb.transform.position;
        //float forceToApply = Mathf.Max(hookForce, hookDirection.magnitude * hookedRb.mass);
        //hookedRb.AddForce(hookDirection.normalized * forceToApply);

        //
        hookedRb.transform.position = hookedRbPoint.position;
        hookedRb.velocity = Vector3.zero;
    }

    //
    public void ReturnShovelsToIdle()
    {
        //
        if (currentShovelsState == ShovelsState.Idle) return;
        //
        currentShovelsState = ShovelsState.Idle;
        //
        for (int i = 0; i < shovels.Length; i++)
        {
            shovels[i].localPosition = shovelsOriginalPositions[i];
            shovels[i].localRotation = shovelsOriginalRotations[i];
        }
        // TODO: Mirar donde hacer esto
        currentShovelPosturePositions.parent = transform;
        currentShovelPosturePositions = null;
        //
        hookLineRenderer.gameObject.SetActive(false);
        //
        powerTrail.SetActive(false);
        //
        CheckAndDestroyFinisherController();
        //
        hookedRb = null;
        //
        AudioManager.Instance.StopLoadFx();
    }

    //
    public void ChangeShovelsPosition(ShovelsState nextState)
    {
        //
        if(nextState == ShovelsState.Idle)
        {            
            ReturnShovelsToIdle();            
            return;
        }
        //
        else
        {
            //
            switch (nextState)
            {
                case ShovelsState.Hooked: currentShovelPosturePositions = shovelPosturesPositions[0]; break;
                case ShovelsState.HookingRB: 
                    currentShovelPosturePositions = shovelPosturesPositions[1]; 
                    break;
                case ShovelsState.LoadingPulseShot: 
                    currentShovelPosturePositions = shovelPosturesPositions[1];
                    if (!hookedRb)
                    {
                        if (EM_PlayerController.Instance.FinisherLockAndLoaded)
                        {
                            GameObject finisher = Instantiate(finisherEnergyBall, hookedRbPoint.position, transform.rotation);
                            hookedRb = finisher.GetComponent<Rigidbody>();
                        }
                        else
                        {
                            GameObject pulseAttack = Instantiate(energyPulseAttackPrefab, hookedRbPoint.position, transform.rotation);
                            hookedRb = pulseAttack.GetComponent<Rigidbody>();
                        }                                               
                    }
                    AudioManager.Instance.PlayLoadFx(loadingClip, false, 1);
                    break;
                case ShovelsState.Sprint:
                    currentShovelPosturePositions = shovelPosturesPositions[2];
                    powerTrail.transform.localPosition = currentShovelPosturePositions.localPosition;
                    powerTrail.SetActive(true);
                    AudioManager.Instance.PlayLoadFx(propulsionClip, true, 1);
                    break;
                case ShovelsState.VerticalImpulse:
                    currentShovelPosturePositions = shovelPosturesPositions[3];
                    powerTrail.transform.localPosition = currentShovelPosturePositions.localPosition;
                    powerTrail.SetActive(true);
                    AudioManager.Instance.PlayLoadFx(propulsionClip, true, 1);
                    break;
            }
            //
            for (int i = 0; i < shovels.Length; i++)
            {
                shovels[i].position = currentShovelPosturePositions.GetChild(i).position;
                shovels[i].rotation = currentShovelPosturePositions.GetChild(i).rotation;
            }
        }        
        //
        currentShovelsState = nextState;
    }

    //
    public void ReleaseShot()
    {
        if (hookedRb)
        {
            Debug.Log("Releasing shot");
            // Si lo que soltamos es el el finisher...
            FinisherLazerController finisherLazerController = hookedRb.GetComponent<FinisherLazerController>();
            if (finisherLazerController)
            {
                Destroy(hookedRb.gameObject);
            }
            else
            {
                // Si lo que vamos a lanzar es el pulso de energía...
                EnergyPulseAttack energyPulseAttack = hookedRb.GetComponent<EnergyPulseAttack>();
                if (energyPulseAttack)
                {
                    energyPulseAttack.transform.forward = transform.forward;
                    energyPulseAttack.Launch(loadAmount);
                }
                else
                {
                    hookedRb.gameObject.AddComponent<EnemyDamager>();
                }
                //
                // Debug.Log("Shooting pulse attack - Proyectile: " + loadAmount);
                //
                hookedRb.AddForce(transform.forward * loadAmount * pulseForce, ForceMode.Impulse);
                rb.AddForce(-transform.forward * 0.1f * loadAmount * pulseForce, ForceMode.Impulse);
                //
                //CameraEffects.Instance.ShakeEffect(0.15f, 1, 10);
                CameraEffects.Instance.FovEffect(0.1f, 50);
            }            
            loadAmount = 0;
            loadBar.fillAmount = 0;
            hookedRb = null;
            hookedRbPoint.localPosition = hookedRbPointOriginalPosition;
            //
            AudioManager.Instance.Play3dFx(transform.position, shootClip, 0.6f);
        }
        ReturnShovelsToIdle();
    }

    public void CheckAndDestroyFinisherController()
    {
        if (hookedRb)
        {
            Debug.Log("Releasing shot");
            // Si lo que soltamos es el el finisher...
            FinisherLazerController finisherLazerController = hookedRb.GetComponent<FinisherLazerController>();
            if (finisherLazerController)
            {
                Destroy(hookedRb.gameObject);
            }
        }
    }
}
