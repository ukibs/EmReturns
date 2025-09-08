using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EM_MaterialController : MonoBehaviour
{
    public FlexibleColorPicker bodyFCP;
    public FlexibleColorPicker bodyEmissionFCP;
    //public FlexibleColorPicker faceFCP;
    public FlexibleColorPicker faceEmissionFCP;

    public MeshRenderer[] bodyMeshRenderers;
    public MeshRenderer[] faceMeshRenderers;

    public Material bodyMaterial;
    public Material faceMaterial;

    public Animator emAnimator;

    private float intensity = 5f;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < bodyMeshRenderers.Length; i++)
        {
            bodyFCP.color = bodyMeshRenderers[i].material.color;
        }

        for (int i = 0; i < bodyMeshRenderers.Length; i++)
        {
            float intensity = 5f;
            bodyEmissionFCP.color = bodyMeshRenderers[i].material.GetColor("_EmissionColor") / intensity;
        }

        //for (int i = 0; i < faceMeshRenderers.Length; i++)
        //{
        //    faceFCP.color = faceMeshRenderers[i].material.color;
        //}

        for (int i = 0; i < faceMeshRenderers.Length; i++)
        {
            faceEmissionFCP.color = faceMeshRenderers[i].material.GetColor("_EmissionColor");
        }
    }

    // Update is called once per frame
    void Update()
    {
        for(int i = 0; i < bodyMeshRenderers.Length; i++)
        {
            bodyMeshRenderers[i].material.color = bodyFCP.color;
        }

        for (int i = 0; i < bodyMeshRenderers.Length; i++)
        {
            // float intensity = 5f;
            //bodyMeshRenderers[i].material.EnableKeyword("_EMISSION");
            bodyMeshRenderers[i].material.SetColor("_EmissionColor", bodyEmissionFCP.color * intensity);
            //DynamicGI.SetEmissive(bodyMeshRenderers[i], bodyEmissionFCP.color);
        }

        for (int i = 0; i < faceMeshRenderers.Length; i++)
        {
            // faceMeshRenderers[i].material.color = faceFCP.color;
            faceMeshRenderers[i].material.color = faceEmissionFCP.color;
        }

        for (int i = 0; i < faceMeshRenderers.Length; i++)
        {
            faceMeshRenderers[i].material.SetColor("_EmissionColor", faceEmissionFCP.color);
        }

        //RenderSettings.skybox.SetColor("_SunDiscColor", bodyFCP.color);
    }


    public void SaveAndReturn()
    {
        bodyMaterial.color = bodyFCP.color;
        bodyMaterial.SetColor("_EmissionColor", bodyEmissionFCP.color * intensity);
        //faceMaterial.color = faceFCP.color;
        faceMaterial.SetColor("_EmissionColor", faceEmissionFCP.color);
        //SceneManager.LoadScene(1);
        StartCoroutine(WaitAndReturn());
    }

    public void CancelAndReturn()
    {
        //SceneManager.LoadScene(1);
        StartCoroutine(WaitAndReturn());
    }

    IEnumerator WaitAndReturn()
    {
        emAnimator.SetTrigger("Color Saved");
        yield return new WaitForSeconds(1);
        SceneManager.LoadScene(1);
    }

    public void ColorChanged()
    {
        emAnimator.SetTrigger("Color Changed");
    }
}
