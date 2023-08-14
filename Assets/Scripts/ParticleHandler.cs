using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleHandler : MonoBehaviour
{
    [SerializeField] ParticleSystem particle;
    public void Play()
    {
        particle.Play();
    }

    public void Stop()
    {
        particle.Stop();
    }
}
