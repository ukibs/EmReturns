using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OffensiveShieldController : MonoBehaviour
{
    public Rigidbody playerRB; 
    // Start is called before the first frame update
    void Start()
    {
        playerRB = GetComponentInParent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //private void OnCollisionEnter(Collision collision)
    //{
    //    Debug.Log("Offensive shield controller collission: " + collision.gameObject.name);
    //    Boss1SegmentController segmentController = collision.collider.GetComponentInParent<Boss1SegmentController>();
    //    if (segmentController != null)
    //    {
    //        segmentController.SufferDamage((int)playerRB.velocity.magnitude);
    //        Destroy(gameObject);
    //    }
    //}
}
