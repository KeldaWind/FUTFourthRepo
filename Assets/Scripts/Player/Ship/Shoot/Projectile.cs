using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Projectile pouvant être tiré dans une direction
/// </summary>
public class Projectile : MonoBehaviour
{
    /// <summary>
    /// Paramètres de tir appliqués à ce projectile
    /// </summary>
    protected ProjectileParameters projectileParameters;

    [SerializeField] protected ProjectilePoolTag projectilePoolTag;
    public ProjectilePoolTag GetProjectilePoolTag
    {
        get
        {
            return projectilePoolTag;
        }
    }

    public bool IsBoulder
    {
        get
        {
            return (this as ProjectileBoulder) != null;
        }
    }

    /// <summary>
    /// Rigidbody de ce projectile
    /// </summary>
    [Header("References")]
    [SerializeField] protected Rigidbody projectileBody;
    /// <summary>
    /// Parent du projectile servant à contrôler sa taille globale
    /// </summary>
    [SerializeField] protected Transform projectileSizeParent;
    /// <summary>
    /// Preview de tir de ce projectile
    /// </summary>
    [SerializeField] protected PreviewPoolTag shootPreviewType;
    /// <summary>
    /// Preview de tir de ce projectile
    /// </summary>
    public PreviewPoolTag GetShootPreviewType
    {
        get
        {
            return shootPreviewType;
        }
    }

    #region Source
    protected Ship sourceShip;
    public Ship GetSourceShip { get { return sourceShip; } }
    protected Turret sourceTurret;
    public Turret GetSourceTurret { get { return sourceTurret; } }
    public IDamageReceiver GetSourceDamageReceiver
    {
        get
        {
            if (sourceShip != null)
                return sourceShip.GetShipDamageReceiver;
            else if (sourceTurret != null)
                return sourceTurret.GetTurretDamageReceiver;
            else
                return null;
        }
    }
    #endregion


    /// <summary>
    /// Particules jouées lorsque ce projectile finit sa course
    /// </summary>
    [Header("Feedbacks")]
    [SerializeField] protected Renderer projectileRenderer;
    [SerializeField] protected ParticleSystem explosionParticles;
    [SerializeField] protected ParticleSystem onLifeTimeEndedParticles;

    #region Projectile Rotation
    [Header("Feedbacks : Air Projectile Rotation")]
    [SerializeField] float rotationSpeed;
    float baseYRotation;
    float currentXRotation;

    public virtual Vector3 GetProjectileDirection
    {
        get
        {
            return projectileParameters.GetCurrentProjectileVelocity.normalized;
        }
    }

    public virtual float GetProjectileAfterLandingSpeed
    {
        get
        {
            return projectileParameters.GetEvolutiveSpeed() / 3;
        }
    }

    public void SetUpAirRotation()
    {
        Vector3 moveDirection = GetProjectileDirection;
        moveDirection.y = 0;
        moveDirection.Normalize();
        baseYRotation = Mathf.Atan2(moveDirection.x, moveDirection.z) * Mathf.Rad2Deg;

        currentXRotation = 0;

        transform.rotation = Quaternion.Euler(new Vector3(currentXRotation, baseYRotation, 0));
    }

    public void UpdateAirRotation()
    {
        currentXRotation += rotationSpeed * Time.deltaTime;

        transform.rotation = Quaternion.Euler(new Vector3(currentXRotation, baseYRotation, 0));
    }

