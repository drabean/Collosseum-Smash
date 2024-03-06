using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartSceneCamera : MonoSingleton<StartSceneCamera>
{
    [SerializeField] AnimationCurve shakeX;

    public void Shake()
    {
        StopAllCoroutines();
        StartCoroutine(co_Shake(0.1f, 20, 0.2f));
    }
    IEnumerator co_Shake(float duration, float shakeSpeed, float xPower)
    {
        Vector3 startPos = transform.position;
        float timer = 0;
        while (timer <= duration)
        {
            float x = shakeX.Evaluate(timer * shakeSpeed) * xPower;

            transform.position = startPos + Vector3.right * x;


            timer += Time.deltaTime;
            yield return null;
        }

        transform.position = startPos;
    }
}
