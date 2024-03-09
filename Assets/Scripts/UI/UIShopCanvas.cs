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
        //TODO: ���� �߰��ϱ�

        UIAdPrefab ad = Instantiate(AdPrefab, ShopPrefabHolder);
        ad.Init();

        //����
        UIShopPrefab prefab = Instantiate(ShopPrefab, ShopPrefabHolder);
        prefab.Init(100, UNLOCK.ATKSPD, "Attack Speed", "Increase player's attack speed.");
        if (LoadedSave.Inst.save.CheckUnlock(UNLOCK.ATKSPD)) prefab.disableBtn();

        //��ô������
        prefab = Instantiate(ShopPrefab, ShopPrefabHolder);
        prefab.Init(50, UNLOCK.THRWDMG, "Throw Damage", "Increase player's Throw Damage.");
        if (LoadedSave.Inst.save.CheckUnlock(UNLOCK.THRWDMG)) prefab.disableBtn();

        //�ִ�ü��
        prefab = Instantiate(ShopPrefab, ShopPrefabHolder);
        prefab.Init(50, UNLOCK.MAXHP, "Max HP", "Increase player's Max HP.");
        if (LoadedSave.Inst.save.CheckUnlock(UNLOCK.MAXHP)) prefab.disableBtn();

        //ü�����
        prefab = Instantiate(ShopPrefab, ShopPrefabHolder);
        prefab.Init(30, UNLOCK.HPREG, "HP Regeneration", "The health recovered at the end of the stage increases.");
        if (LoadedSave.Inst.save.CheckUnlock(UNLOCK.HPREG)) prefab.disableBtn();

        //��
        prefab = Instantiate(ShopPrefab, ShopPrefabHolder);
        prefab.Init(30, UNLOCK.MONEY, "More Coin!", "Enemy drops coin more often.");
        if (LoadedSave.Inst.save.CheckUnlock(UNLOCK.MONEY)) prefab.disableBtn();

        //��ô������
        prefab = Instantiate(ShopPrefab, ShopPrefabHolder);
        prefab.Init(30, UNLOCK.REVIVE, "More Life!", "Player can revive 1 more time.");
        if (LoadedSave.Inst.save.CheckUnlock(UNLOCK.REVIVE)) prefab.disableBtn();

        isShoptInit = true;
    }

    public void Btn_Exit()
    {
        totalPanel.SetActive(false);
    }
}
