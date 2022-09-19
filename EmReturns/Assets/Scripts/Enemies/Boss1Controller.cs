using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


public class Boss1Controller : MonoBehaviour
{
    

    //
    public enum LazerState
    {
        CoolDown,
        Loading,
        Shooting, 
        
        Count
    }

    //
    [Header("Components")]
    public GameObject segmentPrefab;
    public Transform conectionPoint;
    public Boss1ForceField forceField;
    public GameObject smallLazer;
    public GameObject bigLazer;
    public GameObject endPhaseExplosionPrefab;
    [Header("Parameters")]
    public int numberOfSegments = 20;
    //public float movementSpeed = 10f;
    //public float rotationSpeed = 90f;
    public float energyBallPreparation = 1;
    public float energyBallCooldown = 5;
    public float energyShieldPreparation = 2;
    public float energyShieldDuration = 5;
    [Header("Behaviours")]
    public AudioClip combatMusic;
    //public BossBehaviourSO[] bossBehaviours;
    public BossPhaseSO[] bossPhases;
    [Header("Materials")]
    public MeshRenderer[] shaderMeshRenderers;
    public Material healthyMaterial;
    public Material damagedMaterial;
    [Header("FX")]
    public AudioClip loadingLazerClip;
    public AudioClip shootingLazerClip;
    public AudioClip headHitEffect;

    //
    private Boss1SegmentController[] segmentControllers;
    //private BossBehaviourType currentBossBehaviour = BossBehaviourType.GoingToPlayer;
    private Rigidbody rb;
    private int currentBehaviourIndex = 0;
    //private BossAttackType currentAttackType = BossAttackType.EnergyBalls;
    private bool aggresive = false;
    // Varaibles para las bolas de energía
    private float currentEnergyBallPreparation = 0;
    private float currentEnergyBallCooldown = 0;
    private bool loadedBall = false;
    // Varaibles para el mega laser
    private LazerState currentLazerState = LazerState.CoolDown;
    private float lazerTimer = 0;
    //
    private bool shieldActivationRequested = false;
    private float currentEnergyShieldPreparation = 0;
    private float currentEnergyShieldDuration = 0;
    //
    private int currentPhase = 0;
    //
    private float currentStunnedTime = 0;
    //
    private bool dead = false;

    //
    public float MovementSpeed { get { return bossPhases[currentPhase].behaviours[currentBehaviourIndex].movementSpeed; } }

    // Start is called before the first frame update
    void Start()
    {
        //
        rb = GetComponent<Rigidbody>();
        //
        SpawnSegments();
        //rb.velocity = Vector3.forward * movementSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        //
        if (!dead)
        {
            //
            float dt = Time.deltaTime;
            UpdateBehaviourConditions();
            UpdateBehaviour(dt);
            //
            if (currentStunnedTime > 0)
            {
                currentStunnedTime -= dt;
                //Debug.Log("Current stunned time: " + currentStunnedTime);
            }
            else
            {
                //
                if (aggresive)
                {
                    UpdateAttack(dt);
                }
            }
            //
            if (aggresive)
            {
                UpdateDefensiveShield(dt);
            }
        }        
    }

    void SpawnSegments()
    {
        //
        segmentControllers = new Boss1SegmentController[numberOfSegments];
        segmentControllers[0] = segmentPrefab.GetComponent<Boss1SegmentController>();
        Boss1SegmentController firstSegmentController = segmentPrefab.GetComponent<Boss1SegmentController>();
        firstSegmentController.InitializeSegment(this, null, conectionPoint);
        //firstSegmentController.boss1Controller = this;
        //
        for (int i = 1; i < numberOfSegments; i++)
        {
            //
            GameObject newSegment = Instantiate(segmentPrefab, transform.parent);
            newSegment.name = "Segment " + i;
            newSegment.transform.localPosition = (i + 1) * segmentPrefab.transform.localPosition;
            //
            Boss1SegmentController segmentController = newSegment.GetComponent<Boss1SegmentController>();
            segmentControllers[i] = segmentController;
            //
            segmentController.InitializeSegment(this, segmentControllers[i - 1], segmentControllers[i - 1].conectionPoint);
            //segmentController.boss1Controller = this;
            //segmentController.previousSegmentController = segmentControllers[i - 1];
        }
    }

    void UpdateBehaviourConditions()
    {
        //
        switch (bossPhases[currentPhase].behaviours[currentBehaviourIndex].type)
        {
            case BossBehaviourType.GoingToPlayer:
                if (transform.position.y < 0)
                    currentBehaviourIndex = 1;
                break;
            case BossBehaviourType.GoingUp:
                if (transform.position.y > 200)
                    currentBehaviourIndex = 2;
                break;
        }
    }

