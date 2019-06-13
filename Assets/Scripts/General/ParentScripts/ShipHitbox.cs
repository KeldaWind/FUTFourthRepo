using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void OnOtherShipCollision(Ship otherShip);
public class ShipHitbox : MonoBehaviour, ICollisionReceiver, IDamageReceiver, ICollisionSource, IDamageSource
{
    public OnOtherShipCollision OnOtherShipCollision;

    /// <summary>
    /// Bateau du joueur relié à cette hitbox
    /// </summary>
    protected Ship relatedShip;
    /// <summary>
    /// Bateau du joueur relié à cette hitbox
    /// </summary>
    public Ship GetRelatedShip
    {
        get
        {
            return relatedShip;
        }
    }

    [SerializeField] protected AttackTag attackTag = AttackTag.Player;

    /// <summary>
    /// Initialisation de cette hitbox
    /// </summary>
    /// <param name="ship">Le bateau possédant cette hitbox</param>
    public virtual void SetUp(Ship ship)
    {
        relatedShip = ship;
    }

    #region Attacks Gesture
    #region Receiver
    /// <summary>
    /// Returns the tag from this attackReceiver
    /// </summary>
    public AttackTag GetDamageTag
    {
        get
        {
            return attackTag;
        }
    }

    /// <summary>
    /// Returns if this receiver is currently damageable or not
    /// </summary>
    public bool IsDamageable
    {
        get
        {
            return true;
        }
    }

    /// <summary>
    /// Call this function as the life of this attack receiver reaches 0
    /// </summary>
    public virtual void Die()
    {
        if(OnDeath != null)
            OnDeath(this);

        relatedShip.Die();
    }

    /// <summary>
    /// Used to receive the attack from the inputed source
    /// </summary>
    /// <param name="attackSource">The attack source that touched this attack receiver</param>
    /// <param name="attackParameters">The parameters of the received attack</param>
    public virtual void ReceiveDamage(IDamageSource damageSource, DamagesParameters damagesParameters, ProjectileSpecialParameters projSpecialParameters)
    {
        if (relatedShip.IsLifeProtected)
            return;

        relatedShip.LfManager.Damage(damagesParameters.GetDamageAmount, damagesParameters.GetRecoveringTime, damagesParameters.GetRecoveringType);

        #region Damage Feedback
        DamageSourceRelativePosition damageSourceRelativePosition = DamageSourceRelativePosition.Right;

        if (damageSource != null)
        {
            Vector3 attackSourceRelativePos = (damageSource.GetDamageSourcePosition - transform.position).normalized;
            Vector3 selfDirection = relatedShip.GetShipVelocity;
            if (Vector3.Dot(attackSourceRelativePos, selfDirection.GetLeftOrthogonalVectorXZ()) > 0)
                damageSourceRelativePosition = DamageSourceRelativePosition.Left;
        }
        else
        {
            int random = Random.Range(0, 2);
            if (random == 1)
                damageSourceRelativePosition = DamageSourceRelativePosition.Left;
        }

        relatedShip.StartDamageShipAnim(damagesParameters.GetDamageAmount, damageSourceRelativePosition);
        #endregion

        if (projSpecialParameters != null)
        {
            if (projSpecialParameters.GetSpeedModifier != null)
                relatedShip.ShipMvt.StartNewSpeedModifier(projSpecialParameters.GetSpeedModifier);

            if (projSpecialParameters.GetSkeweringParameters.Skewering)
                relatedShip.SetCurrentSkeweringProjectile(projSpecialParameters.GetSkeweringParameters.GetSourceProjectile);
        }
    }

    public virtual void UpdateLifeBar(int lifeAmount)
    {
        relatedShip.LfManager.UpdateLifeRendering();
    }

    public event OnLifeEvent OnDeath;
    #endregion

    #region Source
    public void DealDamage(IDamageReceiver damageReceiver, DamagesParameters damagesParameters)
    {
        damageReceiver.ReceiveDamage(this, damagesParameters, null);
    }

    public Vector3 GetDamageSourcePosition { get { return transform.position; } }
    #endregion
    #endregion

    #region KnockbackManagement
    [Header("Ship Collisions Parameters")]
    [SerializeField] KnockbackParameters shipCollisionKnockbackParameters;

    #region Receiver
    /// <summary>
    /// Renvoie le tag de ce collision receiver
    /// </summary>
    public AttackTag GetCollisionTag
    {
        get
        {
            return attackTag;
        }
    }