    [Header("Feedbacks : Floating Projectile Feedback")]
    [SerializeField] float floatingSpeed = 0.4f;
    float endFloatingSpeed;
    float currentfloatingSpeed = 0;
    [SerializeField] float maxFloatingOffset = 3f;
    [SerializeField] float minFloatingOffset = 0.5f;
    float currentMaxFloatingOffset;
    [SerializeField] float floatingDesceleration = 1.5f;
    [SerializeField] AnimationCurve floatingCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);
    [SerializeField] AnimationCurve floatingMoveCurve;
    float floatingCounter;
    Vector3 baseFloatingPosition;
    [SerializeField] float floatingMoveDuration;
    float remainingFloatingMoveDuration;

    #region Floating Rotation
    float targetFloatingRotation;
    float floatingRotSpeedAcceleration;
    float floatingRotSpeedDesceleration;
    #endregion

    public void SetUpFloatingMove()
    {
        remainingFloatingMoveDuration = floatingMoveDuration;
        floatingCurve.preWrapMode = WrapMode.PingPong;
        floatingCurve.postWrapMode = WrapMode.PingPong;
        currentMaxFloatingOffset = maxFloatingOffset;
        floatingCounter = -0.5f;
        baseFloatingPosition = transform.position;

        floatingMoveCurve = new AnimationCurve().SetAsFastInSlowOutDescreasingCurve();

        float currentXRotation = transform.rotation.eulerAngles.x;
        if (currentXRotation > 90 && currentXRotation < 270)
            targetFloatingRotation = 180;
        else if (currentXRotation <= 90)
            targetFloatingRotation = 0;
        else if (currentXRotation >= 270)
        {
            targetFloatingRotation = 0;
            transform.Rotate(new Vector3(180, 0, 0));
        }

        endFloatingSpeed = floatingSpeed / 3;
        currentfloatingSpeed = floatingSpeed;
    }

    public void UpdateFloatingMove()
    {
        if (currentMaxFloatingOffset > minFloatingOffset)
            currentMaxFloatingOffset -= Time.deltaTime * floatingDesceleration;
        else if (currentMaxFloatingOffset < minFloatingOffset)
            currentMaxFloatingOffset = minFloatingOffset;

        if (currentMaxFloatingOffset == minFloatingOffset)
        {
            currentfloatingSpeed = Mathf.Lerp(currentfloatingSpeed, endFloatingSpeed, 0.1f);
        }

        floatingCounter += Time.deltaTime * currentfloatingSpeed;

        float floatCoeff = floatingCurve.Evaluate(floatingCounter);
        floatCoeff = (floatCoeff * 2) - 1;

        float floatingHeight = floatCoeff * currentMaxFloatingOffset;
        Vector3 newPos = new Vector3(transform.position.x, 0, transform.position.z) + new Vector3(0, floatingHeight + baseFloatingPosition.y, 0);
        transform.position = newPos;
        //transform.position = baseFloatingPosition + new Vector3(0, floatingHeight, 0);

        if (remainingFloatingMoveDuration != 0)
        {
            Vector3 direction = GetProjectileDirection;
            direction.y = 0;
            direction.Normalize();

            if (remainingFloatingMoveDuration > 0)
            {
                remainingFloatingMoveDuration -= Time.deltaTime;
                baseFloatingPosition += direction * GetProjectileAfterLandingSpeed * floatingMoveCurve.Evaluate(1 - remainingFloatingMoveDuration / floatingMoveDuration) * Time.deltaTime;
            }
            else if (remainingFloatingMoveDuration < 0)
                remainingFloatingMoveDuration = 0;
        }

        Vector3 currentEulerRotation = transform.rotation.eulerAngles;
        float newXRotation = Mathf.Lerp(currentEulerRotation.x, targetFloatingRotation, 0.1f);
        transform.rotation = Quaternion.Euler(newXRotation, currentEulerRotation.y, currentEulerRotation.z);
    }
    #endregion

    #region Water Stream Movements
    [Header("Stream Management")]
    [SerializeField] float streamCoeff = 0.3f;
    [SerializeField] float streamMultiplier = 3;
    bool beingStreamed;
    Vector3 currentStreamForce;
    float streamInfluence;

    public void StartStreamForce(Vector3 streamForce)
    {
        beingStreamed = true;
        currentStreamForce = streamForce;
    }

    public void UpdateStreamForce(Vector3 streamForce)
    {
        currentStreamForce = streamForce;
    }

    public void StopStreamForce(Vector3 streamForce)
    {
        beingStreamed = false;
        currentStreamForce = streamForce;
    }

    public Vector3 GetCurrentStreamForce
    {
        get
        {
            return streamInfluence * currentStreamForce;
        }
    }

    public void UpdateStreamInfluenceValue()
    {
        projectileBody.velocity = GetCurrentStreamForce * streamMultiplier;

        if (beingStreamed)
        {
            if (streamInfluence < 1)
                streamInfluence = Mathf.Lerp(streamInfluence, 1, streamCoeff);

            if (streamInfluence > 0.99f)
                streamInfluence = 1;
        }
        else
        {
            if (streamInfluence > 0)
                streamInfluence = Mathf.Lerp(streamInfluence, 0, streamCoeff);

            if (streamInfluence < 0.01f)
                streamInfluence = 0;
        }
    }
    #endregion

    public void SetSource(Ship ship)
    {
        sourceShip = ship;
    }

    public void SetSource(Turret turret)
    {
        sourceTurret = turret;
    }

    /// <summary>
    /// Tire le projectile en lui appliquant différents paramètres
    /// </summary>
    /// <param name="newParam">Paramètres de tirs appliqués à ce projectile</param>
    /// <param name="direction">Direction appliquée à ce projectile</param>
    /// <param name="currentShipVelocity">Vélocité actuelle su tireur</param>
    public virtual void ShootProjectile(ProjectileParameters newParam, Vector3 direction, Vector3 currentShipVelocity)
    {
        gameObject.SetActive(true);
        projectileParameters = newParam;
        projectileParameters.SetProjectileDirection(direction, currentShipVelocity);
        projectileParameters.SetUpParameters();
        projectileBody.velocity = projectileParameters.GetCurrentProjectileVelocity;
        projectileSizeParent.localScale = Vector3.one * projectileParameters.GetCurrentProjectileSize;
        lifetimeEnded = false;

        relatedHitbox.gameObject.SetActive(true);
        relatedHitbox.SetUp(this);

        projectileRenderer.gameObject.SetActive(true);

        projectileFunctionEnded = false;
        projectileReturned = false;

        ResetAllSpecialEffects();


        if (endLifeTimeHeightCurve == null)
            endLifeTimeHeightCurve = endLifeTimeHeightCurve.SetAsSlowInFastOutDecreasingCurve();
        else if(endLifeTimeHeightCurve.length == 0)
            endLifeTimeHeightCurve = endLifeTimeHeightCurve.SetAsSlowInFastOutDecreasingCurve();

        if (projectileParameters.GetTotalLifeTime < normalFallDuration)
            thisFallDuration = projectileParameters.GetTotalLifeTime;
        else
            thisFallDuration = normalFallDuration;

        startedFall = false;
        persistingPlaced = false;

        SetUpAirRotation();
    }

    public virtual void Update()
    {
        if (!projectileFunctionEnded)
        {
            if (!PersistantAndPlaced)
                projectileParameters.UpdateLifetime();
        }

        if (projectileParameters.LifetimeEnded && !lifetimeEnded)
        {
            lifetimeEnded = true;
            OnLifeTimeEnded();
        }

        if (projectileFunctionEnded && !projectileReturned)
            CheckIfReadyToBeReturnedToPool();


        if (!persistingPlaced)
        {
            if (rotationSpeed != 0)
                UpdateAirRotation();
        }
        else
        {
            //if(currentMaxFloatingOffset != 0)
                UpdateFloatingMove();
            UpdateStreamInfluenceValue();
        }
    }

    private void FixedUpdate()
    {
        if (!projectileFunctionEnded)
        {
            if (!PersistantAndPlaced)
                UpdateEvolutiveSpeed();
        }

        if (!projectileFunctionEnded)
        {
            if (!PersistantAndPlaced)
                UpdateEndLifeTime();
        }
    }

    #region Speed and Velocity
    /// <summary>
    /// Actualise la vitesse du projectile 
    /// </summary>
    public void UpdateEvolutiveSpeed()
    {
        projectileBody.velocity = projectileParameters.GetCurrentProjectileVelocity;
    }

    public Vector3 GetProjectileVelocity { get { return projectileBody.velocity; } }
    #endregion

    #region LifeTime
    [Header("Life Time and Movement End")]
    [SerializeField] bool isPersistingProjectile;
    AnimationCurve endLifeTimeHeightCurve;
    float normalFallDuration = 0.2f;
    float thisFallDuration;
    bool startedFall;
    float startFallHeigth;
    protected bool persistingPlaced;

    public bool PersistantAndPlaced
    {
        get
        {
            return isPersistingProjectile && persistingPlaced;
        }
    }

    /// <summary>
    /// Vrai si la durée de vie du projectile a atteint sa fin
    /// </summary>
    protected bool lifetimeEnded;

    public void UpdateEndLifeTime()
    {
        if (!IsBoulder)
        {
            if(projectileParameters.GetCurrentLifeTime < thisFallDuration)
            {
                if (!startedFall)
                {
                    startFallHeigth = transform.position.y;
                    startedFall = true;
                }

                float currentHeightCoeff = endLifeTimeHeightCurve.Evaluate(1 - (projectileParameters.GetCurrentLifeTime / thisFallDuration));
                float currentHeigth = Mathf.Lerp(GameManager.gameManager.GetSeaLevel, startFallHeigth, currentHeightCoeff);

                Vector3 modifiedPosition = transform.position;
                modifiedPosition.y = currentHeigth;
                transform.position = modifiedPosition;
            }
        }
    }

    /// <summary>
    /// S'appelle lorsque le projectile atteint la fin de sa course
    /// </summary>
    public virtual void OnLifeTimeEnded()
    {
        if (isPersistingProjectile)
        {
            projectileBody.velocity = Vector3.zero;
            persistingPlaced = true;
            SetUpFloatingMove();
        }
        else
        {
            if (onLifeTimeEndedParticles != null)
            {
                onLifeTimeEndedParticles.Play();
            }

            CheckForSpecialEffectZoneGeneration();

            SetProjectileInofensive();
        }
    }
    #endregion

    public virtual void ExplodeOnContact()
    {
        if (explosionParticles != null)
        {
            explosionParticles.Play();
        }

        if (!projectileSpecialParameters.GetExplosionParameters.IsNull)
        {
            Explosion newExplosion = GameManager.gameManager.PoolManager.GetExplosion(ExplosionPoolTag.Normal, PoolInteractionType.GetFromPool);
            newExplosion.transform.position = transform.position;
            newExplosion.SetAttackTag(damageTag);
            newExplosion.SetUpExplosionParameters(projectileSpecialParameters.GetExplosionParameters);
        }

        SetProjectileInofensive();

        CheckForSpecialEffectZoneGeneration();
    }

    public void SetProjectileInofensive()
    {
        relatedHitbox.gameObject.SetActive(false);
        projectileFunctionEnded = true;
        projectileRenderer.gameObject.SetActive(false);
        projectileBody.velocity = Vector3.zero;
        sourceShip = null;
        sourceTurret = null;
    }

    #region Damages and Knockback
    /// <summary>
    /// Hitbox liée à ce projectile
    /// </summary>
    [Header("Damages and Knockback")]
    [SerializeField] protected ProjectileHitbox relatedHitbox;
    /// <summary>
    /// L'identificateur permettant de savoir ce que ce projectile va blesser
    /// </summary>
    [SerializeField] protected AttackTag damageTag;
    /// <summary>
    /// L'identificateur permettant de savoir ce que ce projectile va blesser
    /// </summary>
    public AttackTag GetDamageTag
    {
        get
        {
            return damageTag;
        }
    }

    public void SetProjectileTag(AttackTag projTag)
    {
        damageTag = projTag;
    }

    public DamagesParameters GetDamagesParameters
    {
        get
        {
            return projectileParameters.DmgParameters;
        }
    }
    #endregion

    #region Pooling
    protected bool projectileFunctionEnded = false;
    public bool ProjectileFunctionEnded { get { return projectileFunctionEnded; } }

    protected bool projectileReturned = true;
    public void CheckIfReadyToBeReturnedToPool()
    {
        if (!onLifeTimeEndedParticles.isPlaying && !explosionParticles.isPlaying && !projectileReturned)
        {
            ReturnProjectile();
        }
    }

    public void ReturnProjectile()
    {
        projectileReturned = true;
        gameObject.SetActive(false);

        GameManager.gameManager.PoolManager.ReturnProjectile(this);
    }
    #endregion

    #region Persistance
    /// <summary>
    /// Un projectile "persistant" se définit par le fait de rester à la surface de l'eau
    /// </summary>
    #endregion

    #region Special Effects Parameters
    public void ResetAllSpecialEffects()
    {
        projectileSpecialParameters = null;
    }

    ProjectileSpecialParameters projectileSpecialParameters;
    public ProjectileSpecialParameters GetProjectileSpecialParameters { get { return projectileSpecialParameters; } }

    public void SetProjectileSpecialParameters(ProjectileSpecialParameters specialParameters)
    {
        projectileSpecialParameters = 
            new ProjectileSpecialParameters(
                new ShipSpeedModifier(specialParameters.GetSpeedModifier), 
                new ProjectilePiercingParameters(specialParameters.GetPiercingParameters), 
                new ProjectileSkeweringParameters(specialParameters.GetSkeweringParameters), 
                specialParameters.GetExplosionParameters, 
                new SmokeZoneParameters(specialParameters.GetSmokeZoneParameters), 
                new SlowingZoneParameters(specialParameters.GetSlowingZoneParameters));

        projectileSpecialParameters.SetRelatedProjectile(this);
    }

    #region Special Effect Zones
    public void CheckForSpecialEffectZoneGeneration()
    {
        if (!projectileSpecialParameters.GetSmokeZoneParameters.IsNull)
        {
            SmokeZone smokeZone = GameManager.gameManager.PoolManager.GetSpecialEffectZone(SpecialEffectZonePoolTag.Smoke, PoolInteractionType.GetFromPool) as SmokeZone;
            smokeZone.transform.position = transform.position;
            smokeZone.SetUpZone(projectileSpecialParameters.GetSmokeZoneParameters.GetZoneDuration, projectileSpecialParameters.GetSmokeZoneParameters.GetZoneSize, projectileSpecialParameters.GetSmokeZoneParameters);
        }

        if (!projectileSpecialParameters.GetSlowingZoneParameters.IsNull)
        {
            SlowingZone slowingZone = GameManager.gameManager.PoolManager.GetSpecialEffectZone(SpecialEffectZonePoolTag.SpeedModifier, PoolInteractionType.GetFromPool) as SlowingZone;
            slowingZone.transform.position = transform.position;
            slowingZone.SetUpZone(projectileSpecialParameters.GetSlowingZoneParameters.GetZoneDuration, projectileSpecialParameters.GetSlowingZoneParameters.GetZoneSize, projectileSpecialParameters.GetSlowingZoneParameters);
        }
    }
    #endregion
    #endregion
}

