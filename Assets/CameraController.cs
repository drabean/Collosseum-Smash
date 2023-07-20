using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    Camera cam;

    [SerializeField] AnimationCurve shakeX;
    [SerializeField] AnimationCurve shakeY;

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    /// <summary>
    /// 화면을 흔드는 함수입니다.
    /// </summary>
    /// <param name="duration">화면을 흔드는 총 시간</param>
    /// <param name="shakeSpeed">초당  흔드는 횟수</param>
    /// <param name="xPower"></param>
    /// <param name="yPower"></param>
    public void Shake(float duration, float shakeSpeed, float xPower, float yPower)
    {
        StartCoroutine(co_Shake(duration, shakeSpeed, xPower, yPower));
    }
    IEnumerator co_Shake(float duration, float shakeSpeed, float xPower, float yPower)
    {
        Vector3 camOriginPos = transform.localPosition;

        float timer = 0;

        while (timer <= duration)
        {
            float x = shakeX.Evaluate(timer * shakeSpeed) * xPower;
            float y = shakeX.Evaluate(timer * shakeSpeed) * yPower;

            transform.localPosition = camOriginPos + new Vector3(x, y, 0f);


            timer += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = camOriginPos;
    }
    public void Zoom(float duration, float power)
    {
        StartCoroutine(co_Zoom(duration, power));
    }

    IEnumerator co_Zoom(float duration, float power)
    {
        float originSize = cam.orthographicSize;
        float targetSize = originSize * power;

        float elapsedTime = 0f;

        while (elapsedTime < duration * 0.5f)
        {
            float t = elapsedTime / (duration * 0.5f);
            cam.orthographicSize = Mathf.Lerp(originSize, targetSize, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        elapsedTime = 0f;

        while (elapsedTime < duration * 0.5f)
        {
            float t = elapsedTime / (duration * 0.5f);
            cam.orthographicSize = Mathf.Lerp(targetSize, originSize, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        cam.orthographicSize = originSize;

    }
}
