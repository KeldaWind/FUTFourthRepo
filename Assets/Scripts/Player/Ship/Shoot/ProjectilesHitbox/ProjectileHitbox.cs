using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// La hitbox des projectile, reconnaissant les objets touchés.
/// </summary>
public class ProjectileHitbox : MonoBehaviour, IDamageSource
{
    /// <summary>
    /// Le projectile lié à cette hitbox
    /// </summary>
    Projectile relatedProjectile;

    /// <summary>
    /// Initialisation de la hitbox du projectile
    /// </summary>
    /// <param name="proj">Le projectile lié à cette hitbox</param>
    public void SetUp(Projectile proj)
    {
        relatedProjectile = proj;
    }

    #region Trigger Management
    private void OnTriggerEnter(Collider other)
    {
        IDamageReceiver damageReceiver = other.GetComponent<IDamageReceiver>();
        if (damageReceiver != null)
        {
            if (damageReceiver == relatedProjectile.GetSourceDamageReceiver)
                return;

            if (GetDamageTag == AttackTag.Environment || GetDamageTag == AttackTag.Enemies || GetDamageTag != damageReceiver.GetDamageTag)
                DealDamage(damageReceiver, relatedProjectile.GetDamagesParameters);
        }

        if (relatedProjectile.PersistantAndPlaced)
        {
            IDamageSource damageSource = other.GetComponent<IDamageSource>();
            if (damageSource != null && damageReceiver == null)
                relatedProjectile.ExplodeOnContact();
        }

        if (other.GetComponent<Obstacle>() != null)
        {
            relatedProjectile.ExplodeOnContact();
        }
    }
    #endregion

    #region Damages Management
    /// <summary>
    /// Identificateur permettant de savoir ce que cet objet va blesser
    /// </summary>
    public AttackTag GetDamageTag
    {
        get
        {
            return relatedProjectile.GetDamageTag;
        }
    }

    public void DealDamage(IDamageReceiver damageReceiver, DamagesParameters damagesParameters)
    {
        damageReceiver.ReceiveDamage(this, damagesParameters, relatedProjectile.GetProjectileSpecialParameters);

        if (relatedProjectile.GetProjectileSpecialParameters.GetPiercingParameters.GetNumberOfPiercing > 0)
        {
            relatedProjectile.GetProjectileSpecialParameters.GetPiercingParameters.DecreamentPiercing();

            FeedbackObject woodProjection = GameManager.gameManager.PoolManager.GetFeedbackObject(FeedbackObjectPoolTag.WoodDestruction, PoolInteractionType.GetFromPool);
            if (woodProjection != null)
            {
                woodProjection.transform.position = transform.position;
                woodProjection.StartFeedback(2, 0.2f);
            }
        }
        else if (relatedProjectile.GetProjectileSpecialParameters.GetSkeweringParameters.Skewering)
        {
            FeedbackObject woodProjection = GameManager.gameManager.PoolManager.GetFeedbackObject(FeedbackObjectPoolTag.WoodDestruction, PoolInteractionType.GetFromPool);
            if (woodProjection != null)
            {
                woodProjection.transform.position = transform.position;
                woodProjection.StartFeedback(2, 0.2f);
            }
        }
        else
            relatedProjectile.ExplodeOnContact();
    }

    public Vector3 GetDamageSourcePosition { get { return transform.position; } }
    #endregion
}
