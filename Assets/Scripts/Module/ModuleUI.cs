using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModuleUI : MonoBehaviour
{
    Image[] images;
    private void Awake()
    {
        images = GetComponentsInChildren<Image>();
    }

    public void FadeOut(float duration)
    {
        foreach (Image image in images)
        {
            // Fade ó�� �ڷ�ƾ ����
            StartCoroutine(FadeOut(image, duration));
        }
    }

    private IEnumerator FadeOut(Image image, float duration)
    {
        Color curColor = image.color;

        float time = 0;
        float curAlpha = 1.0f;

        // ��ǥ ���İ����� Fade ó��
        while (time <= duration)
        {
            curAlpha -= Time.deltaTime / duration;

            curColor = new Color(curColor.r, curColor.g, curColor.b, curAlpha);
            image.color = curColor;

            time += Time.deltaTime;
            yield return null;
        }

        if (gameObject.activeInHierarchy) gameObject.SetActive(false);
    }

    public void FadeIn(float duration)
    {
        gameObject.SetActive(true);

        foreach (Image image in images)
        {
            // Fade ó�� �ڷ�ƾ ����
            StartCoroutine(FadeIn(image, duration));
        }
    }

    private IEnumerator FadeIn(Image image, float duration)
    {
        Color curColor = image.color;

        float time = 0;
        float curAlpha = 0.0f;

        // ��ǥ ���İ����� Fade ó��
        while (time <= duration)
        {
            curAlpha += Time.deltaTime / duration;

            curColor = new Color(curColor.r, curColor.g, curColor.b, curAlpha);
            image.color = curColor;

            time += Time.deltaTime;
            yield return null;
        }
    }
}
