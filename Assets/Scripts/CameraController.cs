using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    Camera cam;

    [SerializeField] AnimationCurve shakeX;
    [SerializeField] AnimationCurve shakeY;

    //다른 진동에 의해 캔슬되면 안되는 진동일떄 함
    bool isLocked;
    private void Awake()
    {
        cam = GetComponent<Camera>();

        StartCoroutine(co_FollowTarget());
    }

    private void Start()
    {
        if (target == null) target = GameObject.FindObjectOfType<Player>().transform;
    }
    #region 추적
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
    /// 화면 흔들기
    /// </summary>
    /// <param name="duration">화면을 흔드는 총 시간</param>
    /// <param name="shakeSpeed">초당  흔드는 횟수</param>
    /// <param name="xPower"></param>
    /// <param name="yPower"></param>
    public void Shake(float duration, float shakeSpeed, float xPower, float yPower, bool isForced)
    {
        if (isLocked) return;
        StopAllCoroutines();
        StartCoroutine(co_Shake(duration, shakeSpeed, xPower, yPower, isForced));
    }
    IEnumerator co_Shake(float duration, float shakeSpeed, float xPower, float yPower, bool isForced)
    {

        float timer = 0;
        if (isForced) isLocked = true;
        while (timer <= duration)
        {
            float x = shakeX.Evaluate(timer * shakeSpeed) * xPower;
            float y = shakeX.Evaluate(timer * shakeSpeed) * yPower;

            transform.localPosition = target.position + new Vector3(x, y, 0f) + offset;


            timer += Time.deltaTime;
            yield return null;
        }

        if (isForced) isLocked = false;
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
