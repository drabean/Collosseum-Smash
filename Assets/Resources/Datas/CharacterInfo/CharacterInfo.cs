using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "StageInfo", menuName = "ScriptableObject/CharacterInfo")]
public class CharacterInfo : ScriptableObject
{
    public int ID;
    public Player playerPrefab;
    public List<Equip> playerItems;
    public string characterName;
    public string description;
}