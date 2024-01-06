using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveData
{
    public int Exp; // ���� ���൵ ����ġ. (ĳ���� �ر� � ���)
    public int ProgressLV;
    public List<int> Achievements = new List<int>();
    public Dictionary<string, bool> unlocks = new Dictionary<string, bool>(); // �رݵ� �͵鿡 ���� ����. (ĳ���� ��?)

    public bool checkAchivement(ACHIEVEMENT achievement) // Ư�� ������ Ŭ���� ���θ� ��ȯ.
    {
        if (Achievements.Contains((int)achievement)) return true;
        else return false;
    }

    public void ClearAchivement(ACHIEVEMENT achievement)
    {
        if (!Achievements.Contains((int)achievement))
        {
            Achievements.Add((int)achievement);
        }
    }
}
public enum ACHIEVEMENT
{
    TUTORIALCLEAR = 0,

}
/*
 * ���� ����
 * 00. Tutorial Ŭ����
 * 10 ~ 19. �� ������ Ŭ����
 * 20 ~ 29. �� ĳ���͵�� ���� Ŭ����
 * 
 * Exp ����
 * �������� Ŭ���Ƹ��� 1 ����
 * ����ġ�� 10 ���� �� ����, ���ο� ĳ���͸� ������.
 * Exp ���� ������ ���� �����, ���� óġ �� ������ ȹ�� �� �� Scene ��ȯ �� RunData�� ������ �̷������ Ÿ�ֿ̹� ����.
 * Exp�� ���� �ر��� ��¼����¼��
 */