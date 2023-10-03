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
    /// 이 함수를 실행함으로서 LoadScene을 불러오고, LoadScene의 Start에서 다음 씬을 비동기 로딩함.
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

        while (!operation.isDone) // 로딩 완료까지 대기
        {
            yield return null;
        }
        Debug.Log("여긴왔네");
        //최소대기시간
        yield return new WaitForSeconds(1.0f);
        operation.allowSceneActivation = true;

    }
}