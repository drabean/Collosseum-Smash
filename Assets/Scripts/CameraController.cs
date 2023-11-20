using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Vector3 min;
    public Vector3 max;
    public bool useRangeLock;
    Camera cam;

    //X방향 Shake값 Curve
    [SerializeField] AnimationCurve shakeX;
    //Y방향 Shake값 Curve
    [SerializeField] AnimationCurve shakeY;

    //다른 진동에 의해 덮어쓰여지면 안되는 진동.
    bool isShakeLocked;

    bool targetLock;

    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    private void Start()
    {
        camOriginSize = cam.orthographicSize;
    }

    #region 추적
    [SerializeField] Transform target;
    Transform baseTarget;
    [SerializeField] Vector3 offset;



    private void Update()
    {
        if (targetLock) return;
            
         if(target != null)transform.position = Vector3.Lerp(transform.position, clampVector(target.transform.position) + offset, 0.2f);
       //if(!isShaking) transform.position = target.position + offset;
    }

    Vector3 clampVector(Vector3 point)
    {
        // 카메라 이동 가능 범위를 벗어나지 않도록 Clamp 처리합니다.
        float clampedX = Mathf.Clamp(point.x, min.x, max.x);
        float clampedY = Mathf.Clamp(point.y, min.y, max.y);

        return Vector2.right * clampedX + Vector2.up * clampedY;
    }


    #endregion

    /// <summary>
    /// 화면 흔들기
    /// </summary>
    /// <param name="duration">화면을 흔드는 총 시간</param>
    /// <param name="shakeSpeed">초당  흔드는 횟수</param>
    /// <param name="xPower"></param>
    /// <param name="yPower"></param>
    public void Shake(float duration, float shakeSpeed, float xPower, float yPower, bool isForced = false)
    {
        if (isShakeLocked) return;
        StopAllCoroutines();
        StartCoroutine(co_Shake(duration, shakeSpeed, xPower, yPower, isForced));
    }
    IEnumerator co_Shake(float duration, float shakeSpeed, float xPower, float yPower, bool isForced = false)
    {
        float timer = 0;
        if (isForced) isShakeLocked = true;
        while (timer <= duration)
        {
            float x = shakeX.Evaluate(timer * shakeSpeed) * xPower;
            float y = shakeX.Evaluate(timer * shakeSpeed) * yPower;

            transform.localPosition = clampVector(target.position) + new Vector3(x, y, 0f) + offset;


            timer += Time.deltaTime;
            yield return null;
        }

        if (isForced) isShakeLocked = false;
    }


    //카메라 원래 사이즈
    float camOriginSize;

    /// <summary>
    /// 화면 확대 
    /// </summary>
    /// <param name="duration">확대하는 시간</param>
    /// <param name="power">확대하는 정도</param>
    public void Zoom(float duration, float power)
    {
        StartCoroutine(co_Zoom(duration, power));
    }

    IEnumerator co_Zoom(float duration, float power)
    {
        float targetSize = camOriginSize * power;

        float time = 0f;

        while (time < duration * 0.5f)
        {
            float t = time / (duration * 0.5f);
            cam.orthographicSize = Mathf.Lerp(camOriginSize, targetSize, t);
            time += Time.deltaTime;
            yield return null;
        }

        time = 0f;

        while (time < duration * 0.5f)
        {
            float t = time / (duration * 0.5f);
            cam.orthographicSize = Mathf.Lerp(targetSize, camOriginSize, t);
            time += Time.deltaTime;
            yield return null;
        }

        cam.orthographicSize = camOriginSize;
    }

    public void lockPos(Vector3 pos)
    {
        targetLock = true;
        transform.position = pos;
    }
    public void changeTarget(Transform followTarget)
    {
        target = followTarget;
    }

    public void changeTargetToDefault()
    {
        target = baseTarget;
    }

    public void SetBaseTarget(Transform baseTarget)
    {
        this.baseTarget = baseTarget;
        target = baseTarget;
    }
}