    /// <summary>
    /// Gère la prise de Knockback
    /// </summary>
    /// <param name="collisionSource">Source de la collision</param>
    /// <param name="knockbackParameters">Paramètre de knockbacks donnés par la source de la collision</param>
    /// <param name="knockbackDirection">Direction du knockback</param>
    /// <param name="redirection">Redirection appliquée par le knockback</param>
    public void ReceiveKnockback(ICollisionSource collisionSource, KnockbackParameters knockbackParameters, Vector3 knockbackDirection, Vector3 redirection)
    {
        relatedShip.KnckbckManager.SetCurrentKnockbackParameters(knockbackParameters, knockbackDirection, redirection);
    }

    public Vector3 GetMovementDirection()
    {
        if(!relatedShip.BeingSkewered)
            return relatedShip.GetShipVelocity.normalized;
        else
            return relatedShip.GetSkeweringProjectileDirection;

    }

    public bool Recovering()
    {
        return relatedShip.KnckbckManager.Recovering;
    }
    #endregion

    #region Source
    public void DealKnockback(ICollisionReceiver collisionReceiver, KnockbackParameters knockbackParameters, Vector3 knockbackDirection, Vector3 redirection)
    {
        collisionReceiver.ReceiveKnockback(this, knockbackParameters, knockbackDirection, redirection);
    }
    #endregion
    #endregion

