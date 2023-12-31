using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundMgr : MonoSingleton<SoundMgr>
{
    void Awake()
    {
        DontDestroyOnLoad(gameObject);  
        
        GameObject BGM = new GameObject();
        BGM.transform.SetParent(transform);
        BGM.name = "BGMPlayer";
        BGMPlayer = BGM.AddComponent<AudioSource>();
        BGMPlayer.loop = true;

        GameObject Intro = new GameObject();
        Intro.transform.SetParent(transform);
        Intro.name = "IntroPlayer";
        IntroPlayer = BGM.AddComponent<AudioSource>();

        for(int i = 0; i < 3; i++)
        {
            GameObject temp = new GameObject();
            temp.transform.SetParent(transform);
            AudioSource audio = temp.AddComponent<AudioSource>();
            SFXPlayers.Add(audio);
        }

    }

    Dictionary<string, AudioClip> sounds = new Dictionary<string, AudioClip>();
    List<AudioSource> SFXPlayers = new List<AudioSource>();
    AudioSource BGMPlayer;
    AudioSource IntroPlayer;
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
        BGMPlayer.volume = 1;
        BGMPlayer.clip = clip;
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

        BGMPlayer.volume = 1;
        BGMPlayer.clip = BGM;
        BGMPlayer.Play();
    }
    public IEnumerator co_BGMFadeOut(float duration = 1)
    {
        float progress = 1f;

        while(progress >= 0)
        {
            BGMPlayer.volume = progress;
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
}
