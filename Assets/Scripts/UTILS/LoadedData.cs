using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ������ ���۵� �� �ε��Ǵ� �Һ�������
/// </summary>
public class LoadedData : Singleton<LoadedData>
{
    public bool isDataLoaded = false;

    public CharacterInfo[] characterInfos;
    public StageInfo[] stageInfos;

    Dictionary<int, Equip> Equips = new Dictionary<int, Equip>();

    string characterInfoPath = "Datas/CharacterInfo";
    string stageInfoPath = "Datas/StageInfo";
    string equipPath = "Datas/Equip";

    public void LoadData()
    {
        characterInfos = Resources.LoadAll<CharacterInfo>(characterInfoPath);
        stageInfos = Resources.LoadAll<StageInfo>(stageInfoPath);
        Equip[] equips = Resources.LoadAll<Equip>(equipPath);

        foreach(Equip e in equips)
        {
            Equips.Add(e.ID, e);
        }

        isDataLoaded = true;
    }

    public CharacterInfo getCharacterInfoByID(int idx)
    {
        return characterInfos[idx];
    }
    public Equip getEquipByID(int ID)
    {
        if(Equips.ContainsKey(ID)) return Equips[ID];
        else
        {
            Debug.Log("ID�� �ش��ϴ� �������� �������� �ʽ��ϴ�");
            return null;
        }
    }
}
