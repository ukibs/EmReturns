using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [Header("Parameters")]
    public int maxHealth = 5;

    [Header("Materials")]
    public MeshRenderer[] shaderMeshRenderers;

    [Header("Feedback")]
    public AudioClip onDamageClip;
    public AudioClip onDeathClip;

    private int currentHealth = 0;
    [HideInInspector] public bool isDead = false;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SufferDamage(int damage)
    {
        //
        currentHealth -= damage;
        if (currentHealth <= 0)
        {
            //
            AudioManager.Instance.Play3dFx(transform.position, onDeathClip, 0.3f);
            
            // Change player objective if this was the locked one
            if (EM_PlayerController.Instance.currentObjective == this.transform)
                EM_PlayerController.Instance.currentObjective = CameraController.Instance.ChangeBossSegmentObjective(Vector2.right);

            //
            isDead = true;
            
        }
        else
        {
            // 
            AudioManager.Instance.Play2dFx(transform.position, onDamageClip, 0.8f);

            //
            for (int i = 0; i < shaderMeshRenderers.Length; i++)
            {
                MaterialPropertyBlock mpb = new MaterialPropertyBlock();
                shaderMeshRenderers[i].GetPropertyBlock(mpb);
                mpb.SetFloat("_Dissolution", (Mathf.Lerp(1, 0.5f, (float)currentHealth / (float)maxHealth) ) );
                shaderMeshRenderers[i].SetPropertyBlock(mpb);

            }
        }
    }

}
