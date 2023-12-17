using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ������ ���۵� �� �ε��Ǵ� �Һ�������
/// </summary>
public class LoadedData : Singleton<LoadedData>
{
    public bool isDataLoaded = false;

    Dictionary<int, CharacterInfo> CharacterInfos = new Dictionary<int, CharacterInfo>();
    public int characterInfosCount = 0;

    public StageInfo[] stageInfos;

    Dictionary<int, Equip> Equips = new Dictionary<int, Equip>();

    string characterInfoPath = "Datas/CharacterInfo";
    string stageInfoPath = "Datas/StageInfo";
    string equipPath = "Datas/EquipInfo";

    public void LoadData()
    {
        CharacterInfo[] characterInfos = Resources.LoadAll<CharacterInfo>(characterInfoPath);

        foreach(CharacterInfo c in characterInfos)
        {
            if(!CharacterInfos.ContainsKey(c.ID))
            {
                CharacterInfos.Add(c.ID, c);
                characterInfosCount++;
            }
        }

        stageInfos = Resources.LoadAll<StageInfo>(stageInfoPath);


        EquipInfo[] equips = Resources.LoadAll<EquipInfo>(equipPath);

        foreach(EquipInfo e in equips)
        {
            foreach(Equip eq in e.list)
            {
                if (!Equips.ContainsKey(eq.ID))
                {
                    Equips.Add(eq.ID, eq);
                }
            }
        }

        isDataLoaded = true;
    }

    public CharacterInfo getCharacterInfoByID(int idx)
    {
        return CharacterInfos[idx];
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
