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
    [SerializeField] AudioSource shipAudioSource;
    [SerializeField] ParticleSystem damagesParticles;
    [SerializeField] ScreenshakeParameters damageScreenshake;
    public void PlayDamageFeedback()
    {
        if (deathParticles != null)
            if (deathParticles.isPlaying)
                return;

        if (damagesParticles != null)
            damagesParticles.Play();

        if (damageScreenshake.Force != 0 && damageScreenshake.Duration != 0)
            GameManager.gameManager.ScrshkManager.StartScreenshake(damageScreenshake);
    }

    [SerializeField] ParticleSystem deathParticles;
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

    [SerializeField] ParticleSystem stunParticles;
    public void PlayStunFeedback()
    {
        if (stunParticles != null)
        {
            if (!stunParticles.gameObject.activeInHierarchy)
                stunParticles.gameObject.SetActive(true);
            else
                stunParticles.Play();
        }
    }

    public void StopStunFeedback()
    {
        if (stunParticles != null)
            stunParticles.Stop();
    }

    [SerializeField] ParticleSystem slowParticles;

    public void DesactivateSlowParticles()
    {
        if (slowParticles != null)
            slowParticles.gameObject.SetActive(false);
    }

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

    [SerializeField] ParticleSystem rammingParticleSystem;
    public void PlayRammingParticles()
    {
        if (rammingParticleSystem != null)
        {
            if (!rammingParticleSystem.gameObject.activeInHierarchy)
                rammingParticleSystem.gameObject.SetActive(true);
            else
                rammingParticleSystem.Play();
        }
    }

    public void StopRammingParticles()
    {
        if (rammingParticleSystem != null)
            rammingParticleSystem.Stop();
    }
}
