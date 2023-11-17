using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "StageInfo", menuName = "ScriptableObject/CharacterInfo")]
public class CharacterInfo : ScriptableObject
{
    public Player playerPrefab;
    public List<Equip> playerItems;
    public string characterName;
    public string description;
    public STATUS stat;


}