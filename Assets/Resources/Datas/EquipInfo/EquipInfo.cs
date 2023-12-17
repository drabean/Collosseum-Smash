using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EquipInfo", menuName = "ScriptableObject/EquipInfo")]
public class EquipInfo : ScriptableObject
{
    public List<Equip> list = new List<Equip>();
}
