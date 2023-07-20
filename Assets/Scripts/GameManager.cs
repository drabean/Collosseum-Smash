using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    CameraController m_mainCam;

    protected override void Awake()
    {
        base.Awake();
        m_mainCam = Camera.main.GetComponent<CameraController>();
    }

    /// <summary>
    /// ī�޶� ���� �ð����� ���ϴ�.
    /// </summary>
    /// <param name="duration"></param>
    /// <param name="shakeSpeed"></param>
    /// <param name="xPower"></param>
    /// <param name="yPower"></param>
    public void Shake(float duration, float shakeSpeed, float xPower, float yPower = 0)
    {
        m_mainCam.Shake(duration, shakeSpeed, xPower, yPower);
    }

    /// <summary>
    /// ī�޶� ���� �ð����� Ȯ���մϴ�.
    /// </summary>
    /// <param name="duration">Ȯ�� �� ������� ���� �� �� ������ �ð�</param>
    /// <param name="power">Ȯ�� ���� (%)</param>
    public void Zoom(float duration, float power)
    {
        m_mainCam.Zoom(duration, power);
    }


    /// <summary>
    /// ���� �ð����� �ð� ������ �����մϴ�.
    /// </summary>
    /// <param name="time"> ������ �����ϴ� �ð� </param>
    /// <param name="amount"> �ð� ���� </param>
    public void SlowTime(float time, float amount)
    {
        StartCoroutine(co_SlowTime(time, amount));
    }

    IEnumerator co_SlowTime(float time, float amount)
    {
        Time.timeScale = amount;

        yield return new WaitForSecondsRealtime(time);

        Time.timeScale = 1;
    }
}
