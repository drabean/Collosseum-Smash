using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveData : MonoBehaviour
{
    public int Exp; // ���� ���൵ ����ġ. (ĳ���� �ر� � ���)
    public int ProgressLV;
    public Dictionary<string, bool> achievements = new Dictionary<string, bool>(); // Ŭ������ �����鿡 ���� ���� ����.
    public Dictionary<string, bool> unlocks = new Dictionary<string, bool>(); // �رݵ� �͵鿡 ���� ����. (ĳ���� ��?)
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