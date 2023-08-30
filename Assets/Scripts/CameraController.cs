using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Vector3 min;
    public Vector3 max;
    Camera cam;

    //X���� Shake�� Curve
    [SerializeField] AnimationCurve shakeX;
    //Y���� Shake�� Curve
    [SerializeField] AnimationCurve shakeY;

    //�ٸ� ������ ���� ��������� �ȵǴ� ����.
    bool isLocked;
    //����, ȸ������ �����ϋ� true. 
    bool isShaking;
    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    private void Start()
    {
        camOriginSize = cam.orthographicSize;
        if (target == null) target = GameObject.FindObjectOfType<Player>().transform;
    }

    #region ����
    [SerializeField] Transform target;
    [SerializeField] Vector3 offset;



    private void Update()
    {

       if(!isShaking) transform.position = Vector3.Lerp(transform.position, clampVector(target.transform.position) + offset, 0.2f);
       //if(!isShaking) transform.position = target.position + offset;
    }

    Vector3 clampVector(Vector3 point)
    {
        // ī�޶� �̵� ���� ������ ����� �ʵ��� Clamp ó���մϴ�.
        float clampedX = Mathf.Clamp(point.x, min.x, max.x);
        float clampedY = Mathf.Clamp(point.y, min.y, max.y);

        return Vector2.right * clampedX + Vector2.up * clampedY;
    }


    #endregion

    /// <summary>
    /// ȭ�� ����
    /// </summary>
    /// <param name="duration">ȭ���� ���� �� �ð�</param>
    /// <param name="shakeSpeed">�ʴ�  ���� Ƚ��</param>
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
        isShaking = true;
        float timer = 0;
        if (isForced) isLocked = true;
        while (timer <= duration)
        {
            float x = shakeX.Evaluate(timer * shakeSpeed) * xPower;
            float y = shakeX.Evaluate(timer * shakeSpeed) * yPower;

            transform.localPosition = clampVector(target.position) + new Vector3(x, y, 0f) + offset;


            timer += Time.deltaTime;
            yield return null;
        }

        if (isForced) isLocked = false;
        isShaking = false;
    }


    //ī�޶� ���� ������
    float camOriginSize;

    /// <summary>
    /// ȭ�� Ȯ�� 
    /// </summary>
    /// <param name="duration">Ȯ���ϴ� �ð�</param>
    /// <param name="power">Ȯ���ϴ� ����</param>
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
}
