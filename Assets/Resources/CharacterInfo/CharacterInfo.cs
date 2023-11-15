using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "StageInfo", menuName = "ScriptableObject/CharacterInfo")]
public class CharacterInfo : ScriptableObject
{
    public Player playerPrefab;
    public List<Equip> playerItems;
    public string name;
    public string description;


}