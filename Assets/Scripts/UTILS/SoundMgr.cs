using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundMgr : MonoSingleton<SoundMgr>
{
    void Awake()
    {
        DontDestroyOnLoad(gameObject);

        BGMvolume = SaveDatas.Inst.setting.BGMVolume;
        SFXvolume = SaveDatas.Inst.setting.SFXVolume;

        GameObject BGM = new GameObject();
        BGM.transform.SetParent(transform);
        BGM.name = "BGMPlayer";
        BGMPlayer = BGM.AddComponent<AudioSource>();
        BGMPlayer.loop = true;
        BGMPlayer.volume = BGMvolume;

        GameObject Intro = new GameObject();
        Intro.transform.SetParent(transform);
        Intro.name = "IntroPlayer";
        IntroPlayer = Intro.AddComponent<AudioSource>();
        IntroPlayer.volume = BGMvolume;

        for(int i = 0; i < 3; i++)
        {
            GameObject temp = new GameObject();
            temp.transform.SetParent(transform);
            AudioSource audio = temp.AddComponent<AudioSource>();
            SFXPlayers.Add(audio);
        }
    }

    Dictionary<string, Sound> sounds = new Dictionary<string, Sound>();
    List<AudioSource> SFXPlayers = new List<AudioSource>();
    float SFXvolume = 1f;

    AudioSource BGMPlayer;
    float BGMvolume = 1f;
    AudioSource IntroPlayer;
    #region SFX
    public void Play(string name)
    {
        Sound sfx;

        if(sounds.ContainsKey(name))
        {
            sfx = sounds[name];
        }
        else
        {
            sfx = Resources.Load<Sound>("SFX/" + name);
            sounds.Add(name, sfx);
        }

        AudioSource audioSource = findAudioSource();

        audioSource.clip = sfx.clip;
        audioSource.volume = sfx.volume * SFXvolume;
        audioSource.Play();

    }

    AudioSource findAudioSource()
    {
        for(int i = 0; i < SFXPlayers.Count; i++ )
        {
            if (!SFXPlayers[i].isPlaying)
            {
                SFXPlayers[i].volume = SFXvolume;
                return SFXPlayers[i];
            }
        }

        GameObject temp = new GameObject();
        temp.name = "SFXPlayer";
        temp.transform.SetParent(transform);
        AudioSource audio = temp.AddComponent<AudioSource>();
        SFXPlayers.Add(audio);
        return audio;
    }

    public void ChangeSFXVolume(float volume)
    {
        SFXvolume = volume;
        foreach(AudioSource s in SFXPlayers)
        {
            s.volume = SFXvolume;
        }
    }
    #endregion
    #region BGM
    public void PlayBGM(string name)
    {
        AudioClip sfx;

        sfx = Resources.Load<AudioClip>("BGM/" + name);

        BGMPlayer.Stop();
        BGMPlayer.volume = BGMvolume;
        BGMPlayer.clip = sfx;
        BGMPlayer.Play();
    }

    public void PlayBGM(AudioClip Intro, AudioClip BGM)
    {
        StartCoroutine(co_PlayBGM(Intro, BGM));

    }

    public IEnumerator co_PlayBGM(AudioClip Intro, AudioClip BGM)
    {
        BGMPlayer.Stop();
        if (Intro != null)
        {
            IntroPlayer.clip = Intro;

            IntroPlayer.Play();
            while(IntroPlayer.isPlaying)
            {
                yield return null;
            }
        }

        BGMPlayer.volume = BGMvolume;
        BGMPlayer.clip = BGM;
        BGMPlayer.Play();
    }
    public IEnumerator co_BGMFadeOut(float duration = 1)
    {
        float progress = 1f;

        while(progress >= 0)
        {
            BGMPlayer.volume = progress * BGMvolume;
            progress -= Time.deltaTime / duration;
            yield return null;
        }
        BGMPlayer.Stop();
    }


    public void StopBGM()
    {
        if (BGMPlayer == null) return;
        BGMPlayer.Stop();
    }

    public void ChangeBGMVolume(float volume)
    {
        BGMvolume = volume;
        BGMPlayer.volume = volume;
        IntroPlayer.volume = volume;
    }
    #endregion
}
