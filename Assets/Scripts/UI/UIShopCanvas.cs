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
        initShopPage();
    }

    #region 상점 페이지
    [SerializeField] UIShopPrefab ShopPrefab;
    [SerializeField] Transform ShopPrefabHolder;
    bool isShoptInit = false;
    void initShopPage()
    {
        if (isShoptInit) return;
        //공속
        UIShopPrefab prefab = Instantiate(ShopPrefab, ShopPrefabHolder);
        prefab.Init(100, UNLOCKS.ATKSPD, "attack Speed", "increase player's attack speed.");
        if (LoadedSave.Inst.save.CheckUnlock(UNLOCKS.ATKSPD)) prefab.disableBtn();

        //투척데미지
        prefab = Instantiate(ShopPrefab, ShopPrefabHolder);
        prefab.Init(60, UNLOCKS.THRWDMG, "Throw Damage", "increase player's Throw Damage.");
        if (LoadedSave.Inst.save.CheckUnlock(UNLOCKS.THRWDMG)) prefab.disableBtn();

        //투척데미지
        prefab = Instantiate(ShopPrefab, ShopPrefabHolder);
        prefab.Init(30, UNLOCKS.MAXHP, "Max HP", "increase player's Max HP.");
        if (LoadedSave.Inst.save.CheckUnlock(UNLOCKS.MAXHP)) prefab.disableBtn();

        //투척데미지
        prefab = Instantiate(ShopPrefab, ShopPrefabHolder);
        prefab.Init(30, UNLOCKS.HPREG, "HP Regeneration", "The health recovered at the end of the stage increases.");
        if (LoadedSave.Inst.save.CheckUnlock(UNLOCKS.HPREG)) prefab.disableBtn();

        //투척데미지
        prefab = Instantiate(ShopPrefab, ShopPrefabHolder);
        prefab.Init(30, UNLOCKS.MONEY, "More Coin!", "Enemy drops coin more often.");
        if (LoadedSave.Inst.save.CheckUnlock(UNLOCKS.MONEY)) prefab.disableBtn();

        isShoptInit = true;
    }
    #endregion
    #region 업적 페이지

    #endregion
    public void Btn_Move(bool isRight)
    {

    }
    public void Btn_Exit()
    {
        totalPanel.SetActive(false);
    }
}
