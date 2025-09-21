using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputController : MonoBehaviour
{
    //

    //
    private static InputController instance;
    private Gamepad gamepad;
    private Keyboard keyboard;
    private Mouse mouse;
    private float cameraDirection = -1;

    //
    public static InputController Instance { get { return instance; } }

    //
    public float CameraDirection { get { return cameraDirection; } }

    // Axis Properties -------------------------------------------------------------------

    //
    public Vector2 MovementAxis
    {
        get
        {
            Vector2 axisValue = Vector2.zero;
            if (gamepad != null)
            {
                //Debug.Log(gamepad.leftStick.ReadValue());
                axisValue += gamepad.leftStick.ReadValue();
            }
            if(keyboard != null)
            {

                //Debug.Log("Getting keyboard");

                float horizontalAxis = 0;
                if (keyboard.aKey.isPressed) horizontalAxis = -1;
                if (keyboard.dKey.isPressed) horizontalAxis = 1;

                float verticalAxis = 0;
                if (keyboard.wKey.isPressed) verticalAxis = 1;
                if (keyboard.sKey.isPressed) verticalAxis = -1;

                //Debug.Log(horizontalAxis + " - " + verticalAxis);

                axisValue += new Vector2(horizontalAxis, verticalAxis);
            }
            return axisValue;
        }
    }

    //
    public Vector2 CameraAxis
    {
        get
        {
            Vector2 axisValue = Vector2.zero;
            if (gamepad != null)
            {
                Vector2 move = gamepad.rightStick.ReadValue();
                move = new Vector2(Mathf.Pow(move.x, 2) * Mathf.Sign(move.x), Mathf.Pow(move.y, 2) * Mathf.Sign(move.y));
                axisValue += move;
            }
            if(mouse != null) { 
                axisValue += mouse.delta.ReadValue() * 0.1f;
            }
            return axisValue;
        }
    }

    // Button properties ----------------------------------------------------------------------------------------

    // Pulse Shot
    public bool PulseShotPressed
    {
        get
        {
            return (gamepad!= null && gamepad.rightTrigger.wasPressedThisFrame) ||
                (mouse != null && mouse.leftButton.wasPressedThisFrame);
        }
    }

    public bool PulseShotReleased
    {
        get
        {
            return (gamepad != null && gamepad.rightTrigger.wasReleasedThisFrame) ||
                (mouse != null && mouse.leftButton.wasReleasedThisFrame);
        }
    }

    // Charge Forward
    public bool ChargeForwardPressed
    {
        get
        {
            return (gamepad != null && gamepad.leftShoulder.wasPressedThisFrame) ||
                (mouse != null && keyboard.qKey.wasPressedThisFrame);
        }
    }

    public bool ChargeForwardReleased
    {
        get
        {
            return (gamepad != null && gamepad.leftShoulder.wasReleasedThisFrame) ||
                (mouse != null && keyboard.qKey.wasReleasedThisFrame);
        }
    }

    // Rapid fire
    public bool RapidFirePressed
    {
        get
        {
            return (gamepad != null && gamepad.leftTrigger.wasPressedThisFrame) ||
                (mouse != null && keyboard.eKey.wasPressedThisFrame);
        }
    }

    public bool RapidFireReleased
    {
        get
        {
            return (gamepad != null && gamepad.leftTrigger.wasReleasedThisFrame) ||
                (mouse != null && keyboard.eKey.wasReleasedThisFrame);
        }
    }

    // Grapple
    public bool GrapplePressed
    {
        get
        {
            return (gamepad != null && gamepad.rightShoulder.wasPressedThisFrame) ||
                (mouse != null && mouse.rightButton.wasPressedThisFrame);
        }
    }

    public bool GrappleReleased
    {
        get
        {
            return (gamepad != null && gamepad.rightShoulder.wasReleasedThisFrame) ||
                (mouse != null && mouse.rightButton.wasReleasedThisFrame);
        }
    }

    // Jump
    public bool JumpPressed
    {
        get
        {
            return (gamepad != null && gamepad.aButton.wasPressedThisFrame) ||
                (mouse != null && keyboard.spaceKey.wasPressedThisFrame);
        }
    }

    public bool JumpReleased
    {
        get
        {
            return (gamepad != null && gamepad.aButton.wasReleasedThisFrame) ||
                (mouse != null && keyboard.spaceKey.wasReleasedThisFrame);
        }
    }

    // Down
    public bool DownPressed
    {
        get
        {
            return (gamepad != null && gamepad.yButton.wasPressedThisFrame) ||
                (mouse != null && keyboard.leftCtrlKey.wasPressedThisFrame);
        }
    }

    public bool DownReleased
    {
        get
        {
            return (gamepad != null && gamepad.yButton.wasReleasedThisFrame) ||
                (mouse != null && keyboard.leftCtrlKey.wasReleasedThisFrame);
        }
    }

    // Grapple
    public bool SprintPressed
    {
        get
        {
            return (gamepad != null && gamepad.bButton.wasPressedThisFrame) ||
                (mouse != null && keyboard.leftShiftKey.wasPressedThisFrame);
        }
    }

    public bool SprintReleased
    {
        get
        {
            return (gamepad != null && gamepad.bButton.wasReleasedThisFrame) ||
                (mouse != null && keyboard.leftShiftKey.wasReleasedThisFrame);
        }
    }

    // Objective lock
    public bool ObjectiveLockPressed
    {
        get
        {
            return (gamepad != null && gamepad.rightStickButton.wasPressedThisFrame) ||
                (mouse != null && keyboard.tabKey.wasPressedThisFrame);
        }
    }

    // Pause
    public bool PausePressed
    {
        get
        {
            return (gamepad != null && gamepad.startButton.wasPressedThisFrame) ||
                (mouse != null && keyboard.escapeKey.wasPressedThisFrame);
        }
    }

    // Exit
    public bool ExitPressed
    {
        get
        {
            return (gamepad != null && gamepad.selectButton.wasPressedThisFrame) ||
                (mouse != null && keyboard.enterKey.wasPressedThisFrame);
        }
    }

    // Methods -------------------------------------------------------------------

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;   
    }

    // Update is called once per frame
    void Update()
    {
        gamepad = Gamepad.current;
        //Debug.Log("Gamepad: " + gamepad);
        keyboard = Keyboard.current;
        mouse = Mouse.current;
        // Arreglo para dirección de cámara
        if (
            (gamepad != null && gamepad.leftStickButton.wasPressedThisFrame) ||
            (keyboard != null && keyboard.digit1Key.wasPressedThisFrame)
        ){
            cameraDirection *= -1;
        }
    }
}
