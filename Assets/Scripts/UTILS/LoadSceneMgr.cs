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
        // �ε� �ִϸ��̼��� Ȱ��ȭ �Ǵ� �����ݴϴ�.

        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);

        asyncOperation.allowSceneActivation = false;

        while (!asyncOperation.isDone)
        {
            // �ε� ���� ��Ȳ�� ���� �ε��� ������Ʈ �Ǵ� ���� ��Ȳ�� ǥ���ϴ� �ٸ� UI ������Ʈ�� �����մϴ�.

            // ���� ���, loadingBar.fillAmount = progress;�� ���� ������� �ε��� ������Ʈ ����

            if (asyncOperation.progress >= 0.9f)
            {
                // �ε��� �Ϸ�Ǿ��� ��
                asyncOperation.allowSceneActivation = true;

                // �ε� �ִϸ��̼��� ��Ȱ��ȭ �Ǵ� ����ϴ�.

                // ���ϴ� UI ������Ʈ�� �����մϴ�.

                // ���� ���, loadingText.text = "�ε� �Ϸ�";�� ���� ������� �ؽ�Ʈ ������Ʈ ����
            }

            yield return null;
        }
    }
}