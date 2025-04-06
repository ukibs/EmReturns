using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ObjectGroup", menuName = "ScriptableObjects/ObjectGroup", order = 1)]
public class ObjectGroupSO : ScriptableObject
{
    public ObjectGroup[] objectGroups;
}

[System.Serializable] 
public class ObjectGroup
{
    //
    public GameObject[] prefabs;
    public float objectDensity = 1;
    public int xObjects = 10;
    public int yObjects = 1;
    public int zObjects = 10;
    public float xDistance = 100;
    public float yDistance = 0;
    public float zDistance = 100;
}