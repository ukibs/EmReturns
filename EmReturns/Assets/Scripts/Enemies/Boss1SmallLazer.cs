using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss1SmallLazer : MonoBehaviour
{
    //
    private LineRenderer lineRenderer;

    // Start is called before the first frame update
    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, transform.position + (transform.forward * 1000f));
    }
}
