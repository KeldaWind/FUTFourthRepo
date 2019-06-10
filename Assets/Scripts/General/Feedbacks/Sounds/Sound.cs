using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public AudioClip clip;
    public float volume = 1;
    public bool loop;
    public float minPitch = 1;
    public float maxPitch = 1;
}

