using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the explosions of the game, alowing to create explosions.
/// </summary>
[System.Serializable]
public class ExplosionsManager {
    /// <summary>
    /// The prefab of all the explosions of the game
    /// </summary>
    [Header("Constants")]
    [SerializeField] Explosion explosionPrefab;

    /// <summary>
    /// The number of explosions that will be instantiated at the beginning of the game, and constitute the pool of explosions.
    /// </summary>
    [SerializeField] int explosionPoolSize;
    /// <summary>
    /// The object that will be used as parent for the objects, in order to sort things in the editor.
    /// </summary>
    [SerializeField] Transform poolParent;

    /// <summary>
    /// The pool of explosion.
    /// </summary>
    Queue<Explosion> explosionsPool;

    /// <summary>
    /// Call this function on start to set up all the basical values.
    /// </summary>
    public void SetUp()
    {
        explosionsPool = new Queue<Explosion>();

        for(int i = 0; i < explosionPoolSize; i++)
        {
            Explosion newExplosion = Object.Instantiate(explosionPrefab);

            newExplosion.SetOffExplosion();

            newExplosion.transform.parent = poolParent;

            explosionsPool.Enqueue(newExplosion);
        }
    }

    /// <summary>
    /// Creates an explosion with the inputed parameters at the inputed position.
    /// </summary>
    /// <param name="explosionParameters">The parameters of the explosion you want to create</param>
    /// <param name="explosionPosition">The position where you want to create the explosion</param>
    /// <returns></returns>
    public Explosion CreateExplosion(ExplosionParameters explosionParameters, Vector3 explosionPosition)
    {
        Explosion newExplosion = explosionsPool.Dequeue();


        newExplosion.transform.position = explosionPosition;

        newExplosion.SetUpExplosionParameters(explosionParameters);

        newExplosion.gameObject.SetActive(true);

        explosionsPool.Enqueue(newExplosion);

        /*GameManager.gameManager.ScreenShkManager.StartScreenshake(GetExplosionScreenshakeParameters(explosionParameters.explosionMaxSize));
        GameManager.gameManager.FlshManager.StartFlash(GetExplosionFlashParameters(explosionParameters.explosionMaxSize));*/

        return newExplosion;
    }

    public Explosion CreateExplosion(ExplosionParameters explosionParameters, Vector3 explosionPosition, Color explosionColor, Color sparksMinColor, Color sparksMaxColor, Color ringColor)
    {
        Explosion newExplosion = explosionsPool.Dequeue();

        newExplosion.transform.position = explosionPosition;

        newExplosion.SetUpExplosionParameters(explosionParameters);

        newExplosion.gameObject.SetActive(true);

        explosionsPool.Enqueue(newExplosion);

        /*GameManager.gameManager.ScreenShkManager.StartScreenshake(GetExplosionScreenshakeParameters(explosionParameters.explosionMaxSize));
        GameManager.gameManager.FlshManager.StartFlash(GetExplosionFlashParameters(explosionParameters.explosionMaxSize));*/

        return newExplosion;
    }

    [Header("Feedback")]
    [SerializeField] float minExplosionRefValue;
    [SerializeField] ScreenshakeParameters minExplosionScreenshakeParameters;
    [SerializeField] float minExplosionVolume;
    [SerializeField] float maxExplosionRefValue;
    [SerializeField] ScreenshakeParameters maxExplosionScreenshakeParameters;
    [SerializeField] float maxExplosionVolume;

    public ScreenshakeParameters GetExplosionScreenshakeParameters(float explosionSize)
    {
        float coeff = Mathf.Clamp((explosionSize - minExplosionRefValue) / (maxExplosionRefValue - minExplosionRefValue), 0, 1);

        return ScreenshakeParameters.Lerp(minExplosionScreenshakeParameters, maxExplosionScreenshakeParameters, coeff);
    }

    /*public FlashParameters GetExplosionFlashParameters(float explosionSize)
    {
        float coeff = Mathf.Clamp((explosionSize - minExplosionRefValue) / (maxExplosionRefValue - minExplosionRefValue), 0, 1);

        return FlashParameters.Lerp(minExplosionFlashParameters, maxExplosionFlashParameters, coeff);
    }*/

    public float GetExplosionVolume(float explosionSize)
    {
        float coeff = Mathf.Clamp((explosionSize - minExplosionRefValue) / (maxExplosionRefValue - minExplosionRefValue), 0, 1);

        return Mathf.Lerp(minExplosionVolume, maxExplosionVolume, coeff);
    }
}
