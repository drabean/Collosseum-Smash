using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extension
{
    /// <summary>
    /// 벡터를 각도값으로 변환해줍니다.
    /// </summary>
    /// <param name="direction"></param>
    /// <returns></returns>
    public static Quaternion ToQuaternion(this Vector3 direction)
    {
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        return Quaternion.AngleAxis(angle, Vector3.forward);
    }
    /// <summary>
    /// 벡터의 가 min 좌표에서 max좌표 사이에 있도록 조절합니다.
    /// </summary>
    /// <param name="thisVec"></param>
    /// <param name="min"></param>
    /// <param name="max"></param>
    /// <returns></returns>
    public static Vector3 Clamp(this Vector3 thisVec, Vector3 min, Vector3 max)
    {
        float x = Mathf.Clamp(thisVec.x, min.x, max.x);
        float y = Mathf.Clamp(thisVec.y, min.y, max.y);
        float z = Mathf.Clamp(thisVec.z, min.z, max.z);

        return Vector3.right * x + Vector3.up * y + Vector3.forward * z;
    }

    /// <summary>
    /// randomRange만큼의 랜덤 벡터를 더하여 반환합니다.
    /// </summary>
    /// <param name="thisVec"></param>
    /// <param name="randomRange"></param>
    /// <returns></returns>
    public static Vector3 Randomize(this Vector3 thisVec, float randomRange)
    {
        return thisVec + Vector3.right * Random.Range(-randomRange, randomRange) + Vector3.up * Random.Range(-randomRange, randomRange);
    }
}
