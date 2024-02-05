using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "SFX", menuName = "ScriptableObject/SFX")]
public class Sound : ScriptableObject
{
    public AudioClip clip;
    public float volume = 1;
}
