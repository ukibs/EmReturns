using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    //
    public Transform xPivot;
    public Transform cameraFrame;
    public Vector2 rotationSpeed = new Vector2(360, 90);
    //
    [HideInInspector] public Transform objectiveInCenter;
    [HideInInspector] public Vector3 objectiveInCenterPoint;
    //
    private EM_PlayerController playerController;
    private Camera properCamera;
    //
    //private int cameraDirection = -1;

    public Quaternion ProperCameraRotation { get { return properCamera.transform.rotation; } }

    // Start is called before the first frame update
    void Start()
    {
        playerController = FindObjectOfType<EM_PlayerController>();
        properCamera = GetComponentInChildren<Camera>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        float dt = Time.deltaTime;

        //var gamepad = Gamepad.current;
        //if (gamepad == null)
        //    return; // No gamepad connected.

        //
        if (playerController.currentObjective)
        {
            transform.LookAt(playerController.currentObjective);
            //properCamera.transform.LookAt(playerController.currentObjective);
            cameraFrame.LookAt(playerController.currentObjective);
        }
        else
        {
            UpdateCameraMovement(dt);
            // TODO: Mirar a ver si jugamos también con la y en algun caso
            //properCamera.transform.localEulerAngles = new Vector3(properCamera.transform.localEulerAngles.x, 0, 0);
            cameraFrame.localEulerAngles = Vector3.zero;
        }        

        //
        transform.position = playerController.transform.position;

        //
        UpdateCentralPoint();

        //
        //if (gamepad.leftStickButton.wasPressedThisFrame)
        //{
        //    cameraDirection *= -1;
        //}
    }

    //
    void UpdateCameraMovement(float dt)
    {
        // Movimiento
        //Vector2 move = gamepad.rightStick.ReadValue();
        Vector2 move = InputController.Instance.CameraAxis;
        //move = new Vector2(Mathf.Pow(move.x, 2) * Mathf.Sign(move.x), Mathf.Pow(move.y, 2) * Mathf.Sign(move.y));
        //Debug.Log(move);
        //
        transform.Rotate(Vector3.up, move.x * rotationSpeed.x * dt);
        //
        //xPivot.Rotate(Vector3.right, move.y * rotationSpeed.y * dt);
        transform.Rotate(Vector3.right, move.y * InputController.Instance.CameraDirection * rotationSpeed.y * dt);
        //
        transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 0);

        // Vamos a acotar la rotación en x
        // TODO: OJO ñapa
        float maxX = 85;
        float minX = -85;
        Quaternion currentRotation = transform.rotation;
        currentRotation.x = Mathf.Clamp(currentRotation.x, minX, maxX);

        transform.rotation = currentRotation;
    }

    public Transform GetNearestObjectiveToScreenCenter(bool hasHookedRb)
    {
        //
        Transform selectedObjective = null;
        //Vector2 screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);
        float nearestToCenter = Mathf.Infinity;
        //
        Objective[] possibleObjectives = FindObjectsOfType<Objective>();
        for (int i = 0; i < possibleObjectives.Length; i++)
        {
            //
            if (hasHookedRb && possibleObjectives[i].type == Objective.Type.Scenario)
                continue;
            //
            if (possibleObjectives[i] == EM_ShovelController.Instance.HookedObjective)
                continue;
            // Distancia al centro de pantalla
            Vector3 posInScreen = properCamera.WorldToViewportPoint(possibleObjectives[i].transform.position);
            float distanceToCenter = Mathf.Pow(posInScreen.x - 0.5f, 2) + Mathf.Pow(posInScreen.y - 0.5f, 2);
            // TODO: Peso extra según la etuiqueta
            bool inScreen = posInScreen.x >= 0 && posInScreen.x <= 1 &&
                posInScreen.y >= 0 && posInScreen.y <= 1 &&
                posInScreen.z > 0;
            //
            if (inScreen && distanceToCenter < nearestToCenter)
            {
                nearestToCenter = distanceToCenter;
                selectedObjective = possibleObjectives[i].transform;
            }
        }

        //
        return selectedObjective;
    }

    public Transform ChangeObjective(Vector2 direction)
    {
        //
        Transform selectedObjective = null;
        //Vector2 screenCenter = new Vector2(Screen.width / 2, Screen.height / 2);
        float nearestAngle = Mathf.Infinity; //180f;
        //
        float stickAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        //
        Objective[] possibleObjectives = FindObjectsOfType<Objective>();
        //
        Objective currentObjective = playerController.currentObjective.GetComponent<Objective>();
        //
        for (int i = 0; i < possibleObjectives.Length; i++)
        {
            //
            if (possibleObjectives[i] == playerController.currentObjective
                || possibleObjectives[i] == EM_ShovelController.Instance.HookedObjective
                || possibleObjectives[i].type != currentObjective.type  // Que sea del mismo tipo
                )
                continue;
            // De momento puro angulo
            Vector3 posInScreen = properCamera.WorldToViewportPoint(possibleObjectives[i].transform.position);
            Vector2 coordinatesFromScreenCenter = new Vector2(posInScreen.x - 0.5f, posInScreen.y - 0.5f);
            float objectiveAngle = Mathf.Atan2(coordinatesFromScreenCenter.y, coordinatesFromScreenCenter.x) * Mathf.Rad2Deg;
            float angleOffset = Mathf.Abs(stickAngle - objectiveAngle);     // Max 180
            float distanceToCenter = Mathf.Pow(posInScreen.x - 0.5f, 2) + Mathf.Pow(posInScreen.y - 0.5f, 2);   // Max 1 con algo
            float distanceToCenterScaled = distanceToCenter * 10;
            bool inScreen = posInScreen.x >= 0 && posInScreen.x <= 1 &&
                posInScreen.y >= 0 && posInScreen.y <= 1 &&
                posInScreen.z > 0;
            // TODO: Hacer un mix de distancia y angulo para valorar
            if (inScreen && angleOffset + distanceToCenterScaled < nearestAngle)
            {
                nearestAngle = angleOffset + distanceToCenterScaled;
                selectedObjective = possibleObjectives[i].transform;
            }
        }

        //
        return selectedObjective;               
    }

    //
    void UpdateCentralPoint()
    {
        //
        RaycastHit hitInfo;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hitInfo))
        {
            //
            objectiveInCenter = hitInfo.transform;
            objectiveInCenterPoint = hitInfo.point;
        }
        else
        {
            objectiveInCenter = null;
        }
    }
}
