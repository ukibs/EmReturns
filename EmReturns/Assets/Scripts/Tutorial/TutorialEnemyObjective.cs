using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialEnemyObjective : MonoBehaviour
{
    private EnemyHealth enemyHealth;
    private bool objectiveCounted = false;

    // Start is called before the first frame update
    void Start()
    {
        enemyHealth = GetComponent<EnemyHealth>();
    }

    // Update is called once per frame
    void Update()
    {
        if (enemyHealth.isDead && !objectiveCounted)
        {
            //Explode();
            TutorialManager.Instance.CheckAndNextPhase();
            objectiveCounted = true;
        }
    }
}
