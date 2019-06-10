using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyParticlesOnEnd : MonoBehaviour
{
    [SerializeField] ParticleSystem particles;
    private void Update()
    {
        if (!particles.isPlaying)
            Destroy(gameObject);
    }
}
