using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosion : MonoBehaviour {
    /// <summary>
    /// The parameters of this explosion
    /// </summary>
    [SerializeField] ExplosionParameters currentExplosionParameters;
    public ExplosionParameters CurrentExplParameters
    {
        get
        {
            return currentExplosionParameters;
        }
    }
    [SerializeField] ExplosionHitbox explosionHitbox;

    [Header("Rendering")]
    //[SerializeField] Transform particlesParent;
    [SerializeField] ParticleSystem explosionParticleSystem;
    [SerializeField] Vector3 normalizedParticlesSize;

    [Header("Sounds")]
    [SerializeField] AudioSource audioSource;
    [SerializeField] Sound explosionSound;

    /// <summary>
    /// Call this function to make this explosion unefficient and invisible.
    /// </summary>
    public void SetOffExplosion()
    {
        EndExplosion();
    }

    /// <summary>
    /// Sets up the parameters of this explosion with the inputed explosion parameters.
    /// </summary>
    /// <param name="newExplosionParameters"></param>
    public void SetUpExplosionParameters(ExplosionParameters newExplosionParameters)
    {
        currentExplosionParameters = newExplosionParameters;

        currentExplosionParameters.SetUpDurations();

        gameObject.SetActive(true);
        explosionHitbox.gameObject.SetActive(true);

        explosionHitbox.transform.localScale = Vector3.zero;

        /*if (particlesParent != null)
            particlesParent.localScale = currentExplosionParameters.explosionMaxSize * Vector3.one;*/

        if (explosionParticleSystem != null)
        {
            explosionParticleSystem.transform.localScale = normalizedParticlesSize * currentExplosionParameters.explosionMaxSize;
            explosionParticleSystem.Play();
        }

        if (audioSource != null)
            audioSource.PlaySound(explosionSound);
    }

    public void SetAttackTag(AttackTag attackTag)
    {
        explosionHitbox.SetAttackTag(attackTag);
    }

    void Update()
    {
        if (currentExplosionParameters.Expanding || currentExplosionParameters.Persisting)
            UpdateExplosion();

        if (waitingToReturn)
        {
            if (ReadyToBeReturnedToPool)
            {
                gameObject.SetActive(false);
                GameManager.gameManager.PoolManager.ReturnExplosion(this);
                waitingToReturn = false;
            }
        }
    }

    /// <summary>
    /// Updates the explosion sizeand timers.
    /// </summary>
    public void UpdateExplosion()
    {
        explosionHitbox.transform.localScale = currentExplosionParameters.UpdateAndGetCurrentExplosionScale(this);
    }

    /// <summary>
    /// Called at the end of the persisting phase. Disable the hitbox, but stills plays the end of the animation/particules.
    /// </summary>
    public void EndExplosion()
    {
        //gameObject.SetActive(false);
        explosionHitbox.gameObject.SetActive(false);
        waitingToReturn = true;
        //GameManager.gameManager.PoolManager.ReturnExplosion(this);
    }

    #region Pooling
    [Header("Pooling")]
    [SerializeField] ExplosionPoolTag explosionTag;
    public ExplosionPoolTag GetExplosionPoolTag { get { return explosionTag; } }

    bool waitingToReturn;
    public bool ReadyToBeReturnedToPool
    {
        get
        {
            return !audioSource.isPlaying && !explosionParticleSystem.IsAlive();
        }
    }
    #endregion 
}

/// <summary>
/// The parameters of an explosion : attack parameters, size and durations.
/// </summary>
[System.Serializable]
public struct ExplosionParameters
{
    public bool IsNull
    {
        get
        {
            return explosionMaxSize == 0 || (explosionExpandTime == 0 && explosionPersistanceTime == 0);
        }
    }

    #region Constant parameters
    /// <summary>
    /// The parameters of the attack created by the explosion
    /// </summary>
    public AttackParameters explosionAttackParameters;

    /// <summary>
    /// The size that the explosion will be at its apogee
    /// </summary>
    public float explosionMaxSize;

    /// <summary>
    /// The time between the creation of the explosion and its apogee
    /// </summary>
    public float explosionExpandTime;

    /// <summary>
    /// The time between the apogee of the explosion and its end
    /// </summary>
    public float explosionPersistanceTime;
    #endregion

    #region Variable parameters : Expand
    /// <summary>
    /// The currently remaining time of the expand phase
    /// </summary>
    [HideInInspector]
    public float explosionCurrentExpandTime;

    public void SetUpDurations()
    {
        explosionCurrentExpandTime = explosionExpandTime;
        explosionCurrentPersistanceTime = explosionPersistanceTime;
        expanded = false;
        persisted = false;
        globalExplosionLifeTime = explosionExpandTime + explosionPersistanceTime;
    }

    float globalExplosionLifeTime;

    /// <summary>
    /// Returns if the explosion is currenlty expanding, based on the expand timer.
    /// </summary>
    public bool Expanding
    {
        get
        {
            return explosionCurrentExpandTime != 0;
        }
    }

    /// <summary>
    /// The progression of the expand, between 0 and 1, 0 beeing the beginning and 1 being the end.
    /// </summary>
    public float ExpandingProgression
    {
        get
        {
            return 1 - (explosionCurrentExpandTime/explosionExpandTime);
        }
    }
    #endregion

    #region Variable parameters : Persistance
    /// <summary>
    /// The currently remaining time of the Peristance phase
    /// </summary>
    [HideInInspector]
    public float explosionCurrentPersistanceTime;

    /// <summary>
    /// Returns if the explosion is currenlty persisting, based on the expand and persisting timers.
    /// </summary>
    public bool Persisting
    {
        get
        {
            return explosionCurrentExpandTime == 0 && explosionCurrentPersistanceTime != 0;
        }
    }

    /// <summary>
    /// The progression of the persistance, between 0 and 1, 0 beeing the beginning and 1 being the end.
    /// </summary>
    public float PersistingProgression
    {
        get
        {
            return 1 - (explosionCurrentPersistanceTime / explosionPersistanceTime);
        }
    }
    #endregion

    bool expanded;
    bool persisted;
    public Vector3 UpdateAndGetCurrentExplosionScale(Explosion refferedExplosion)
    {
        Vector3 currentExplosionScale = new Vector3();

        if (Expanding)
        {
            if (explosionCurrentExpandTime > 0)
                explosionCurrentExpandTime -= Time.deltaTime;
            else if (explosionCurrentExpandTime <= 0 && !expanded)
            {
                explosionCurrentExpandTime = 0;
                expanded = true;
            }

            currentExplosionScale = Vector3.Lerp(Vector3.zero, Vector3.one * explosionMaxSize, ExpandingProgression);
        }
        else if (Persisting)
        {
            if (explosionCurrentPersistanceTime > 0)
            {
                explosionCurrentPersistanceTime -= Time.deltaTime;
                currentExplosionScale = Vector3.one * explosionMaxSize;
            }
            else if (explosionCurrentPersistanceTime <= 0 && !persisted)
            {
                explosionCurrentPersistanceTime = 0;
                refferedExplosion.EndExplosion();
                persisted = true;
                return Vector3.zero;
            }

        }

        if (globalExplosionLifeTime > 0)
            globalExplosionLifeTime -= Time.deltaTime;
        else if (globalExplosionLifeTime <= 0 && !persisted)
        {
            refferedExplosion.EndExplosion();
            persisted = true;
        }

        return currentExplosionScale;
    }
}