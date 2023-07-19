using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    Camera cam;
    private void Awake()
    {
        cam = GetComponent<Camera>();
    }
    public void Shake(float duration, float power)
    {
        StartCoroutine(co_Shake(duration, power));
    }

    IEnumerator co_Shake(float duration, float magnitude)
    {
        Vector3 camOriginPos = transform.localPosition;

        float timer = 0;

        while (timer <= duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;

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
