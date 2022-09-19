using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class EM_Repulsor : MonoBehaviour
{
    //
    [Header("Parameters")]
    public float idealDistanceToFloor = 3;
    public float jumpForce = 50;
    [Header("Debug")]
    public TMP_Text forceDownText;
    public TMP_Text forceUpText;

    //
    private Rigidbody rb;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //
        if (!EM_PlayerController.Instance.Ragdolled)
        {
            //
            bool makesContact = CheckFloor();
            //
            //var gamepad = Gamepad.current;
            //if (gamepad == null)
            //    return; // No gamepad connected.
                        //
            if (makesContact && InputController.Instance.JumpPressed)
            {
                rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
            }
        }        
    }

    //
    bool CheckFloor()
    {
        //
        RaycastHit hitInfo;
        //
        if(Physics.SphereCast(transform.position, 0.5f, Vector3.down, out hitInfo))
        {
            
            //
            float distanceToFloor = (hitInfo.point - transform.position).magnitude;
            //
            if(distanceToFloor < idealDistanceToFloor)
            {
                //
                float offsetCompensation = 1 + Mathf.Pow(1 - (distanceToFloor / idealDistanceToFloor), 3);
                //
                if (offsetCompensation < 1.1f) offsetCompensation = 0;
                //offsetCompensation = 0;
                //
                float fallingSpeed = Mathf.Min(rb.velocity.y, 0);
                //
                float speedCompensation = Mathf.Min(-fallingSpeed, 5);
                //
                //Debug.Log(hitInfo.transform.name + " - " + distanceToFloor);
                //
                //rb.AddForce(Vector3.up * rb.mass * 1 * 50 /*(idealDistanceToFloor/distanceToFloor)*/, ForceMode.Force);
                //
                Vector3 forceToApply = transform.up * (offsetCompensation + speedCompensation) * rb.mass;
                rb.AddForce(forceToApply, ForceMode.Impulse);
                //                
                forceUpText.text = forceToApply.y + "";
            }
            else
            {
                forceUpText.text = "0";
            }
            //
            forceDownText.text = (rb.velocity * rb.mass).y + "";
            //
            return true;
        }
        //
        return false;
    }
}
