using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialDestructibleObjective : MonoBehaviour
{
    private DestructibleObject destructibleObject;
    private bool objectiveCounted = false;

    // Start is called before the first frame update
    void Start()
    {
        destructibleObject = GetComponent<DestructibleObject>();
    }

    // Update is called once per frame
    void Update()
    {
        if(destructibleObject.isDestroyed && !objectiveCounted)
        {
            TutorialManager.Instance.CheckAndNextPhase();
            objectiveCounted = true;
        }
    }
}
