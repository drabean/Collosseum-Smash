using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Background : MonoBehaviour
{
    float spd = 0.4f;
    int dir = 1;

    WaitForSeconds waitForBackgroundMovement = new WaitForSeconds(1.5f);
    private IEnumerator Start()
    {
        float t = 0;

        while (true)
        {
            transform.position += Time.deltaTime * spd * Vector3.right * dir;
            t += Time.deltaTime;

            if(t >= 3)
            {
                t = 0;
                dir *= -1;
            }
            yield return null;
        }
    }
}
