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

        StartCoroutine(co_FollowTarget());
    }
    #region ÃßÀû
    [SerializeField] Transform target;
    [SerializeField] Vector3 offset;
    //TEST
    

    IEnumerator co_FollowTarget()
    {
        while(true)
        {
            transform.position = Vector3.Lerp(transform.position, target.position + offset, 0.2f);
            yield return null;
        }
    }




    #endregion

    /// <summary>
    /// È­¸é Èçµé±â
    /// </summary>
    /// <param name="duration">È­¸éÀ» Èçµå´Â ÃÑ ½Ã°£</param>
    /// <param name="shakeSpeed">ÃÊ´ç  Èçµå´Â È½¼ö</param>
    /// <param name="xPower"></param>
    /// <param name="yPower"></param>
    public void Shake(float duration, float shakeSpeed, float xPower, float yPower)
    {
        StopAllCoroutines();
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
        StartCoroutine(co_FollowTarget());
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
        StartCoroutine(co_FollowTarget());
    }
}
