using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartSceneMgr : MonoSingleton<StartSceneMgr>
{
    [SerializeField] UICharacterSelectCanvas CharacterSelectGroup;
    [SerializeField] UIItemDescriptionPanel ItemDescription;


    private void Awake()
    {
        if(!LoadedData.Inst.isDataLoaded) LoadedData.Inst.LoadData();
    }
    public void BtnStart()
    {
        Debug.Log("START");
        //SceneManager.LoadScene("Main");
        LoadSceneMgr.LoadSceneAsync("Main");
    }

    public void BtnOption()
    {
        Debug.Log("OpenOption");
        LoadSceneMgr.LoadSceneAsync("Tutorial");
    }

    public void Btn_Exit()
    {
        Application.Quit();
    }

    public void Btn_CharacterChange()
    {
        CharacterSelectGroup.gameObject.SetActive(true);
        CharacterSelectGroup.OpenCharacterSelect(0);
    }

    public void ShowItemDescription(Equip equip)
    {
        ItemDescription.ShowPanel(equip);
        ItemDescription.gameObject.SetActive(true);
    }
}
