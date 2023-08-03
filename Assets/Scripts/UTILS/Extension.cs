using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extension
{
    public static Quaternion ToQuaternion(this Vector3 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        return Quaternion.AngleAxis(angle, Vector3.forward);
    }
}
