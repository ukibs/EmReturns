using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialObjective : MonoBehaviour
{
    private MaterialDissolver materialDissolver;
    private bool activated = false;

    // Start is called before the first frame update
    void Start()
    {
        materialDissolver = GetComponent<MaterialDissolver>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("On trigger enter");
        if (!activated)
        {
            TutorialManager.Instance.CheckAndNextPhase();
            materialDissolver.StartDissolution(-1);
            activated = true;
        }        
    }
}
