using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss1SegmentController : MonoBehaviour
{
    //
    [Header("Components")]
    [HideInInspector] public Boss1Controller boss1Controller;
    [HideInInspector] public Boss1SegmentController previousSegmentController;
    [HideInInspector] public Transform segmentObjective;
    public Transform conectionPoint;
    public Transform[] launchPositions;
    public GameObject energyBallPrefab;
    public AudioClip shootClip;
    public Boss1ForceField forceField;
    public GameObject healthMarkerPrefab;
    //public Canvas canvas;
    public RectTransform healthMarkersPanel;
    [Header("Parameters")]
    public float launchForce = 300f;
    public float shootMaxOffset = 10f;
    public float minPlayerDistanceToActivateShields = 50f;
    public int maxHealth = 100;
    [Header("Materials")]
    public MeshRenderer[] shaderMeshRenderers;
    [Header("Feedback")]
    public AudioClip onDamageClip;
    public AudioClip onImpactClip;
    [Header("Extra")]
    public GameObject finisherChargerPrefab;

    //
    [HideInInspector] public float currentSpeed;

    //
    private Rigidbody rb;
    private Vector3 idealDistanceToObjective;
    private Transform connectionPointObjective;
    [HideInInspector]public bool damaged = false;
    private Boss1EnergyBall[] currentEnergyBalls;
    private bool dead = false;
    private int currentHealth;
    private HealthMarkerController healthMarkerController;

    //
    [HideInInspector] public bool Damaged { get { return damaged; } }

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        currentEnergyBalls = new Boss1EnergyBall[4];
        currentHealth = maxHealth;
        healthMarkerController = Instantiate(healthMarkerPrefab, healthMarkersPanel).GetComponent<HealthMarkerController>();
        healthMarkerController.SetHealthController(this);
        //healthMarkerController.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        //
        if (!dead)
        {
            //
            transform.LookAt(connectionPointObjective);
            //
            UpdateSpeed();
            //
            transform.Translate(Vector3.forward * Time.deltaTime * currentSpeed);
            //
            CheckPlayerDistanceForShields();
        }        
    }

    public void InitializeSegment(Boss1Controller boss1Controller, Boss1SegmentController boss1SegmentController,
        Transform connectionPointObjective)
    {
        //
        this.boss1Controller = boss1Controller;
        this.previousSegmentController = boss1SegmentController;
        this.connectionPointObjective = connectionPointObjective;
        //
        segmentObjective = (boss1SegmentController != null) ? boss1SegmentController.transform : boss1Controller.transform;
        //
        idealDistanceToObjective = segmentObjective.position - transform.position;
        //Debug.Log(gameObject.name + " - " + segmentObjective + " - " + idealDistanceToObjective.magnitude);
    }

    public void UpdateSpeed()
    {
        //
        float referenceSpeed = (previousSegmentController) ? previousSegmentController.currentSpeed : boss1Controller.MovementSpeed;
        //
        //currentSpeed =
        //    ((segmentObjective.transform.position - transform.position).sqrMagnitude < idealDistanceToObjective.sqrMagnitude) ?
        //    referenceSpeed : referenceSpeed * 0.9f;
        ////
        Vector3 currentDistance = segmentObjective.transform.position - transform.position;
        //
        if (currentDistance.sqrMagnitude < idealDistanceToObjective.sqrMagnitude)
            referenceSpeed *= 0.9f;
        //
        if (currentDistance.sqrMagnitude > idealDistanceToObjective.sqrMagnitude)
            referenceSpeed *= 1.1f;
        //
        currentSpeed = referenceSpeed;
    }

    void CheckPlayerDistanceForShields()
    {
        if((EM_PlayerController.Instance.transform.position - transform.position).sqrMagnitude 
            < Mathf.Pow(minPlayerDistanceToActivateShields, 2))
        {
            boss1Controller.AskShieldActivation();
        }
    }

    public void SufferDamage(int damage)
    {
        //
        //Debug.Log("Segment suffering damage - " + damage);
        //
        if (!damaged)
        {
            //
            currentHealth -= damage;
            if(currentHealth <= 0)
            {
                //
                for (int i = 0; i < shaderMeshRenderers.Length; i++)
                {
                    shaderMeshRenderers[i].material = boss1Controller.damagedMaterial;
                }
                AudioManager.Instance.Play3dFx(transform.position, onDamageClip, 0.3f);
                boss1Controller.OnDamagedSegment();
                damaged = true;
                //
                for (int i = 0; i < 10; i++)
                {
                    GameObject newFinisherCharger = Instantiate(finisherChargerPrefab, transform.position, Quaternion.identity);
                    newFinisherCharger.transform.eulerAngles = new Vector3(Random.Range(0, 360), Random.Range(0, 360), Random.Range(0, 360));
                }
                //
                if (healthMarkerController)
                {
                    //Destroy(healthMarkerController.gameObject);
                    //healthMarkerController = null;
                    healthMarkerController.gameObject.SetActive(false);
                }
            }
            else
            {
                // TODO: Poner otro audio
                AudioManager.Instance.Play2dFx(transform.position, onImpactClip, 0.8f);
                //if (!healthMarkerController)
                //{
                //    healthMarkerController = Instantiate(healthMarkerPrefab, canvas.transform).GetComponent<HealthMarkerController>();
                //    healthMarkerController.SetHealthController(this);                    
                //}
                healthMarkerController.gameObject.SetActive(true);
                healthMarkerController.UpdateValue(currentHealth, maxHealth);
            }

        }        
    }

    public void Restore()
    {
        for (int i = 0; i < shaderMeshRenderers.Length; i++)
        {
            shaderMeshRenderers[i].material = boss1Controller.healthyMaterial;
        }
        damaged = false;
        currentHealth = maxHealth;
    }

    #region Attack Balls Methods
    public void PrepareBalls(int ballsToUse)
    {
        for (int i = 0; i < ballsToUse; i++)
        {
            PrepareBall(i);
        }
    }

    public void PrepareBall(int ballIndex)
    {
        //Debug.Log(launchPositions[ballIndex]);
        GameObject newEnergyBall = Instantiate(energyBallPrefab, launchPositions[ballIndex]);
        //Debug.Log(currentEnergyBalls[ballIndex]);
        currentEnergyBalls[ballIndex] = newEnergyBall.GetComponent<Boss1EnergyBall>();
    }

    public void LaunchBalls(int ballsToUse)
    {
        for(int i = 0; i < ballsToUse; i++)
        {
            LaunchBall(i);
        }
    }

    public void LaunchBall(int ballIndex)
    {
        //Chequeo por si ocurre el cambio de fase habiendo bolas spameadas
        if (currentEnergyBalls[ballIndex] == null) return;
        //
        currentEnergyBalls[ballIndex].transform.parent = null;

        Rigidbody rb = currentEnergyBalls[ballIndex].GetComponent<Rigidbody>();
        //Vector3 objectivePosition = EM_PlayerController.Instance.transform.position;

        float shotSpeed = launchForce / rb.mass;

        // Primera pasada de estimación
        //float estimatedImpactTime = GeneralFunctions.EstimateTimeBetweenTwoPoints(objectivePosition, launchPositions[0].position, shotSpeed);
        //Vector3 estimatedObjectivePosition =
        //    GeneralFunctions.EstimateFuturePosition(objectivePosition, EM_PlayerController.Instance.Rb.velocity, estimatedImpactTime);

        // Segunda pasada de estimación
        //estimatedImpactTime = GeneralFunctions.EstimateTimeBetweenTwoPoints(estimatedObjectivePosition, launchPositions[0].position, shotSpeed);
        //estimatedObjectivePosition =
        //    GeneralFunctions.EstimateFuturePosition(estimatedObjectivePosition, EM_PlayerController.Instance.Rb.velocity, estimatedImpactTime);

        // Offset random para dispersar la rafaga
        //float smo = shootMaxOffset;
        //estimatedObjectivePosition += new Vector3(Random.Range(-smo, smo), Random.Range(-smo, smo), Random.Range(-smo, smo));
        //Vector3 direction = estimatedObjectivePosition - launchPositions[ballIndex].position;

        
        rb.AddForce(launchPositions[ballIndex].forward * launchForce, ForceMode.Impulse);

        Boss1EnergyBall energyBall = currentEnergyBalls[ballIndex].GetComponent<Boss1EnergyBall>();
        //energyBall.Activate(estimatedImpactTime);
        energyBall.Activate();

        AudioManager.Instance.Play3dFx(launchPositions[ballIndex].position, shootClip, 0.3f);

        currentEnergyBalls[ballIndex] = null;
    }

    #endregion

    #region Attack Lasers Methods

    public void PrepareLasers()
    {

    }

    public void ActivateLasers()
    {

    }

    #endregion

    public void Die()
    {
        //
        rb.isKinematic = false;
        rb.velocity = transform.forward * boss1Controller.MovementSpeed;
        rb.useGravity = true;
        //
        dead = true;
    }
}
