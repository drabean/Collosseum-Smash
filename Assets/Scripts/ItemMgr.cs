using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//������ ���� ������ Ǯ�� �����ϴ� ��ũ��Ʈ�Դϴ�.
public class ItemMgr : MonoSingleton<ItemMgr>
{
    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        //TODO: ���� Ȯ���Ͽ� �������� Ǯ�� �߰��ϱ�
    }

    /// <summary>
    /// ���� ��� ����
    /// </summary>
    public List<Equip> equipPool; 

    public Equip getRandomEquip()
    {
        if (equipPool.Count == 0) return null;

        Equip equip = equipPool[Random.Range(0, equipPool.Count)];
        equipPool.Remove(equip);

        return equip;
    }

}
