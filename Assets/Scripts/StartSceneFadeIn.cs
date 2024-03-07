using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartSceneFadeIn : MonoBehaviour
{
    public CanvasGroup LoadingPanel;

    private void Awake()
    {
        if (!MainGameLogic.isInit) StartCoroutine(co_FadeIn(1.0f));
        else Destroy(gameObject);
    }

    IEnumerator co_FadeIn(float duration)
    {
        float progress = 0;

        do
        {
            LoadingPanel.alpha = 1 - progress;
            progress += Time.deltaTime / duration;

            yield return null;
        } while (progress < 1);

        Destroy(gameObject);
    }
}
