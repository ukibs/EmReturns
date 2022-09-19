using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExpandedCollider : MonoBehaviour
{
    private Rigidbody parentRb;
    private CapsuleCollider capsuleCollider;

    // Start is called before the first frame update
    void Start()
    {
        parentRb = GetComponentInParent<Rigidbody>();
        capsuleCollider = GetComponentInChildren<CapsuleCollider>();
    }

    // Update is called once per frame
    void Update()
    {
        float dt = Time.deltaTime;
        transform.forward = parentRb.velocity.normalized;
        capsuleCollider.height = parentRb.velocity.magnitude * dt;
    }
}
