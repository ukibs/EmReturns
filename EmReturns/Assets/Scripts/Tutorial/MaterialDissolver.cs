using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialDissolver : MonoBehaviour
{
    private MeshRenderer meshRenderer;
    private float dissolutionStatus = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnEnable()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        StartDissolution(1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void StartDissolution(int direction)
    {
        //if(
        //    (direction == 1 && dissolutionStatus == 0) 
        //)
        //{
            StartCoroutine(StartDissolutionCoroutine(direction));
        //}
    }

    public void StartTransition(int direction)
    {
        StartCoroutine(StartTransitionCoroutine(direction));
    }

    IEnumerator StartDissolutionCoroutine(int direction)
    {
        float initialValue = direction == 1 ? 0 : 1;
        float finalValue = direction == 1 ? 1 : 0;
        int steps = 100;
        float duration = 2f;
        for(int i = 0; i < steps; i++)
        {
            
            MaterialPropertyBlock mpb = new MaterialPropertyBlock();
            meshRenderer.GetPropertyBlock(mpb);
            float dissolutionValue = (Mathf.Lerp(initialValue, finalValue, (float)i / (float)steps));
            //Debug.Log("Step: " + i + ", value: " + dissolutionValue);
            mpb.SetFloat("_Dissolution", dissolutionValue);
            meshRenderer.SetPropertyBlock(mpb);
            yield return new WaitForSeconds(duration / steps);
        }
        dissolutionStatus = finalValue;
    }

    IEnumerator StartTransitionCoroutine(int direction)
    {
        float initialValue = direction == 1 ? 0 : 1;
        float finalValue = direction == 1 ? 1 : 0;
        int steps = 100;
        float duration = 2f;
        for (int i = 0; i < steps; i++)
        {

            MaterialPropertyBlock mpb = new MaterialPropertyBlock();
            meshRenderer.GetPropertyBlock(mpb);
            float dissolutionValue = (Mathf.Lerp(initialValue, finalValue, (float)i / (float)steps));
            //Debug.Log("Step: " + i + ", value: " + dissolutionValue);
            mpb.SetFloat("_Transition", dissolutionValue);
            meshRenderer.SetPropertyBlock(mpb);
            yield return new WaitForSeconds(duration / steps);
        }
        //dissolutionStatus = finalValue;
    }
}
