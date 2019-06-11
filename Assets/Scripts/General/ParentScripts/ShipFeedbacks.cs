using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShipFeedbacks : MonoBehaviour
{
    public bool ReadyToBeReturnedToPool
    {
        get
        {
            return !shipAudioSource.isPlaying && !deathParticles.isPlaying;
        }
    }

    [Header("Feedbacks")]
    [SerializeField] protected AudioSource shipAudioSource;
    [SerializeField] protected ParticleSystem damagesParticles;
    public void PlayDamageFeedback()
    {
        if (deathParticles != null)
            if (deathParticles.isPlaying)
                return;

        if (damagesParticles != null)
            damagesParticles.Play();
    }

    [SerializeField] protected ParticleSystem deathParticles;
    [SerializeField] Sound deathSound;
    public void PlayDeathFeedback()
    {
        if (slowParticles != null)
            slowParticles.Stop();

        if (slowParticles != null)
            damagesParticles.Stop();

        if (deathParticles != null)
            deathParticles.Play();

        if (shipAudioSource != null)
            shipAudioSource.PlaySound(deathSound);
    }

    [SerializeField] protected ParticleSystem stunParticles;
    public void PlayStunFeedback()
    {
        if (stunParticles != null)
            stunParticles.Play();
    }

    public void StopStunFeedback()
    {
        if (stunParticles != null)
            stunParticles.Stop();
    }

    [SerializeField] protected ParticleSystem slowParticles;

    public void PlaySlowingFeedback()
    {
        if (deathParticles != null)
            if (deathParticles.isPlaying)
                return;

        if (slowParticles != null)
        {
            if (slowParticles.gameObject.activeInHierarchy)
            {
                if (!slowParticles.isPlaying)
                    slowParticles.Play();
            }
            else
                slowParticles.gameObject.SetActive(true);
        }
    }

    public void StopSlowingFeedback()
    {
        if (slowParticles != null)
            slowParticles.Stop();
    }
}
