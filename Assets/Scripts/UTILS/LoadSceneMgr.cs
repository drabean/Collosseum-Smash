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
        Time.timeScale = 1;

        operation = SceneManager.LoadSceneAsync(nextSceneName);

        while (!operation.isDone) // �ε� �Ϸ���� ���
        {
            yield return null;
        }
        Debug.Log("����Գ�");
        //�ּҴ��ð�
        yield return new WaitForSeconds(1.0f);
        operation.allowSceneActivation = true;

    }
}