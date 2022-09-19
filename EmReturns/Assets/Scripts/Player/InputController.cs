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
            //Debug.Log(gamepad);
            if (gamepad != null)
            {
                //Debug.Log(gamepad.leftStick.ReadValue());
                return gamepad.leftStick.ReadValue();
            }
            else if(keyboard != null)
            {

                //Debug.Log("Getting keyboard");

                float horizontalAxis = 0;
                if (keyboard.aKey.isPressed) horizontalAxis = -1;
                if (keyboard.dKey.isPressed) horizontalAxis = 1;

                float verticalAxis = 0;
                if (keyboard.wKey.isPressed) verticalAxis = 1;
                if (keyboard.sKey.isPressed) verticalAxis = -1;

                //Debug.Log(horizontalAxis + " - " + verticalAxis);

                return new Vector2(horizontalAxis, verticalAxis);
            }
            else
            {
                return Vector2.zero;
            }
        }
    }

    //
    public Vector2 CameraAxis
    {
        get
        {
            if (gamepad != null)
            {
                Vector2 move = gamepad.rightStick.ReadValue();
                move = new Vector2(Mathf.Pow(move.x, 2) * Mathf.Sign(move.x), Mathf.Pow(move.y, 2) * Mathf.Sign(move.y));
                return move;
            }
            else if(mouse != null) { 
                return mouse.delta.ReadValue() * 0.1f;
            }
            else
            {
                return Vector2.zero;
            }
        }
    }

    // Button properties ----------------------------------------------------------------------------------------

    // Pulse Shot
    public bool PulseShotPressed
    {
        get
        {
            if(gamepad != null)
            {
                return gamepad.rightTrigger.wasPressedThisFrame;
            }
            else if(mouse != null){
                return mouse.leftButton.wasPressedThisFrame;
            }
            else
            {
                return false;
            }
        }
    }

    public bool PulseShotReleased
    {
        get
        {
            if (gamepad != null)
            {
                return gamepad.rightTrigger.wasReleasedThisFrame;
            }
            else if (mouse != null)
            {
                return mouse.leftButton.wasReleasedThisFrame;
            }
            else
            {
                return false;
            }
        }
    }

    // Grapple
    public bool GrapplePressed
    {
        get
        {
            if (gamepad != null)
            {
                return gamepad.rightShoulder.wasPressedThisFrame;
            }
            else if (mouse != null)
            {
                return mouse.rightButton.wasPressedThisFrame;
            }
            else
            {
                return false;
            }
        }
    }

    public bool GrappleReleased
    {
        get
        {
            if (gamepad != null)
            {
                return gamepad.rightShoulder.wasReleasedThisFrame;
            }
            else if (mouse != null)
            {
                return mouse.rightButton.wasReleasedThisFrame;
            }
            else
            {
                return false;
            }
        }
    }

    // Jump
    public bool JumpPressed
    {
        get
        {
            if (gamepad != null)
            {
                return gamepad.aButton.wasPressedThisFrame;
            }
            else if (keyboard != null)
            {
                return keyboard.spaceKey.wasPressedThisFrame;
            }
            else
            {
                return false;
            }
        }
    }

    public bool JumpReleased
    {
        get
        {
            if (gamepad != null)
            {
                return gamepad.aButton.wasReleasedThisFrame;
            }
            else if (keyboard != null)
            {
                return keyboard.spaceKey.wasReleasedThisFrame;
            }
            else
            {
                return false;
            }
        }
    }

    // Grapple
    public bool SprintPressed
    {
        get
        {
            if (gamepad != null)
            {
                return gamepad.bButton.wasPressedThisFrame;
            }
            else if (keyboard != null)
            {
                return keyboard.leftShiftKey.wasPressedThisFrame;
            }
            else
            {
                return false;
            }
        }
    }

    public bool SprintReleased
    {
        get
        {
            if (gamepad != null)
            {
                return gamepad.bButton.wasReleasedThisFrame;
            }
            else if (keyboard != null)
            {
                return keyboard.leftShiftKey.wasReleasedThisFrame;
            }
            else
            {
                return false;
            }
        }
    }

    // Objective lock
    public bool ObjectiveLockPressed
    {
        get
        {
            if (gamepad != null)
            {
                return gamepad.rightStickButton.wasPressedThisFrame;
            }
            else if (keyboard != null)
            {
                return keyboard.tabKey.wasPressedThisFrame;
            }
            else
            {
                return false;
            }
        }
    }

    // Pause
    public bool PausePressed
    {
        get
        {
            if (gamepad != null)
            {
                return gamepad.startButton.wasPressedThisFrame;
            }
            else if (keyboard != null)
            {
                return keyboard.escapeKey.wasPressedThisFrame;
            }
            else
            {
                return false;
            }
        }
    }

    // Exit
    public bool ExitPressed
    {
        get
        {
            if (gamepad != null)
            {
                return gamepad.selectButton.wasPressedThisFrame;
            }
            else if (keyboard != null)
            {
                return keyboard.enterKey.wasPressedThisFrame;
            }
            else
            {
                return false;
            }
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
        keyboard = Keyboard.current;
        mouse = Mouse.current;
        //
        if (gamepad != null && gamepad.leftStickButton.wasPressedThisFrame)
        {
            cameraDirection *= -1;
        }
        //
        if (keyboard != null && keyboard.digit1Key.wasPressedThisFrame)
        {
            cameraDirection *= -1;
        }
    }
}