    #region Inflicted Damages And Knockback
    private void OnCollisionStay(Collision collision)
    {
        bool hasRammingPriority = true;
        if (relatedShip.ShipMvt.GetCurrentRammingParameters.IsAttacking)
        {
            ShipHitbox hitShipHitbox = collision.gameObject.GetComponent<ShipHitbox>();
            if (hitShipHitbox != null)
            {
                if (hitShipHitbox.relatedShip.ShipMvt.GetCurrentRammingParameters.IsAttacking)
                {
                    //les deux attaquent, il faut vérifier si l'autre est le joueur
                    if (hitShipHitbox.relatedShip as PlayerShip != null)
                        hasRammingPriority = false;
                }
            }
        }
        else
            hasRammingPriority = false;


            ICollisionReceiver hitCollisionReceiver = collision.gameObject.GetComponent<ICollisionReceiver>();
        if(hitCollisionReceiver != null)
        {
            if (hitCollisionReceiver.Recovering())
                return;

            if (relatedShip.ShipMvt.GetCurrentRammingParameters.IsAttacking)
            {
                if (hasRammingPriority)
                    SetRammingKnockback(collision, hitCollisionReceiver);
            }
            else
            {
                ShipHitbox hitShip = collision.gameObject.GetComponent<ShipHitbox>();
                if (hitShip != null)
                {
                    if (OnOtherShipCollision != null)
                        OnOtherShipCollision(hitShip.relatedShip);

                    if (!hitShip.relatedShip.ShipMvt.GetCurrentRammingParameters.IsRamming)
                    {
                        Vector3 knockbackDir = hitShip.transform.position - transform.position;
                        knockbackDir.y = 0;
                        knockbackDir.Normalize();

                        DealKnockback(hitCollisionReceiver, shipCollisionKnockbackParameters, knockbackDir, knockbackDir);
                    }

                    if (relatedShip.BeingSkewered)
                    {
                        hitShip.relatedShip.SetStun(relatedShip.GetSkeweringParameters.GetStunOnOtherShipDuration);
                        DealDamage(hitShip, relatedShip.GetSkeweringParameters.GetDamagesOnImpact);

                        relatedShip.SetStun(relatedShip.GetSkeweringParameters.GetStunOnOtherShipDuration);
                        ReceiveDamage(this, relatedShip.GetSkeweringParameters.GetDamagesOnImpact, null);

                        relatedShip.EndSkewering();
                    }
                }
            }
        }

        IDamageReceiver hitDamageReceiver = collision.gameObject.GetComponent<IDamageReceiver>();
        if (hitDamageReceiver != null)
        {
            if (relatedShip.ShipMvt.GetCurrentRammingParameters.IsAttacking)
            {
                if (hasRammingPriority)
                    DealDamage(hitDamageReceiver, relatedShip.ShipMvt.GetCurrentRammingParameters.GetDamagesParameters);

                if(relatedShip as PlayerShip != null)
                    GameManager.gameManager.SlwMoManager.SetSlowMo(relatedShip.ShipMvt.GetCurrentRammingParameters.GetSlowMoParameters);
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        ShipHitbox hitShip = collision.gameObject.GetComponent<ShipHitbox>();
        if (hitShip != null)
        {
            if (OnOtherShipCollision != null)
                OnOtherShipCollision(hitShip.relatedShip);
        }
    }
    #endregion

    #region Ship Collisions Management
    public void SetRammingKnockback(Collision collision, ICollisionReceiver hitCollisionReceiver)
    {
        #region Calculates Redirection and Knockback
        Vector3 knockbackVector = relatedShip.GetShipVelocity.normalized;
        Vector3 redirectionVector = hitCollisionReceiver.GetMovementDirection();

        Vector3 hitObjectDirection = Quaternion.Euler(-collision.transform.rotation.eulerAngles) * hitCollisionReceiver.GetMovementDirection();

        Vector3 hitNormal = new Vector3();
        Vector3 hitPosition = new Vector3();
        Vector3 worldHitPosition;
        int counter = 0;

        foreach (ContactPoint contact in collision.contacts)
        {
            hitNormal += contact.normal;
            hitPosition += contact.point;
            counter++;
        }

        #region V1
        Vector3 hitObjectCenter = collision.collider.bounds.center;

        hitNormal /= counter;
        hitNormal = Quaternion.Euler(-collision.transform.rotation.eulerAngles) * hitNormal;

        hitPosition /= counter;
        worldHitPosition = hitPosition;
        hitPosition = Quaternion.Euler(-collision.transform.rotation.eulerAngles) * (hitPosition - hitObjectCenter);

        float semiLenght = collision.collider.bounds.extents.z;
        float semiWidht = collision.collider.bounds.extents.x;

        if (Mathf.Abs(Vector3.Dot(hitNormal, hitObjectDirection)) < 0.5f)
        {
            //latéral
            if (hitPosition.x > 0)
            {
                //par la droite
                if (hitPosition.z > 0)
                {
                    //à l'avant
                    if (Vector3.Dot(GetMovementDirection(), hitCollisionReceiver.GetMovementDirection()) > -0.45f)
                    {
                        knockbackVector = -hitCollisionReceiver.GetMovementDirection();
                        redirectionVector = ((-hitCollisionReceiver.GetMovementDirection().normalized + hitCollisionReceiver.GetMovementDirection().GetLeftOrthogonalVectorXZ().normalized) / 2).normalized;
                        //Debug.Log("pas exception");
                    }
                    else if(Vector3.Dot(GetMovementDirection(), hitCollisionReceiver.GetMovementDirection()) < 0f)
                    {
                        knockbackVector = hitCollisionReceiver.GetMovementDirection().GetLeftOrthogonalVectorXZ();
                        redirectionVector = ((hitCollisionReceiver.GetMovementDirection().normalized + hitCollisionReceiver.GetMovementDirection().GetLeftOrthogonalVectorXZ().normalized) / 2).normalized;
                        //Debug.Log("exception");
                    }
                    //Debug.Log("latéral avant droit");
                }
                else
                {
                    //à l'arrière
                    if (Vector3.Dot(GetMovementDirection(), hitCollisionReceiver.GetMovementDirection()) < 0.75f)
                    {
                        knockbackVector = hitCollisionReceiver.GetMovementDirection();
                        redirectionVector = ((hitCollisionReceiver.GetMovementDirection().normalized + hitCollisionReceiver.GetMovementDirection().GetRightOrthogonalVectorXZ().normalized) / 2).normalized;
                        //Debug.Log("pas exception");
                    }
                    else
                    {
                        knockbackVector = hitCollisionReceiver.GetMovementDirection().GetLeftOrthogonalVectorXZ();
                        redirectionVector = ((hitCollisionReceiver.GetMovementDirection().normalized + hitCollisionReceiver.GetMovementDirection().GetLeftOrthogonalVectorXZ().normalized) / 2).normalized;
                        //Debug.Log("exception");
                    }
                    //Debug.Log("latéral arrière droit");
                }

            }
            else
            {
                //par la gauche
                if (hitPosition.z > 0)
                {
                    //à l'avant
                    if (Vector3.Dot(GetMovementDirection(), hitCollisionReceiver.GetMovementDirection()) > -0.45f)
                    {
                        knockbackVector = -hitCollisionReceiver.GetMovementDirection();
                        redirectionVector = ((-hitCollisionReceiver.GetMovementDirection().normalized + hitCollisionReceiver.GetMovementDirection().GetRightOrthogonalVectorXZ().normalized) / 2).normalized;
                        //Debug.Log("pas exception");
                    }
                    else
                    {
                        knockbackVector = hitCollisionReceiver.GetMovementDirection().GetRightOrthogonalVectorXZ();
                        redirectionVector = ((hitCollisionReceiver.GetMovementDirection().normalized + hitCollisionReceiver.GetMovementDirection().GetRightOrthogonalVectorXZ().normalized) / 2).normalized;
                        //Debug.Log("exception");
                    }
                    //Debug.Log("latéral avant gauche");
                }
                else
                {
                    //à l'arrière
                    if (Vector3.Dot(GetMovementDirection(), hitCollisionReceiver.GetMovementDirection()) < 0.75f)
                    {
                        knockbackVector = hitCollisionReceiver.GetMovementDirection();
                        redirectionVector = ((hitCollisionReceiver.GetMovementDirection().normalized + hitCollisionReceiver.GetMovementDirection().GetLeftOrthogonalVectorXZ().normalized) / 2).normalized;
                        //Debug.Log("pas exception");
                    }
                    else
                    {
                        knockbackVector = hitCollisionReceiver.GetMovementDirection().GetRightOrthogonalVectorXZ();
                        redirectionVector = ((hitCollisionReceiver.GetMovementDirection().normalized + hitCollisionReceiver.GetMovementDirection().GetRightOrthogonalVectorXZ().normalized) / 2).normalized;
                        //Debug.Log("exception");
                    }
                    //Debug.Log("latéral arrière gauche");
                }
            }
        }
        else
        {
            //avant ou arrière
            if (hitPosition.x > 0)
            {
                // sur la droite
                //knockbackVector = ((hitCollisionReceiver.GetMovementDirection().normalized + hitCollisionReceiver.GetMovementDirection().GetLeftOrthogonalVectorXZ().normalized) / 2).normalized;
                knockbackVector = hitCollisionReceiver.GetMovementDirection().GetLeftOrthogonalVectorXZ().normalized;
                redirectionVector = ((hitCollisionReceiver.GetMovementDirection().normalized + hitCollisionReceiver.GetMovementDirection().GetLeftOrthogonalVectorXZ().normalized) / 2).normalized;
                //Debug.Log("frontal droite");
            }
            else
            {
                // sur la gauche
                //knockbackVector = ((hitCollisionReceiver.GetMovementDirection().normalized + hitCollisionReceiver.GetMovementDirection().GetRightOrthogonalVectorXZ().normalized) / 2).normalized;
                knockbackVector = hitCollisionReceiver.GetMovementDirection().GetRightOrthogonalVectorXZ().normalized;
                redirectionVector = ((hitCollisionReceiver.GetMovementDirection().normalized + hitCollisionReceiver.GetMovementDirection().GetRightOrthogonalVectorXZ().normalized) / 2).normalized;
                //Debug.Log("frontal gauche");
            }
        }
        #endregion

        #endregion

        DealKnockback(hitCollisionReceiver, relatedShip.ShipMvt.GetCurrentRammingParameters.GetInflictedKnockbackParameters, knockbackVector, redirectionVector);

        ReceiveKnockback(this, relatedShip.ShipMvt.GetCurrentRammingParameters.GetSustainedKnockbackParametersOnImpact, -relatedShip.GetShipVelocity.normalized, relatedShip.GetShipVelocity.normalized);

        #region Feedback
        if (waitBeforeRammingFeedback)
            return;

        if ((relatedShip as EnemyShip != null && collision.gameObject.GetComponent<PlayerShip>() != null)/* || (relatedShip as PlayerShip != null && collision.gameObject.GetComponent<EnemyShip>() != null)*/)
        {
            GameManager.gameManager.SlwMoManager.SetSlowMo(relatedShip.ShipMvt.GetCurrentRammingParameters.GetSlowMoParameters);
        }

        GameManager.gameManager.ScrshkManager.StartScreenshake(relatedShip.ShipMvt.GetCurrentRammingParameters.GetScreenshakeParameters);

        FeedbackObject woodProjection = GameManager.gameManager.PoolManager.GetFeedbackObject(FeedbackObjectPoolTag.WoodDestruction, PoolInteractionType.GetFromPool);
        if(woodProjection != null)
        {
            woodProjection.transform.position = worldHitPosition;
            woodProjection.StartFeedback(2, 0.2f);
        }
        StartCoroutine(WaitBeforeReplayRammingFeedback());
        #endregion
    }
    #endregion

    bool waitBeforeRammingFeedback;
    public IEnumerator WaitBeforeReplayRammingFeedback()
    {
        waitBeforeRammingFeedback = true;
        yield return new WaitForSeconds(0.2f);
        waitBeforeRammingFeedback = false;
    }
}
