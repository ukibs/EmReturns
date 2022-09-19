using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneralFunctions
{
    public static float EstimateTimeBetweenTwoPoints(Vector3 point1, Vector3 point2, float speed)
    {
        float estimatedTime = 0;
        estimatedTime = (point1 - point2).magnitude / speed;
        return estimatedTime;
    }

    public static Vector3 EstimateFuturePosition(Vector3 originPoint, Vector3 velocity, float time)
    {
        Vector3 estimatedPosition = Vector3.zero;
        estimatedPosition = originPoint + (velocity * time);
        return estimatedPosition;
    }
}
