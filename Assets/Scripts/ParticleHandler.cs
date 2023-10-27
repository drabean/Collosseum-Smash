using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleHandler : MonoBehaviour
{
    [SerializeField] ParticleSystem particle;
    public void PlayParticle()
    {
        particle.Play();
    }

    public void StopParticle()
    {
        Debug.Log("STOP");
        particle.Stop();
    }
}
