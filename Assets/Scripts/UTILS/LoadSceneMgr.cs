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
        //SingleTone�� ���� LoadScene�� �ҷ�����, LoadScene�� Start �Լ����� �� ��ȯ�� ��������.
        StartCoroutine(co_AsyncLoading());
    }


    /// <summary>
    /// �� �Լ��� ���������μ� LoadScene�� �ҷ�����, LoadScene�� Start���� ���� ���� �񵿱� �ε���.
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
        // Fade In ȿ�� ����
        yield return StartCoroutine(Fade(0.3f, true));

        // �񵿱� �� �ε��� ����
        operation = SceneManager.LoadSceneAsync(nextSceneName);
        operation.allowSceneActivation = false;

        yield return new WaitForSeconds(loadingWaitTIme);

      
        operation.allowSceneActivation = true;

        // Fade Out ȿ�� ����
        yield return StartCoroutine(Fade(0.3f, false));
        Destroy(gameObject);
    }

    #region Fade
    //ver1. Fade In / Fade Out
    /// <summary>
    /// Fade In / Fade Out�� �����ϴ� �Լ�
    /// </summary>
    /// <param name="duration">Fade�� �ɸ��� �ð�</param>
    /// <param name="isIn"> True�Ͻ� Fade In, False�Ͻ� Fade Out</param>
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