    void UpdateBehaviour(float dt)
    {
        // Rotación - Solo cuando no está out
        //if (currentStunnedTime <= 0)
        if (true)
        {
            //
            //Debug.Log("Not sutnned, acting normal");
            //
            Vector3 playerDirection = EM_PlayerController.Instance.transform.position - transform.position;
            Vector3 playerCross = Vector3.Cross(playerDirection, Vector3.up);
            Vector3 newDirection = transform.forward;
            //
            switch (bossPhases[currentPhase].behaviours[currentBehaviourIndex].type)
            {
                case BossBehaviourType.GoingToPlayer:
                    newDirection = Vector3.RotateTowards(transform.forward, playerDirection,
                        bossPhases[currentPhase].behaviours[currentBehaviourIndex].rotationSpeed * dt * Mathf.Deg2Rad, 0f);
                    break;
                case BossBehaviourType.GoingUp:
                    newDirection = Vector3.RotateTowards(transform.forward, Vector3.up,
                        bossPhases[currentPhase].behaviours[currentBehaviourIndex].rotationSpeed * dt * Mathf.Deg2Rad, 0f);
                    break;
                case BossBehaviourType.EncirclingPlayer:
                    newDirection = Vector3.RotateTowards(transform.forward, playerCross,
                        bossPhases[currentPhase].behaviours[currentBehaviourIndex].rotationSpeed * dt * Mathf.Deg2Rad, 0f);
                    break;
            }

            //
            transform.rotation = Quaternion.LookRotation(newDirection);
        }
       
        transform.Translate(Vector3.forward * Time.deltaTime * bossPhases[currentPhase].behaviours[currentBehaviourIndex].movementSpeed);
    }

    void UpdateAttack(float dt)
    {
        //
        switch (bossPhases[currentPhase].behaviours[currentBehaviourIndex].attackType)
        {
            case BossAttackType.EnergyBalls:
                //
                int ballsToUse = 1;
                switch (currentPhase)
                {
                    case 0: ballsToUse = 1; break;
                    case 1: ballsToUse = 2; break;
                    case 2: ballsToUse = 4; break;
                }
                //
                if (loadedBall)
                {
                    currentEnergyBallPreparation += dt;
                    if (currentEnergyBallPreparation >= energyBallPreparation)
                    {
                        //Debug.Log("Launching balls");
                        // Lanzamos la bola en los segmentos
                        for (int i = 0; i < segmentControllers.Length; i++)
                        {
                            segmentControllers[i].LaunchBalls(ballsToUse);
                            currentEnergyBallPreparation = 0;
                            loadedBall = false;
                        }
                        //
                        GoToNextBehaviour();
                    }
                }
                else
                {
                    currentEnergyBallCooldown += dt;
                    if (currentEnergyBallCooldown >= energyBallCooldown)
                    {
                        //Debug.Log("Preparing balls");
                        // Preparamos la bola en los segmentos
                        for (int i = 0; i < segmentControllers.Length; i++)
                        {
                            segmentControllers[i].PrepareBalls(ballsToUse);
                            currentEnergyBallCooldown = 0;
                            loadedBall = true;
                        }
                    }
                }
                break;
            case BossAttackType.MegaLazer:
                //
                lazerTimer += dt;
                //
                switch (currentLazerState)
                {
                    case LazerState.CoolDown:
                        if (lazerTimer > 5) SwitchLazerState();
                        break;
                    case LazerState.Loading:
                        if (lazerTimer > 5) SwitchLazerState();
                        break;
                    case LazerState.Shooting:
                        if (lazerTimer > 5) { SwitchLazerState(); GoToNextBehaviour(); }
                        break;
                }                
                break;
        }        
    }

    void UpdateDefensiveShield(float dt)
    {
        if (shieldActivationRequested)
        {
            if(currentEnergyShieldPreparation < energyShieldPreparation)
            {
                currentEnergyShieldPreparation += dt;
                forceField.UpdateSize(currentEnergyShieldPreparation / energyBallPreparation);
                for (int i = 0; i < segmentControllers.Length; i++)
                    segmentControllers[i].forceField.UpdateSize(currentEnergyShieldPreparation / energyBallPreparation);
                // TODO: Ampliamos escudo
                if (currentEnergyShieldPreparation >= energyShieldPreparation)
                {
                    // Audio de escudo activado
                    forceField.Activate(damagedMaterial);
                    for (int i = 0; i < segmentControllers.Length; i++)
                        segmentControllers[i].forceField.Activate(damagedMaterial);
                }
            }
            else
            {
                currentEnergyShieldDuration += dt;
                if (currentEnergyShieldDuration >= energyShieldDuration)
                {
                    shieldActivationRequested = false;
                    forceField.Deactivate();
                    for (int i = 0; i < segmentControllers.Length; i++)
                        segmentControllers[i].forceField.Deactivate();
                }
            }
        }
    }

