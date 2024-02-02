using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LoadSceneMgr : MonoSingleton<LoadSceneMgr>
{
    static string nextSceneName;
    AsyncOperation operation;

    public CanvasGroup LoadingPanel;
    float loadingWaitTIme = 4.0f;

    public TextMeshProUGUI TMPTip;
    [TextArea]
    public List<string> Tips;
    
    void Awake()
    {
        DontDestroyOnLoad(gameObject);

    }
    

    private void Start()
    { 
        //SingleTone을 통해 LoadScene을 불러오고, LoadScene의 Start 함수에서 씬 전환을 실행해줌.
        StartCoroutine(co_AsyncLoading());
    }


    /// <summary>
    /// 이 함수를 실행함으로서 LoadScene을 불러오고, LoadScene의 Start에서 다음 씬을 비동기 로딩함.
    /// </summary>
    /// <param name="sceneName"></param>
    public static void LoadSceneAsync(string sceneName)
    {
        nextSceneName = sceneName;

        SceneManager.LoadScene("Loading", LoadSceneMode.Additive);

    }
    IEnumerator co_AsyncLoading()
    {
        SoundMgr.Inst.Play("LoadScene");
        Time.timeScale = 1;
        TMPTip.text = Tips[Random.Range(0, Tips.Count)];
        // Fade In 효과 실행
        yield return StartCoroutine(Fade(0.3f, true));

        // 비동기 씬 로딩을 시작
        operation = SceneManager.LoadSceneAsync(nextSceneName);
        operation.allowSceneActivation = false;

        yield return new WaitForSeconds(loadingWaitTIme);

      
        operation.allowSceneActivation = true;

        // Fade Out 효과 실행
        yield return StartCoroutine(Fade(0.3f, false));
        Destroy(gameObject);
    }

    #region Fade
    //ver1. Fade In / Fade Out
    /// <summary>
    /// Fade In / Fade Out을 관리하는 함수
    /// </summary>
    /// <param name="duration">Fade에 걸리는 시간</param>
    /// <param name="isIn"> True일시 Fade In, False일시 Fade Out</param>
    /// <returns></returns>
    IEnumerator Fade(float duration, bool isIn)
    {
        float progress = 0;

        do
        {
            if (isIn) LoadingPanel.alpha = progress;
            else LoadingPanel.alpha = 1 - progress;
            progress += Time.deltaTime / duration;

            yield return null;
        } while (progress < 1);

    }

    #endregion
}