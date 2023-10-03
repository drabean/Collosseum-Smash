using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartSceneMgr : MonoBehaviour
{
    public void BtnStart()
    {
        Debug.Log("START");
        //SceneManager.LoadScene("Main");
        LoadSceneMgr.LoadSceneAsync("Main");
    }

    public void BtnOption()
    {
        Debug.Log("OpenOption");
    }

    public void Btn_Exit()
    {
        Application.Quit();
    }
}