    public void OnDamagedSegment()
    {
        //
        if (!aggresive)
        {
            aggresive = true;
            AudioManager.Instance.PlayMusic(combatMusic, 0.3f, true);
            GoToNextBehaviour();
        }
        //
        //currentBehaviourIndex = 2;
    }

    public void AskShieldActivation()
    {
        if (aggresive && !shieldActivationRequested)
        {
            shieldActivationRequested = true;
            currentEnergyShieldPreparation = 0;
            currentEnergyShieldDuration = 0;
            // Audio de escudo arrancando
            forceField.StartCycle(healthyMaterial);
            for (int i = 0; i < segmentControllers.Length; i++)
                segmentControllers[i].forceField.StartCycle(healthyMaterial);
        }
    }

    void SwitchLazerState()
    {
        //
        lazerTimer = 0;
        //
        switch (currentLazerState)
        {
            case LazerState.CoolDown:
                currentLazerState = LazerState.Loading;
                smallLazer.SetActive(true);
                AudioManager.Instance.Play2dFx(smallLazer.transform.position, loadingLazerClip, 0.5f);
                break;
            case LazerState.Loading:
                currentLazerState = LazerState.Shooting;
                smallLazer.SetActive(false);
                bigLazer.SetActive(true);
                AudioManager.Instance.Play2dFx(smallLazer.transform.position, shootingLazerClip, 1f);
                break;
            case LazerState.Shooting:
                currentLazerState = LazerState.CoolDown;
                bigLazer.SetActive(false);
                break;
        }
    }

    void GoToNextBehaviour()
    {
        currentBehaviourIndex++;
        if (currentBehaviourIndex >= bossPhases[currentPhase].behaviours.Length)
            currentBehaviourIndex = 0;
    }

    public void EndPhase()
    {
        //
        GameObject endPhaseExplosion = Instantiate(endPhaseExplosionPrefab, transform.position, Quaternion.identity);
        //
        if (currentPhase >= bossPhases.Length - 1)
        {
            // Muerte
            Die();
        }
        else
        {
            // Siguiente fase
            currentPhase++;
            currentBehaviourIndex = 0;
            ResetSegments();
            StartCoroutine(WaitAndActivateFloatingBodies());
            //AudioManager.Instance.Play2dFx(endPhaseExplosion.transform.position, endPhaseExplosion)
        }
    }

    public void SufferDamage(float stunnedTimeToAdd)
    {
        //
        //if (AllSegmentsDamaged())
        //{

        //}

        // TODO: Lo aturdimos
        // TODO: Ruido seco para dar pista al jugador
        currentStunnedTime += stunnedTimeToAdd;
        currentStunnedTime = Mathf.Min(currentStunnedTime, 5f);
        //Debug.Log("Adding " + stunnedTimeToAdd + " stunned time - Current stunned time: " + currentStunnedTime);

        AudioManager.Instance.Play3dFx(transform.position, headHitEffect, 1);
    }

    bool AllSegmentsDamaged()
    {
        //
        for(int i = 0; i < segmentControllers.Length; i++)
        {
            if (!segmentControllers[i].Damaged)
                return false;
        }
        //
        return true;
    }

    void ResetSegments()
    {
        for(int i = 0; i < segmentControllers.Length; i++)
        {
            segmentControllers[i].Restore();
        }
    }

    IEnumerator WaitAndActivateFloatingBodies()
    {
        yield return new WaitForSeconds(3);
        FloatingBody[] floatingBodies = FindObjectsOfType<FloatingBody>();
        for(int i = 0; i < floatingBodies.Length; i++)
        {
            floatingBodies[i].Activate();
        }
    }

    void Die()
    {
        for (int i = 0; i < segmentControllers.Length; i++)
        {
            segmentControllers[i].Die();
        }
        //
        rb.isKinematic = false;
        rb.velocity = transform.forward * bossPhases[currentPhase].behaviours[currentBehaviourIndex].movementSpeed;
        rb.useGravity = true;
        //
        dead = true;
        //
        LevelManager.Instance.EndLevel(true);
    }
}

//[Serializable]
//public class BossBehaviour
//{
//    public BossBehaviourType type;
//    public Boss1Controller.Boss1AttackType attackType;
//    public float movementSpeed = 10;
//    public float rotationSpeed = 10;
//}