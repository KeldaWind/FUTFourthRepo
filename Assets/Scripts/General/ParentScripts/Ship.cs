using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour
{
    /// <summary>
    /// Rigidbody du bateau
    /// </summary>
    [Header("Common References")]
    [SerializeField] protected Rigidbody shipBody;
    [SerializeField] protected BoxCollider shipBoxCollider;
    public void SetShipBoxColliderDimensions(Vector3 dimensions)
    {
        shipBoxCollider.size = dimensions;
    }

    /// <summary>
    /// Module de contrôle des mouvements de ce bateau
    /// </summary>
    [Space]
    [Header("Common Modules : Movement")]
    [SerializeField] protected bool startMoving;
    [SerializeField] protected ShipMovements shipMovements;
    public ShipMovements ShipMvt
    {
        get
        {
            return shipMovements;
        }
    }
    /// <summary>
    /// Renvoie la vélocité actuelle du bateau affectée par le module de mouvements
    /// </summary>
    public Vector3 GetShipVelocity
    {
        get
        {
            return shipMovements.GetCurrentShipVelocity;
        }
    }
    /// <summary>
    /// Renvoie la vitesse actuelle du bateau affectée par le module de mouvements
    /// </summary>
    public float GetShipSpeed
    {
        get
        {
            return shipMovements.GetCurrentSpeed;
        }
    }

    /// <summary>
    /// Gestionnaire de vie de ce bateau
    /// </summary>
    [Space]
    [Header("Common Modules : Life and Knockback")]
    [SerializeField] protected LifeManager lifeManager;
    public LifeManager LfManager
    {
        get
        {
            return lifeManager;
        }
    }
    /// <summary>
    /// Gestionnaire de knockback de ce bateau
    /// </summary>
    [SerializeField] protected KnockbackManager knockbackManager;
    public KnockbackManager KnckbckManager
    {
        get
        {
            return knockbackManager;
        }
    }
    /// <summary>
    /// La hitbox de ce bateau
    /// </summary>
    [SerializeField] protected ShipHitbox relatedShipHitbox;
    public IDamageReceiver GetShipDamageReceiver
    {
        get
        {
            return relatedShipHitbox;
        }
    }

    protected bool alreadyDead;

    [Space]
    [Header("Common Modules : Rendering")]
    /// <summary>
    /// Module de rendering du bateau 
    /// </summary>
    [SerializeField] protected ShipRenderingManager shipRenderingManager;
    public ShipRenderingManager ShipRenderManager { get { return shipRenderingManager; } }

    public virtual void SetUp(bool stopped)
    {
        shipRenderingManager.SetUp(transform.rotation.eulerAngles.y, lifeManager);
        shipMovements.SetUp(this, stopped);
        lifeManager.SetUp(relatedShipHitbox);
        knockbackManager.SetUp(this);

        if (relatedShipHitbox != null)
        {
            relatedShipHitbox.SetUp(this);
        }

        alreadyDead = false;
    }

    public virtual void Start()
    {
        SetUp(!startMoving);
    }

    public virtual void FixedUpdate()
    {
        Vector3 velocity = (BeingSkewered ? skeweringProjectile.GetProjectileVelocity : shipMovements.GetCurrentShipVelocity) + knockbackManager.GetCurrentKnockbackForce + shipMovements.GetCurrentStreamForce;

        if (shipMovements.ShipIsAnchored)
            velocity = Vector3.zero;

        shipBody.velocity = velocity;
        UpdateShipRotation();

        shipRenderingManager.UpdateRendering(shipMovements.GetCurrentRotationSignedCoeff, shipMovements.GetCurrentSpeedCoeffFromZeroToMax, shipMovements.Stopped);
    }

    public virtual void Update()
    {
        lifeManager.UpdateLifeManagement();
        knockbackManager.UpdateAndGetCurrentKnockbackForce();

        if (IsStun)
            UpdateStun();

        if (BeingSkewered)
            UpdateSkewering();

        if (currentProtectionParameters.Protecting)
            UpdateCurrentProtectionParameters();

        if (blind && remainingBlindingDuration != 0)
            UpdateBlinding();
    }

    public void SetPositionAndRotation(Transform posAndRot)
    {
        transform.position = posAndRot.position;
        shipMovements.SetCurrentRotation(posAndRot.rotation.eulerAngles.y);
    }

    #region Rotation Management
    public void UpdateShipRotation()
    {
        Vector3 directionToTurnToward = shipMovements.GetCurrentShipVelocity;

        if (shipMovements.GetCurrentShipVelocity != Vector3.zero)
        {
            float rotY = Mathf.Atan2(directionToTurnToward.x, directionToTurnToward.z) * Mathf.Rad2Deg;
            transform.localRotation = Quaternion.Euler(new Vector3(0, rotY, 0));
        }
        else
            transform.localRotation = Quaternion.Euler(new Vector3(0, shipMovements.GetCurrentRotation, 0));
    }

    public void StartDamageShipAnim(int damages, DamageSourceRelativePosition damageSourceRelativePosition)
    {
        shipRenderingManager.StartDamageAnimation(damages, damageSourceRelativePosition);

        if(shipFeedbacks != null)
            shipFeedbacks.PlayDamageFeedback();
    }
    #endregion

    public virtual void Die()
    {
        shipRenderingManager.HideShip();
        shipMovements.StopShip();

        alreadyDead = true;


        #region Variables reset
        remainingStunDuration = 0;
        skeweringProjectile = null;

        shipMovements.Reset();
        knockbackManager.Reset();
        #endregion

        if (shipFeedbacks != null)
            shipFeedbacks.PlayDeathFeedback();

        knockbackManager.Reset();
    }

    #region Stun Management
    protected float remainingStunDuration;

    public bool IsStun
    {
        get
        {
            return remainingStunDuration != 0;
        }
    }

    public virtual void SetStun(float stunDuration)
    {
        remainingStunDuration = stunDuration;
        shipMovements.StopShip();

        if (shipFeedbacks != null)
            shipFeedbacks.PlayStunFeedback();
    }

    public void UpdateStun()
    {
        if (remainingStunDuration > 0)
            remainingStunDuration -= Time.deltaTime;
        else if (remainingStunDuration < 0)
            EndStun();
    }

    public virtual void EndStun()
    {
        remainingStunDuration = 0;
        shipMovements.StartShip();

        if (shipFeedbacks != null)
            shipFeedbacks.StopStunFeedback();
    }
    #endregion

    #region Skewering Management
    public bool BeingSkewered
    {
        get
        {
            return skeweringProjectile != null;
        }
    }

    public ProjectileSkeweringParameters GetSkeweringParameters
    {
        get
        {
            if (skeweringProjectile == null)
                return null;
            else
                return skeweringProjectile.GetProjectileSpecialParameters.GetSkeweringParameters;
        }
    }

    public Vector3 GetSkeweringProjectileDirection
    {
        get
        {
            return skeweringProjectile.GetProjectileVelocity.normalized;
        }
    }

    protected Projectile skeweringProjectile;

    public virtual void SetCurrentSkeweringProjectile(Projectile projectile)
    {
        skeweringProjectile = projectile;
    }

    public void UpdateSkewering()
    {
        if (skeweringProjectile.ProjectileFunctionEnded)
        {
            EndSkewering();
        }
    }

    public void EndSkewering()
    {
        DestroySkeweringProjectile();
        if(!IsStun)
            shipMovements.StartShip();
    }

    public void DestroySkeweringProjectile()
    {
        skeweringProjectile.ExplodeOnContact();
        skeweringProjectile = null;
    }
    #endregion

    #region ProtectionManagement
    protected ProtectionParameters currentProtectionParameters;

    public bool IsLifeProtected { get { return currentProtectionParameters.Protecting && currentProtectionParameters.IsLifeProtection; } }
    public bool IsKnockbackProtected { get { return currentProtectionParameters.Protecting && currentProtectionParameters.IsKnockbackProtection; } }

    public void SetCurrentProtectionParameters(ProtectionParameters protectionParameters)
    {
        if (protectionParameters.GetProtectionDuration == 0)
            return;

        currentProtectionParameters = protectionParameters;
        currentProtectionParameters.StartProtection();

        if (currentProtectionParameters.GeneratesProtectionSphere)
        {
            ProtectionSphere newProtectionSphere = GameManager.gameManager.PoolManager.GetProtectionSphere(currentProtectionParameters.GetProtectionSphereType, PoolInteractionType.GetFromPool);
            newProtectionSphere.SetUpSphere(currentProtectionParameters.GetProtectionDuration, currentProtectionParameters.GetProtectionSphereRadius, transform, relatedShipHitbox.GetDamageTag);
        }
    }

    public void UpdateCurrentProtectionParameters()
    {
        currentProtectionParameters.UpdateProtection();
    }
    #endregion

    #region Blinding Management
    protected float remainingBlindingDuration;
    protected bool blind;

    /// <summary>
    /// Pas de durée, aveugle jusqu'à ce que la fonction de fin de blind soit appellée
    /// </summary>
    public virtual void Blind()
    {
        blind = true;
    }

    /// <summary>
    /// Aveugle sur la durée rentrée en Input
    /// </summary>
    /// <param name="duration"></param>
    public virtual void Blind(float duration)
    {
        blind = true;
        remainingBlindingDuration = duration;
    }

    public void UpdateBlinding()
    {
        if (remainingBlindingDuration > 0)
            remainingBlindingDuration -= Time.deltaTime;
        else if (remainingBlindingDuration < 0)
            Unblind();
    }

    public virtual void Unblind()
    {
        blind = false;
        remainingBlindingDuration = 0;
    }
    #endregion

    [Header("Feedbacks")]
    [SerializeField] protected ShipFeedbacks shipFeedbacks;

    public void PlaySlowingFeedback()
    {
        if (shipFeedbacks != null)
            shipFeedbacks.PlaySlowingFeedback();
    }

    public void StopSlowingFeedback()
    {
        if (shipFeedbacks != null)
            shipFeedbacks.StopSlowingFeedback();
    }
}
