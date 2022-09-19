using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateOnStart : MonoBehaviour
{
    //
    public float xMaxDeviation = 15f;
    public float yMaxDeviation = 0;
    public float zMaxDeviation = 15f;
    // Start is called before the first frame update
    void Start()
    {
        transform.localEulerAngles = new Vector3(Random.Range(-xMaxDeviation, xMaxDeviation),
                                                Random.Range(-yMaxDeviation, yMaxDeviation),
                                                Random.Range(-zMaxDeviation, zMaxDeviation));
    }

}
