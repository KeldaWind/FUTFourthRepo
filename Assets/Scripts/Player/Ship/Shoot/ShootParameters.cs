using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

/// <summary>
/// Paramètres liés à un tir
/// </summary>
[System.Serializable]
public struct ShootParameters 
{   
    /// <summary>
    /// Ensemble des salves effectuées lors de ce tis
    /// </summary>
    [Header("Salvos")]
    [SerializeField] Salvo[] shootSalvos;

    /// <summary>
    /// Temps d'attente entre deux salves de ce tir
    /// </summary>
    [Header("Salvos Management")]
    [SerializeField] float waitTimeBeforeTwoSalvo;
    /// <summary>
    /// Temps d'attente entre deux salves de ce tir
    /// </summary>
    public float GetTimeBewteenSalvos
    {
        get
        {
            return waitTimeBeforeTwoSalvo;
        }
    }

    /// <summary>
    /// Index actuel de progression des salves
    /// </summary>
    int currentSalvoIndex;
    public int GetCurrentSalvoIndex
    {
        get
        {
            return currentSalvoIndex;
        }
    }

    /// <summary>
    /// Renvoie la prochaine salve qui sera effectuée par ce tir
    /// </summary>
    public Salvo GetCurrentSalvo
    {
        get
        {
            return shootSalvos[currentSalvoIndex];
        }
    }

    /// <summary>
    /// Augmente de 1 l'index de progression des salves
    /// </summary>
    public void IncreaseSalvoIndex()
    {
        currentSalvoIndex++;
    }

    /// <summary>
    /// Vrai si ce tir a effectué toutes ses salves
    /// </summary>
    public bool Over
    {
        get
        {
            if (shootSalvos != null)
                return currentSalvoIndex >= shootSalvos.Length;
            else
                return true;
        }
    }

    [Header("Tag")]
    [SerializeField] AttackTag projectileTag;
    public AttackTag GetProjectileTag
    {
        get
        {
            return projectileTag;
        }
    }

    [Header("Catapult")]
    [SerializeField] float catapultMinDistance;
    public float GetCatapultMinDistance
    {
        get
        {
            return catapultMinDistance;
        }
    }

    [SerializeField] float catapultMaxDistance;
    public float GetCatapultMaxDistance
    {
        get
        {
            return catapultMaxDistance;
        }
    }

    [Header("Feedback")]
    [SerializeField] Sound shootSound;
    public Sound GetShootSound { get { return shootSound; } }
}

/// <summary>
/// Salve de projectiles qui vont être tirés en même temps
/// </summary>
[System.Serializable]
public struct Salvo
{
    /// <summary>
    /// Prefab du projectile qui va être tiré
    /// </summary>
    [Header("Basic Parameters")]
    /*[SerializeField] Projectile projectilePrefab;
    /// <summary>
    /// Prefab du projectile qui va être tiré
    /// </summary>
    public Projectile GetProjPrefab
    {
        get
        {
            return projectilePrefab;
        }
    }*/
    [SerializeField] ProjectilePoolTag projectileType;
    public ProjectilePoolTag GetProjectileType
    {
        get
        {
            return projectileType;
        }
    }

    /// <summary>
    /// Nombre de projectiles tirés par direction de tir
    /// </summary>
    [SerializeField] int numberOfShotProjectiles;
    /// <summary>
    /// Nombre de projectiles tirés par direction de tir
    /// </summary>
    public int GetNumberOfProjectiles
    {
        get
        {
            return numberOfShotProjectiles;
        }
    }

    /// <summary>
    /// Paramètres qui seront appliqués au projectile tiré
    /// </summary>
    [SerializeField] ProjectileParameters projectileParameters;
    /// <summary>
    /// Paramètres qui seront appliqués au projectile tiré
    /// </summary>
    public ProjectileParameters GetProjectileParameters
    {
        get
        {
            return projectileParameters;
        }
    }

    /// <summary>
    /// Niveau d'imprécision des projectiles tirés (différence d'angle pour les tirs droit, différence de position pour les tirs en cloche)
    /// </summary>
    [SerializeField] float imprecisionParameter;
    /// <summary>
    /// Niveau d'imprécision des projectiles tirés (différence d'angle pour les tirs droit, différence de position pour les tirs en cloche)
    /// </summary>
    public float GetImprecisionParameter
    {
        get
        {
            return imprecisionParameter;
        }
    }

    [SerializeField] float projectilesSpacing;
    public float GetProjectilesSpacing
    {
        get
        {
            return (projectilesSpacing != 0) ? projectilesSpacing : 5;
        }
    }

    /// <summary>
    /// Nombre de directions vers lesquelles des tirs vont être effectués
    /// </summary>
    [Header("Multiple Directions")]
    [SerializeField] int numberOfDirections;
    /// <summary>
    /// Nombre de directions vers lesquelles des tirs vont être effectués
    /// </summary>
    public int GetNumberOfDirections
    {
        get
        {
            return numberOfDirections;
        }
    }

    /// <summary>
    /// Angle entre chaque direction dans laquelle des projectiles seront tirés
    /// </summary>
    [SerializeField] float angleBetweenDirections;
    /// <summary>
    /// Angle entre chaque direction dans laquelle des projectiles seront tirés
    /// </summary>
    public float GetAngleBetweenDirections
    {
        get
        {
            return angleBetweenDirections;
        }
    }

    /// <summary>
    /// Angle total couvert par toutes les directions
    /// </summary>
    public float GetTotalAngle
    {
        get
        {
            return angleBetweenDirections * Mathf.Clamp(numberOfDirections - 1,0, Mathf.Infinity);
        }
    }
}
 