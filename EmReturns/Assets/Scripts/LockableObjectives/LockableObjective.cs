using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockableObjective : MonoBehaviour
{
    public enum Type
    {
        Enemy,
        Scenario
    }

    public Type type;
}
