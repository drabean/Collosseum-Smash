using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Combo
{
    //콤보 유지 시간
    const float comboLimit = 1.5f;
    //마지막 콤보 갱신 시간
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
    /// 콤보를 올리지 않고, 시간만 갱신.
    /// </summary>
    public void updateComboTime()
    {
        lastComboTIme = Time.time;
    }

    /// <summary>
    /// 콤보를 증가시기지 않고, 현재 콤보만 반환
    /// </summary>
    /// <returns></returns>
    public int GetCombo()
    {
        return curCombo;
    }
}
