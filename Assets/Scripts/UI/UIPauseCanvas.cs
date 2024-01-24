using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class UIPauseCanvas : MonoBehaviour
{
    int curPageIdx = 0;
    public GameObject[] Pages;

    public TextMeshProUGUI TMPTitle;

    public GameObject totalPanel;

    public GameObject BtnGroup;
    public GameObject OptionGroup;
    public GameObject ItemListGroup;

    public void OpenPausePanel()
    {
        Debug.Log("PAUSED");
        TMPTitle.text = "Pause";
        totalPanel.SetActive(true);
        Time.timeScale = 0;
        Btn_Move(false);

    }
    public void ClosePausePanel()
    {
        Time.timeScale = 1;
        totalPanel.SetActive(false);
    }


    public void Btn_OpenOption()
    {

    }
    public void Btn_Move(bool isRight)
    {
        Pages[curPageIdx].SetActive(false);
        if (isRight) curPageIdx++; else curPageIdx--;
        if(curPageIdx < 0 ) curPageIdx += Pages.Length;
        curPageIdx %= Pages.Length;

        Pages[curPageIdx].SetActive(true);

        switch(curPageIdx)
        {
            case 0:
                OpenItemPage();
                break;
            case 1:
                OpenOptionPage();
                break;
        }
    }
    #region 아이템 관련
    public RectTransform ItemHolder;
    public UIEquipHolder EquipHolderPrefab;
    bool isItemInit;

    public void OpenItemPage()
    {
        TMPTitle.text = "Equips";

        if (isItemInit) return;
        isItemInit = true;
        RunData curRunData = GameMgr.Inst.curRunData;

        for(int i = 0; i < curRunData.item.Count; i++)
        {
            Equip curEquip = LoadedData.Inst.getEquipByID (curRunData.item[i]);
            UIEquipHolder temp = Instantiate(EquipHolderPrefab);
            temp.SetEquip(curEquip);
            temp.transform.SetParent(ItemHolder.transform, false);
        }
    }
    #endregion

    #region 옵션 관련

    public void OpenOptionPage()
    {
        TMPTitle.text = "Option";
    }


    #endregion
    public void Btn_BackToTitle()
    {

    }
}
