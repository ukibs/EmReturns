using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EM_UnderFloorRecoverer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.position.y < 0)
        {
            RaycastHit hitInfo;
            if(Physics.Raycast(transform.position + (Vector3.up * 100), Vector3.down, out hitInfo)){
                if(hitInfo.transform.gameObject.layer == 9)
                {
                    transform.position = hitInfo.point + Vector3.up;
                }
            }
        }
    }
}
