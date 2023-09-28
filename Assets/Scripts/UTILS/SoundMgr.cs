using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundMgr : MonoSingleton<SoundMgr>
{
    void Awake()
    {
        DontDestroyOnLoad(gameObject);       
    }

    Dictionary<string, AudioClip> sounds = new Dictionary<string, AudioClip>();
    List<AudioSource> SFXPlayers = new List<AudioSource>();
    AudioSource BGMPlayer;

    public void Play(string name)
    {
        AudioClip clip;

        if(sounds.ContainsKey(name))
        {
            clip = sounds[name];
        }
        else
        {
            clip = Resources.Load<AudioClip>("SFX/" + name);
            sounds.Add(name, clip);
        }

        AudioSource audioSource = findAudioSource();

        audioSource.clip = clip;
        audioSource.Play();

    }

    AudioSource findAudioSource()
    {
        for(int i = 0; i < SFXPlayers.Count; i++ )
        {
            if (!SFXPlayers[i].isPlaying) return SFXPlayers[i];
        }

        GameObject temp = new GameObject();
        temp.transform.SetParent(transform);
        AudioSource audio = temp.AddComponent<AudioSource>();
        SFXPlayers.Add(audio);

        return audio;
    }
    public void PlayBGM(string name)
    {
        if(BGMPlayer == null)
        {
            GameObject temp = new GameObject();
            temp.transform.SetParent(transform);
            BGMPlayer = temp.AddComponent<AudioSource>();
            BGMPlayer.loop = true;
        }

        AudioClip clip;
        if (sounds.ContainsKey(name))
        {
            clip = sounds[name];
        }
        else
        {
            clip = Resources.Load<AudioClip>("BGM/" + name);
            sounds.Add(name, clip);
        }

        BGMPlayer.Stop();
        BGMPlayer.clip = clip;
        BGMPlayer.Play();
    }

}