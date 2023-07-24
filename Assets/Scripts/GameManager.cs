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
    /// 카메라를 일정 시간동안 흔듭니다.
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
    /// 카메라를 일정 시간동안 확대합니다.
    /// </summary>
    /// <param name="duration">확대 후 원래대로 돌아 올 때 까지의 시간</param>
    /// <param name="power">확대 비율 (%)</param>
    public void Zoom(float duration, float power)
    {
        m_mainCam.Zoom(duration, power);
    }


    /// <summary>
    /// 일정 시간동안 시간 배율을 조절합니다.
    /// </summary>
    /// <param name="time"> 배율을 조절하는 시간 </param>
    /// <param name="amount"> 시간 배율 </param>
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
