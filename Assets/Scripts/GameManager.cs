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

    public void Shake(float duration, float power)
    {
        m_mainCam.Shake(duration, power);
    }

    public void Zoom(float duration, float power)
    {
        m_mainCam.Zoom(duration, power);
    }
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
