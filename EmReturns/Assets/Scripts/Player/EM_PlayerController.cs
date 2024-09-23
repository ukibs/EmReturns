using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class EM_PlayerController : MonoBehaviour
{
    //
    [Header("Parameters")]
    public float movementForce = 50;
    public float ragdollDuration = 1;
    public float invulnerabilityDuration = 1.5f;
    public float maxShields = 1000;
    public float maxHealth = 3;
    public float finisherDuration = 10;
    public float dashForce = 25;
    [Header("Components")]
    public GameObject hazardSignalier;
    [Header("HUD")]
    public RectTransform objectiveMarker;
    public RectTransform hazardMarker;
    public RectTransform usableMarker;
    public Image shieldBarBack;
    public Image shieldBarFront;
    public Image finisherBarBack;
    public Image finisherBarFront;
    public Image recoverBar;
    public GameObject recoverLetter;
    public GameObject[] healthIcons;
    public Animator finisherBarAnimator;
    public TMP_Text speedIndicator;
    public GameObject endGamePanel;
    public RawImage damageImage;
    [Header("Feedback")]
    public AudioClip damageClip;
    public AudioClip restorationClip;
    public AudioClip finisherEnergyClip;
    public AudioClip finisherLoadCompletedClip;
    public AudioClip dashClip;

    //
    [HideInInspector] public Transform currentObjective = null;
    [HideInInspector] public Transform nearestHazardOnScreen = null;
    [HideInInspector] public Transform nearestHazard = null;
    [HideInInspector] public Transform nearestUsable = null;

    //
    private static EM_PlayerController instance;
    private Rigidbody rb;
    private EM_ShovelController shovelController;
    [HideInInspector] public CameraController cameraController;

    private bool objectiveChangeAllowed = true;
    private Transform testObjective = null;
    private bool ragdolled = false;
    private bool dead = false;

    private float currentRagdollDuration = 0;
    private float currentInvulnerabilityDuration = 0;
    private float currentShield = 0;
    private float currentHealth = 0;
    private float currentShieldBarFilled = 0;

    private float currentFinisherEnergy = 0;
    private float currentFinisherEnergyFilled = 0;
    private bool finisherActive = false;

    private float dashChargeAmount = 0;

    private Boss1Controller boss1Controller;

    public static EM_PlayerController Instance { get { return instance; } }
    public bool Ragdolled { get { return ragdolled; } }
    public Rigidbody Rb { get { return rb; } }
    public bool FinisherLockAndLoaded
    {
        get
        {
            //
            if(!currentObjective) return false;
            //
            Boss1Controller boss1Controller = currentObjective.GetComponent<Boss1Controller>();
            if(boss1Controller && finisherActive)
            {
                return true;
            }
            //
            return false;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //
        instance = this;
        //
        rb = GetComponent<Rigidbody>();
        shovelController = GetComponent<EM_ShovelController>();
        cameraController = FindObjectOfType<CameraController>();
        //
        boss1Controller = FindObjectOfType<Boss1Controller>();
        //
        currentShield = maxShields;
        currentHealth = maxHealth;
        currentShieldBarFilled = maxShields;
    }

    //
    private void FixedUpdate()
    {
        //var gamepad = Gamepad.current;
        //if (gamepad == null)
        //    return; // No gamepad connected.
        //
        if (!ragdolled)
        {
            //
            switch (shovelController.currentShovelsState)
            {
                case EM_ShovelController.ShovelsState.Idle:
                case EM_ShovelController.ShovelsState.Hooked:
                case EM_ShovelController.ShovelsState.HookingRB:
                case EM_ShovelController.ShovelsState.LoadingPulseShot:
                case EM_ShovelController.ShovelsState.Sprint:
                case EM_ShovelController.ShovelsState.VerticalImpulse:
                case EM_ShovelController.ShovelsState.RapidFire:
                    //UpdateMovement(gamepad);
                    UpdateMovement();
                    break;
            }
        }              
    }

    // 
    void Update()
    {
        float dt = Time.deltaTime;

        //var gamepad = Gamepad.current;
        //if (gamepad == null)
        //    return; // No gamepad connected.

        //
        UpdateShieldAndFinisherFillers(dt);

        //
        if(currentInvulnerabilityDuration <= invulnerabilityDuration)
        {
            currentInvulnerabilityDuration += dt;
        }

        //
        if (!ragdolled)
        {
            //
            CheckShovelControls();

            //
            RecoverShield(dt);

            //
            UpdateDashChargeAmount(dt);

            //
            CheckDashControl();
        }
        else if(!dead)
        {
            UpdateRagdollState(dt);
            //
            if (InputController.Instance.JumpPressed)
            {
                currentRagdollDuration += 0.1f;
            }
        }
        //else
        //{
        //    if (gamepad.startButton.wasPressedThisFrame)
        //    {
        //        SceneManager.LoadScene(0);
        //    }
        //}

        // Control de fijado de cámara
        if (InputController.Instance.ObjectiveLockPressed)
        {
            CheckObjective();
        }

        //
        CheckHazards();
        //
        if (shovelController.hookedRb == null && currentObjective == null)
        {
            CheckUsables();
        }
        else
        {
            usableMarker.gameObject.SetActive(false);
        }

        // Si el finisher est activo solo puedes fijar la cabeza
        if (!finisherActive)
            UpdateObjectiveMangement();

        //if (gamepad.startButton.wasPressedThisFrame)
        //{
        //    SceneManager.LoadScene(0);
        //}
    }

    //
    private void LateUpdate()
    {
        speedIndicator.text = (int)(rb.velocity.magnitude * 3.6f) + " km/h";
    }

    //
    void UpdateShieldAndFinisherFillers(float dt)
    {
        // Fillers
        currentShieldBarFilled += Mathf.Sign(currentShield - currentShieldBarFilled) * (maxShields/6f) * dt;
        currentFinisherEnergyFilled += Mathf.Sign(currentFinisherEnergy - currentFinisherEnergyFilled) * 3.5f * dt;

        // Shield bars
        shieldBarBack.fillAmount = (currentShieldBarFilled > currentShield) ? currentShieldBarFilled / maxShields : currentShield / maxShields;
        shieldBarFront.fillAmount = (currentShieldBarFilled <= currentShield) ? currentShieldBarFilled / maxShields : currentShield / maxShields;

        // Finisher bars
        finisherBarBack.fillAmount = (currentFinisherEnergyFilled > currentFinisherEnergy) ? currentFinisherEnergyFilled / 100f : currentFinisherEnergy / 100f;
        finisherBarFront.fillAmount = (currentFinisherEnergyFilled <= currentFinisherEnergy) ? currentFinisherEnergyFilled / 100f : currentFinisherEnergy / 100f;
    }

    //
    void CheckShovelControls()
    {
        switch (shovelController.currentShovelsState)
        {
            case EM_ShovelController.ShovelsState.Idle:
                //
                if (InputController.Instance.GrapplePressed)
                {
                    shovelController.CheckGrabbingPoint();
                }
                //
                if (InputController.Instance.PulseShotPressed)
                {
                    shovelController.ChangeShovelsPosition(EM_ShovelController.ShovelsState.LoadingPulseShot);
                }
                //
                if (InputController.Instance.RapidFirePressed)
                {
                    shovelController.ChangeShovelsPosition(EM_ShovelController.ShovelsState.RapidFire);
                }
                //
                if (InputController.Instance.SprintPressed)
                {
                    shovelController.ChangeShovelsPosition(EM_ShovelController.ShovelsState.Sprint);
                }
                //
                if (InputController.Instance.JumpPressed)
                {
                    shovelController.ChangeShovelsPosition(EM_ShovelController.ShovelsState.VerticalImpulse);
                }
                break;
            case EM_ShovelController.ShovelsState.Hooked:
                //
                if (InputController.Instance.GrappleReleased)
                {
                    shovelController.ReturnShovelsToIdle();
                }
                break;
            case EM_ShovelController.ShovelsState.HookingRB:
                //
                if (InputController.Instance.GrappleReleased)
                {
                    shovelController.ReturnShovelsToIdle();
                }
                //
                if (InputController.Instance.PulseShotPressed)
                {
                    shovelController.ChangeShovelsPosition(EM_ShovelController.ShovelsState.LoadingPulseShot);
                }
                break;
            case EM_ShovelController.ShovelsState.LoadingPulseShot:
                if (InputController.Instance.GrappleReleased || 
                    (InputController.Instance.PulseShotReleased && shovelController.hookedRb))
                {
                    //shovelController.ReturnShovelsToIdle();
                    shovelController.ReleaseShot();
                }
                break;
            case EM_ShovelController.ShovelsState.Sprint:
                //
                if (InputController.Instance.SprintReleased)
                {
                    shovelController.ReturnShovelsToIdle();
                }
                break;
            case EM_ShovelController.ShovelsState.VerticalImpulse:
                //
                if (InputController.Instance.JumpReleased)
                {
                    shovelController.ReturnShovelsToIdle();
                }
                break;
            case EM_ShovelController.ShovelsState.RapidFire:
                //
                if (InputController.Instance.RapidFireReleased)
                {
                    shovelController.ReturnShovelsToIdle();
                }
                break;
        }
    }

    void CheckDashControl()
    {
        if (InputController.Instance.SprintPressed)
        {
            //Vector2 move = gamepad.leftStick.ReadValue();
            rb.AddForce(transform.forward * dashChargeAmount * dashForce, ForceMode.Impulse);
            AudioManager.Instance.Play2dFx(transform.position, dashClip, dashChargeAmount);
            dashChargeAmount = 0;
        }
    }

    void UpdateObjectiveMangement()
    {
        //
        if (currentObjective)
        {
            if (objectiveChangeAllowed && InputController.Instance.CameraAxis.sqrMagnitude > 0.25)
            {
                //cameraController
                currentObjective = cameraController.ChangeBossSegmentObjective(InputController.Instance.CameraAxis);
                StartCoroutine(WaitAndAllowObjectiveChange());
                //testObjective = cameraController.ChangeObjective(gamepad.rightStick.ReadValue());
            }
            //
            if (!objectiveMarker.gameObject.activeSelf)
                objectiveMarker.gameObject.SetActive(true);
            //
            if (currentObjective && testObjective)
            {
                Debug.DrawLine(currentObjective.position, testObjective.position, new Color(0, 0, 1));
            }
        }
        else
        {
            if (objectiveMarker.gameObject.activeSelf)
                objectiveMarker.gameObject.SetActive(false);
        }
    }

    void UpdateMovement()
    {

        // Movimiento
        // Vector2 move = gamepad.leftStick.ReadValue();
        Vector2 move = InputController.Instance.MovementAxis;
        //Debug.Log(move);
        //
        Vector3 directionZ = Camera.main.transform.forward * move.y;
        Vector3 directionX = Camera.main.transform.right * move.x;
        // The movement direction will depend on the current up
        directionZ = Vector3.ProjectOnPlane(directionZ, Vector3.up).normalized;
        //directionX = Vector3.ProjectOnPlane(directionX, Vector3.up).normalized;
        //
        Vector3 movementDirection = (directionX + directionZ).normalized;
        //
        CheckAndLook(movementDirection);        
        //
        rb.AddForce(movementDirection * movementForce);
        
    }

    void CheckObjective()
    {
        if (currentObjective)
        {
            currentObjective = null;
        }            
        else
        {
            //
            if (finisherActive)
            {
                // TODO: Que coja automaticamente el punto a atacar
                Boss1Controller boss1Controller = FindObjectOfType<Boss1Controller>();
                currentObjective = boss1Controller.transform;
            }
            else
            {
                //currentObjective = FindObjectOfType<Objective>().transform;
                //currentObjective = cameraController.GetNearestObjectiveToScreenCenter<LockableObjective>(shovelController.hookedRb);
                currentObjective = cameraController.GetNearestBossSectionToScreenCenter();
            }            
        }
    }

    void CheckHazards()
    {
        // On screen
        nearestHazardOnScreen = cameraController.GetNearestObjectiveToScreenCenter<Hazard>(shovelController.hookedRb);
        if(nearestHazardOnScreen != null)
        {
            Vector3 nearestHazardScreenPosition = Camera.main.WorldToScreenPoint(nearestHazardOnScreen.position);
            //hazardMarker.anchoredPosition = new Vector2((nearestHazardScreenPosition.x - 0.5f) * (Screen.width / 2), (nearestHazardScreenPosition.y - 0.5f) * (Screen.height / 2));
            hazardMarker.anchoredPosition = nearestHazardScreenPosition;
            hazardMarker.gameObject.SetActive(true);
        }
        else
        {
            hazardMarker.gameObject.SetActive(false);
        }

        // In total
        nearestHazard = cameraController.GetNearestObjectiveToPlayer<Hazard>(shovelController.hookedRb);
        if(nearestHazard != null)
        {
            hazardSignalier.gameObject.SetActive(true);
            hazardSignalier.transform.LookAt(nearestHazard.transform.position);
        }
        else
        {
            hazardSignalier.gameObject.SetActive(false);
        }
    }

    void CheckUsables()
    {
        nearestUsable = cameraController.GetNearestObjectiveToScreenCenter<Scenario>(shovelController.hookedRb);
        if (nearestUsable != null)
        {
            Vector3 nearestUsableScreenPosition = Camera.main.WorldToScreenPoint(nearestUsable.position);
            //usableMarker.anchoredPosition = new Vector2((nearestUsableScreenPosition.x - 0.5f) * (Screen.width / 2), (nearestUsableScreenPosition.y - 0.5f) * (Screen.height / 2));
            usableMarker.anchoredPosition = nearestUsableScreenPosition;
            usableMarker.gameObject.SetActive(true);
        }
        else
        {
            usableMarker.gameObject.SetActive(false);
        }
    }

    void CheckAndLook(Vector3 movementDirection)
    {
        if (nearestHazardOnScreen && shovelController.currentShovelsState == EM_ShovelController.ShovelsState.RapidFire)
        {
            Rigidbody rapidFireRb = shovelController.rapidFirePrefab.GetComponent<Rigidbody>();
            Rigidbody hazardRb = nearestHazardOnScreen.gameObject.GetComponent<Rigidbody>();
            float timeToReach = GeneralFunctions.EstimateTimeBetweenTwoPoints(
                transform.position, nearestHazardOnScreen.position, shovelController.rapidFireForce / rapidFireRb.mass);
            //Debug.Log("Time to reach: " + timeToReach);
            Vector3 hazardFuturePosition = GeneralFunctions.EstimateFuturePosition(nearestHazardOnScreen.position, hazardRb.velocity, timeToReach);
            transform.LookAt(hazardFuturePosition);
        }
        else if (currentObjective && 
            (shovelController.currentShovelsState == EM_ShovelController.ShovelsState.LoadingPulseShot ||
            shovelController.currentShovelsState == EM_ShovelController.ShovelsState.RapidFire)
            ) 
        {
            if(shovelController.currentShovelsState == EM_ShovelController.ShovelsState.RapidFire)
            {
                Rigidbody rapidFireRb = shovelController.rapidFirePrefab.GetComponent<Rigidbody>();
                FakeRigidbody fakeRigidbody = currentObjective.gameObject.GetComponent<FakeRigidbody>();
                Boss1SegmentController boss1SegmentController = currentObjective.gameObject.GetComponent <Boss1SegmentController>();
                float timeToReach = GeneralFunctions.EstimateTimeBetweenTwoPoints(
                    transform.position, fakeRigidbody.transform.position, shovelController.rapidFireForce / rapidFireRb.mass);
                //Debug.Log("Time to reach: " + timeToReach);
                Vector3 bossVelocity = boss1SegmentController.Velocity;
                Vector3 hazardFuturePosition = GeneralFunctions.EstimateFuturePosition(
                    fakeRigidbody.transform.position, 
                    fakeRigidbody.currentVelocity + bossVelocity, 
                    timeToReach
                    
                    );
                transform.LookAt(hazardFuturePosition);
            }
            else
            {
                transform.LookAt(currentObjective);
            }            
        }
        else if (
            shovelController.currentShovelsState == EM_ShovelController.ShovelsState.LoadingPulseShot || 
            shovelController.currentShovelsState == EM_ShovelController.ShovelsState.RapidFire
        )
        {
            if (cameraController.objectiveInCenter)
            {
                transform.LookAt(cameraController.objectiveInCenterPoint);
            }
            else
            {
                transform.rotation = cameraController.ProperCameraRotation;
            }
        }
        else
        {
            //
            transform.LookAt(transform.position + movementDirection);
        }
    }

    IEnumerator WaitAndAllowObjectiveChange()
    {
        objectiveChangeAllowed = false;
        yield return new WaitForSeconds(0.5f);
        objectiveChangeAllowed = true;
    }

    public void ApplyDamage(int damage)
    {
        //
        //Debug.Log("Damaging player");
        if (currentInvulnerabilityDuration <= invulnerabilityDuration)
        {
            return;
        }
        //
        if (!ragdolled)
        {
            shovelController.ReturnShovelsToIdle();
            ragdolled = true;
            rb.freezeRotation = false;
            AudioManager.Instance.Play3dFx(transform.position, damageClip, 0.5f);
            recoverLetter.SetActive(true);
            currentRagdollDuration = 0;
            currentInvulnerabilityDuration = 0;
            //
            //CameraEffects.Instance.ShakeEffect(0.15f, 2, 10);
            CameraEffects.Instance.FovEffect(0.15f, 50);
        }
        else
        {
            //damage /= 10;
            damage = 0;
        }
        //
        currentShield -= damage;
        if(currentShield <= 0)
        {
            currentHealth--;
            currentShield = maxShields;
            UpdateHealthIcons();
            //
            if(currentHealth == 0)
            {
                //AudioManager.Instance.Play2dFx(transform.position, damageClip, 1);
                //endGamePanel.SetActive(true);
                LevelManager.Instance.EndLevel(false);
                dead = true;
            }
        }
        Debug.Log("Attack received, current shield: " + currentShield + " - Current shield bar filled: " + currentShieldBarFilled + " - Max health: " + maxHealth);
        // TODO:
        //shieldBarFront.fillAmount = currentShield / maxShields;
        //finisherBar.fillAmount = currentHealth / maxHealth;
        //
        float smo = 50;
        rb.AddTorque(new Vector3(Random.Range(-smo, smo), Random.Range(-smo, smo), Random.Range(-smo, smo)));        
    }

    void UpdateHealthIcons()
    {
        for(int i = 0; i < healthIcons.Length; i++)
        {
            if (i < currentHealth)
                healthIcons[i].SetActive(true);
            else
                healthIcons[i].SetActive(false);
        }
    }

    void UpdateRagdollState(float dt)
    {
        currentRagdollDuration += dt;
        recoverBar.fillAmount = currentRagdollDuration / ragdollDuration;
        damageImage.color = new Color(255, 0, 0, 0.5f - (currentRagdollDuration / ragdollDuration * 0.5f));
        if(currentRagdollDuration >= ragdollDuration)
        {            
            RestoreControls();
        }
    }

    void RestoreControls()
    {
        recoverBar.fillAmount = 0;
        ragdolled = false;
        rb.freezeRotation = true;
        transform.eulerAngles = Vector3.zero;
        AudioManager.Instance.Play3dFx(transform.position, restorationClip, 1);
        recoverLetter.SetActive(false);
    }

    void RecoverShield(float dt)
    {
        currentShield += dt;
        currentShield = Mathf.Min(currentShield, maxShields);
        // TODO:
        //shieldBarFront.fillAmount = currentShield / maxShields;
    }

    void UpdateDashChargeAmount(float dt)
    {
        if(dashChargeAmount < 1)
        {
            dashChargeAmount += dt;
            //
            if(dashChargeAmount > 1)
                dashChargeAmount = 1;
            //
            recoverBar.fillAmount = dashChargeAmount;
        }
        
    }

    public void GetFinisherEnergy(int energy = 1)
    {
        currentFinisherEnergy++;
        // TODO:
        //finisherBarFront.fillAmount = (float)currentFinisherEnergy / 100f;
        AudioManager.Instance.Play2dFx(transform.position, finisherEnergyClip, 0.5f);
        if (currentFinisherEnergy >= 100)
        {
            // TODO: Activar modo finisher
            AudioManager.Instance.Play2dFx(transform.position, finisherLoadCompletedClip, 1f);
            finisherActive = true;
            finisherBarAnimator.SetBool("Full", true);
            // Extra para fijar al boss
            currentObjective = null;
            objectiveMarker.gameObject.SetActive(true);
            CheckObjective();
        }
    }

    public void SpendFinisherEnergy()
    {
        float dt = Time.deltaTime;
        currentFinisherEnergy -= dt * 100f / finisherDuration;
        // TODO:
        //finisherBarFront.fillAmount = currentFinisherEnergy / 100f;
        //
        boss1Controller.SufferDamage(dt);
        boss1Controller.GetComponent<FakeRigidbody>().AddForce(transform.forward * 10);
        //
        if(currentFinisherEnergy <= 0)
        {
            currentFinisherEnergy = 0;
            finisherActive = false;
            finisherBarAnimator.SetBool("Full", false);
            //
            EM_ShovelController.Instance.CheckAndDestroyFinisherController();
            //            
            boss1Controller.EndPhase();
        }
    }
}
