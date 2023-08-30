using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Combo
{
    //�޺� ���� �ð�
    const float comboLimit = 1.5f;
    //������ �޺� ���� �ð�
    float lastComboTIme = -10;

    int curCombo = 1;

    public int increaseCombo()
    {
        if (Time.time - lastComboTIme < comboLimit) curCombo++;
        else curCombo = 1;

        lastComboTIme = Time.time;

        return curCombo;
    }

    /// <summary>
    /// �޺��� �ø��� �ʰ�, �ð��� ����.
    /// </summary>
    public void updateComboTime()
    {
        lastComboTIme = Time.time;
    }

    /// <summary>
    /// �޺��� �����ñ��� �ʰ�, ���� �޺��� ��ȯ
    /// </summary>
    /// <returns></returns>
    public int GetCombo()
    {
        return curCombo;
    }
}
