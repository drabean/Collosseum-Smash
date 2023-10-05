using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadSceneMgr : MonoSingleton<LoadSceneMgr>
{
    static string nextSceneName;
    AsyncOperation operation;

    private void Start()
    { 
        StartCoroutine(co_AsyncLoading());
    }


    /// <summary>
    /// �� �Լ��� ���������μ� LoadScene�� �ҷ�����, LoadScene�� Start���� ���� ���� �񵿱� �ε���.
    /// </summary>
    /// <param name="sceneName"></param>
    public static void LoadSceneAsync(string sceneName)
    {
        nextSceneName = sceneName;
        SceneManager.LoadScene("Loading");

    }
    IEnumerator co_AsyncLoading()
    {
        SoundMgr.Inst.Play("LoadScene");
        Time.timeScale = 1;

        operation = SceneManager.LoadSceneAsync(nextSceneName);

        operation.allowSceneActivation = false;

        yield return new WaitForSeconds(0.5f);
        operation.allowSceneActivation = true;

    }
}