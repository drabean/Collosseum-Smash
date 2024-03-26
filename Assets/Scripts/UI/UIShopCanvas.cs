using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIShopCanvas : MonoBehaviour
{
    public GameObject totalPanel;
    [SerializeField] UIShopPrefab ShopPrefab;
    [SerializeField] UIAdPrefab AdPrefab;
    [SerializeField] Transform ShopPrefabHolder;
    bool isShoptInit = false;

    public void OpenPanel()
    {
        totalPanel.SetActive(true);
        initShopPanel();
    }

    void initShopPanel()
    {
        if (isShoptInit) return;
        //TODO: 광고 추가하기

        UIAdPrefab ad = Instantiate(AdPrefab, ShopPrefabHolder);
        ad.Init();


        UIShopPrefab prefab;

        if (LoadedSave.Inst.save.CheckAchivement(ACHIEVEMENT.NORMALCLEAR))
        {
            prefab = Instantiate(ShopPrefab, ShopPrefabHolder);
            prefab.Init(200, UNLOCK.ADDITEM, "inherited equipment", "At the beginning of the game, you start with one additional random equipment.");
            if (LoadedSave.Inst.save.CheckUnlock(UNLOCK.ADDITEM)) prefab.disableBtn();
        }
        //공속
        prefab = Instantiate(ShopPrefab, ShopPrefabHolder);
        prefab.Init(200, UNLOCK.ATKSPD, "Quick Hands", "Increase player's attack speed.");
        if (LoadedSave.Inst.save.CheckUnlock(UNLOCK.ATKSPD)) prefab.disableBtn();

        //공격력
        prefab = Instantiate(ShopPrefab, ShopPrefabHolder);
        prefab.Init(200, UNLOCK.ATKDMG, "Innate Strength", "Increase player's attack damage");
        if (LoadedSave.Inst.save.CheckUnlock(UNLOCK.ATKDMG)) prefab.disableBtn();

        //이동속도
        prefab = Instantiate(ShopPrefab, ShopPrefabHolder);
        prefab.Init(100, UNLOCK.MOVSPD, "Innate Agility", "Increase player's move speed.");
        if (LoadedSave.Inst.save.CheckUnlock(UNLOCK.MOVSPD)) prefab.disableBtn();

        //최대체력
        prefab = Instantiate(ShopPrefab, ShopPrefabHolder);
        prefab.Init(50, UNLOCK.MAXHP, "Innate Stamina", "Increase player's Max HP.");
        if (LoadedSave.Inst.save.CheckUnlock(UNLOCK.MAXHP)) prefab.disableBtn();


        //투척데미지
        prefab = Instantiate(ShopPrefab, ShopPrefabHolder);
        prefab.Init(50, UNLOCK.THRWDMG, "Innate Accuracy", "Increase player's Throw Damage.");
        if (LoadedSave.Inst.save.CheckUnlock(UNLOCK.THRWDMG)) prefab.disableBtn();

        //체력재생
        prefab = Instantiate(ShopPrefab, ShopPrefabHolder);
        prefab.Init(50, UNLOCK.HPREG, "Comfortable Bed", "After Clearing Stage, restore all HP");
        if (LoadedSave.Inst.save.CheckUnlock(UNLOCK.HPREG)) prefab.disableBtn();

        //돈
        prefab = Instantiate(ShopPrefab, ShopPrefabHolder);
        prefab.Init(50, UNLOCK.MONEY, "More Coin!", "Enemies drops coin more often.");
        if (LoadedSave.Inst.save.CheckUnlock(UNLOCK.MONEY)) prefab.disableBtn();

        //부활횟수
        prefab = Instantiate(ShopPrefab, ShopPrefabHolder);
        prefab.Init(50, UNLOCK.REVIVE, "More Life!", "Player can revive one more time.");
        if (LoadedSave.Inst.save.CheckUnlock(UNLOCK.REVIVE)) prefab.disableBtn();

        isShoptInit = true;
    }

    public void Btn_Exit()
    {
        totalPanel.SetActive(false);
    }
}
