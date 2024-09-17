using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RapidFireBulletController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, 15);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        Boss1SegmentController segmentController = collision.collider.GetComponentInParent<Boss1SegmentController>();
        if(segmentController != null)
        {
            segmentController.SufferDamage(1);
            Destroy(gameObject);
        }
    }
}