public class ProjectileSpecialParameters
{
    public ProjectileSpecialParameters(ShipSpeedModifier speedModifier, ProjectilePiercingParameters piercingParameters, ProjectileSkeweringParameters skeweringParameters, ExplosionParameters explParameters, SmokeZoneParameters smokeZoneParams, SlowingZoneParameters slowingZoneParams)
    {
        shipSpeedModifier = speedModifier;
        projectilePiercingParameters = piercingParameters;
        projectileSkeweringParameters = skeweringParameters;
        explosionParameters = explParameters;
        smokeZoneParameters = smokeZoneParams;
        slowingZoneParameters = slowingZoneParams;
    }

    public void SetRelatedProjectile(Projectile proj)
    {
        relatedProjectile = proj;
    }
    Projectile relatedProjectile;
    public Projectile GetRelatedProjectile { get { return relatedProjectile; } }

    ShipSpeedModifier shipSpeedModifier;
    public ShipSpeedModifier GetSpeedModifier
    {
        get
        {
            if (shipSpeedModifier.IsNull)
                return null;
            else 
                return shipSpeedModifier;
        }
    }

    ProjectilePiercingParameters projectilePiercingParameters;
    public ProjectilePiercingParameters GetPiercingParameters { get { return projectilePiercingParameters; } }

