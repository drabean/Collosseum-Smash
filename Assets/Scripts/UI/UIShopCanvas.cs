using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIShopCanvas : MonoBehaviour
{
    public GameObject totalPanel;

    public GameObject[] Pages;

    public void OpenPanel()
    {
        totalPanel.SetActive(true);
    }

    #region ���� ������

    #endregion
    #region ���� ������

    #endregion
    public void Btn_Move(bool isRight)
    {

    }
    public void Btn_Exit()
    {
        totalPanel.SetActive(false);
    }
}
