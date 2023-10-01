using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingSceneMgr : MonoSingleton<LoadingSceneMgr>
{

    public void LoadNextSceneAsync(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    private IEnumerator LoadSceneAsync(string sceneName)
    {
        // 로딩 애니메이션을 활성화 또는 보여줍니다.

        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);

        asyncOperation.allowSceneActivation = false;

        while (!asyncOperation.isDone)
        {
            // 로딩 진행 상황에 따라 로딩바 업데이트 또는 진행 상황을 표시하는 다른 UI 업데이트를 수행합니다.

            // 예를 들어, loadingBar.fillAmount = progress;와 같은 방법으로 로딩바 업데이트 가능

            if (asyncOperation.progress >= 0.9f)
            {
                // 로딩이 완료되었을 때
                asyncOperation.allowSceneActivation = true;

                // 로딩 애니메이션을 비활성화 또는 숨깁니다.

                // 원하는 UI 업데이트를 수행합니다.

                // 예를 들어, loadingText.text = "로딩 완료";와 같은 방법으로 텍스트 업데이트 가능
            }

            yield return null;
        }
    }
}