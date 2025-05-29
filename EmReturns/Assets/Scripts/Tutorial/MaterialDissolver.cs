using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialDissolver : MonoBehaviour
{
    MeshRenderer meshRenderer;

    // Start is called before the first frame update
    void Start()
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
        StartCoroutine(StartDissolutionCoroutine(direction));
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
        
    }
}
