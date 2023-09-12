using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIRetryBtn : MonoBehaviour
{
    public void onPress()
    {
        Debug.Log("ASD");
        SceneManager.LoadScene("Main");
    }
}
