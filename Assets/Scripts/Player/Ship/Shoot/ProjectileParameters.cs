using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Paramètres liés à un projectiles
/// </summary>
[System.Serializable]
public struct ProjectileParameters 
{
    /// <summary>
    /// Vitesse du projectile, pouvant être évolutive et/ou tirée entre deux nombres
    /// </summary>
    [Header("Constant Parameters")]
    [SerializeField] SpeedType speedType;
    [SerializeField] ShootType shootType;
    [SerializeField] ParticleSystem.MinMaxCurve projectileSpeed;
    /// <summary>
    /// Taille du projectile
    /// </summary>
    [SerializeField] float projectileSize;
    /// <summary>
    /// Durée de vie du projectile. Pour les boulders, il s'agit du temps que va mettre la projectile à arriver au terme de sa trajectoire
    /// </summary>
    [SerializeField] float projectileLifeTime;
    /// <summary>
    /// Durée de vie du projectile. Pour les boulders, il s'agit du temps que va mettre la projectile à arriver au terme de sa trajectoire
    /// </summary>
    public float GetTotalLifeTime
    {
        get
        {
            return projectileLifeTime;
        }
    }
    /// <summary>
    /// Durée de vie restante du projectile
    /// </summary>
    float currentLifeTime;
    public float GetCurrentLifeTime { get { return currentLifeTime; } }

    /// <summary>
    /// Coefficient déterminant à quel point la vitesse du tireur va avoir une influence sur le tir de ce projectile
    /// </summary>
    [SerializeField] float shooterVelocityInfluenceCoeff;
    /// <summary>
    /// Paramètres d'attaque liés à ce projectile
    /// </summary>
    [SerializeField] DamagesParameters damagesParameters;
    public DamagesParameters DmgParameters
    {
        get
        {
            return damagesParameters;
        }
    }

    /// <summary>
    /// Vélocité du tireur au moment où il a tiré ce projectile
    /// </summary>
    Vector3 shooterInfluence;

    /// <summary>
    /// Coefficient tiré à l'instantiement du projectile pour déterminer où sa vitesse se place sur la courbe évolutive tirée aléatoirement
    /// </summary>
    float randomSpeedCoeff;

    /// <summary>
    /// Direction actuelle du projectile
    /// </summary>
    [Header("Situational Parameters")]
    Vector3 projectileDirection;

    /// <summary>
    /// Affecte la direction actuelle du projectile
    /// </summary>
    /// <param name="dir">Direction du projectile</param>
    /// <param name="shooterVelocity">vélocité du tireur</param>
    public void SetProjectileDirection(Vector3 dir, Vector3 shooterVelocity)
    {
        projectileDirection = dir.normalized;
        shooterInfluence = shooterVelocity * shooterVelocityInfluenceCoeff;
    }

    #region Movements
    /// <summary>
    /// Initialisation des parametres internes
    /// </summary>
    public void SetUpParameters()
    {
        randomSpeedCoeff = Random.Range(0f, 1f);
        currentLifeTime = projectileLifeTime;
    }

    /// <summary>
    /// Renvoie la vélocité actuelle du projectile
    /// </summary>
    public Vector3 GetCurrentProjectileVelocity
    {
        get
        {
            if (Time.timeScale == 0)
                return Vector3.zero;

            float speed = 0;
            switch (projectileSpeed.mode)
            {
                case (ParticleSystemCurveMode.Constant):
                    speed = projectileSpeed.constant;
                    break;

                case (ParticleSystemCurveMode.TwoConstants):
                    speed = Mathf.Lerp(projectileSpeed.constantMin, projectileSpeed.constantMax, randomSpeedCoeff);
                    break;

                case (ParticleSystemCurveMode.Curve):
                    speed = GetEvolutiveSpeed();
                    break;

                case (ParticleSystemCurveMode.TwoCurves):
                    speed = GetEvolutiveSpeed();
                    break;

            }
            return projectileDirection * speed * 50 * Time.deltaTime / Time.timeScale + shooterInfluence;
        }
    }

    /// <summary>
    /// Renvoie la taille du projectile
    /// </summary>
    public float GetCurrentProjectileSize
    {
        get
        {
            return projectileSize;
        }
    }

    /// <summary>
    /// Renvoie la vitesse actuelle du projectile
    /// </summary>
    /// <returns>Vitesse actuelle du projectile</returns>
    public float GetEvolutiveSpeed()
    {
        if (projectileSpeed.mode == ParticleSystemCurveMode.Curve)
        {
            return projectileSpeed.curve.Evaluate(LifetimeProgression);
        }
        else if (projectileSpeed.mode == ParticleSystemCurveMode.TwoCurves)
        {
            return Mathf.Lerp(projectileSpeed.curveMin.Evaluate(LifetimeProgression), projectileSpeed.curveMax.Evaluate(LifetimeProgression), randomSpeedCoeff);
        }

        return projectileSpeed.constant;
        //return (int) speedType;
    }

    /// <summary>
    /// Renvoie vrai si ce projectile possède une vitesse évolutive
    /// </summary>
 /*   public bool HasEvolutiveSpeed
    {
        get
        {
            return projectileSpeed.mode == ParticleSystemCurveMode.Curve || projectileSpeed.mode == ParticleSystemCurveMode.TwoCurves;
        }
    }*/
    #endregion

    #region Lifetime
    /// <summary>
    /// Attribue une durée de vie à ce projectile
    /// </summary>
    /// <param name="lifetime">Durée de vie à affecter</param>
    public void SetLifeTime(float lifetime)
    {
        projectileLifeTime = lifetime;
        currentLifeTime = projectileLifeTime;
    }

    /// <summary>
    /// Actualise la durée de vie actuelle de ce projectile
    /// </summary>
    public void UpdateLifetime()
    {
        if (currentLifeTime > 0)
            currentLifeTime -= Time.deltaTime;
        else if (currentLifeTime < 0)
            currentLifeTime = 0;
    }

    /// <summary>
    /// Vrai si la durée de vie du projectile a atteint 0
    /// </summary>
    public bool LifetimeEnded
    {
        get
        {
            return currentLifeTime == 0;
        }
    }

    /// <summary>
    /// Renvoie une valeur entre 0 et 1 correspondant à la progression de la durée de vie (0 : lancement du projectile ; 1 : fin du projectile)
    /// </summary>
    public float LifetimeProgression
    {
        get
        {
            return 1 - currentLifeTime/projectileLifeTime;
        }
    }
    #endregion

    public enum ShootType
    {
        Normal, Shotgun
    }

    public enum SpeedType
    {
        Fast = 40, Average=30, Slow=20
    }
}