    ProjectileSkeweringParameters projectileSkeweringParameters;
    public ProjectileSkeweringParameters GetSkeweringParameters { get { return projectileSkeweringParameters; } }

    ExplosionParameters explosionParameters;
    public ExplosionParameters GetExplosionParameters { get { return explosionParameters; } }

    SmokeZoneParameters smokeZoneParameters;
    public SmokeZoneParameters GetSmokeZoneParameters { get { return smokeZoneParameters; } }

    SlowingZoneParameters slowingZoneParameters;
    public SlowingZoneParameters GetSlowingZoneParameters { get { return slowingZoneParameters; } }
}

[System.Serializable]
public class ProjectilePiercingParameters
{
    public ProjectilePiercingParameters(ProjectilePiercingParameters parameters)
    {
        numberOfPiercing = parameters.numberOfPiercing;
    }

    [SerializeField] int numberOfPiercing;
    public int GetNumberOfPiercing { get { return numberOfPiercing; } }

    public void DecreamentPiercing()
    {
        numberOfPiercing--;
    }
}

[System.Serializable] 
public class ProjectileSkeweringParameters
{
    Projectile sourceProjectile;
    public Projectile GetSourceProjectile { get { return sourceProjectile; } }
    public void SetSourceProjectile(Projectile proj)
    {
        sourceProjectile = proj;
    }

    public ProjectileSkeweringParameters(ProjectileSkeweringParameters parameters)
    {
        skeweringOn = parameters.skeweringOn;
        damagesOnImpact = parameters.damagesOnImpact;
        stunOnObstacleDuration = parameters.stunOnObstacleDuration;
        stunOnOtherShipDuration = parameters.stunOnOtherShipDuration;
    }

    [SerializeField] bool skeweringOn;
    public bool Skewering { get { return skeweringOn; } }

    [SerializeField] DamagesParameters damagesOnImpact;
    public DamagesParameters GetDamagesOnImpact { get { return damagesOnImpact; } }

    [SerializeField] float stunOnObstacleDuration;
    public float GetStunOnObstacleDuration { get { return stunOnObstacleDuration; } }

    [SerializeField] float stunOnOtherShipDuration;
    public float GetStunOnOtherShipDuration { get { return stunOnOtherShipDuration; } }
}