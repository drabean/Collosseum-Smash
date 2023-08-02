using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ComboMgr :Singleton<ComboMgr>
{
    const float comboLimit = 3;
    float lastComboTIme = -10;

    int curCombo = 1;

    public int checkCombo()
    {
        if (Time.time - lastComboTIme < comboLimit) curCombo++;
        else curCombo = 1;

        lastComboTIme = Time.time;

        return curCombo;
    }
}
