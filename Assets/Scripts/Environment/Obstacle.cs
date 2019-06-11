using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour, IDamageSource, ICollisionSource
{
    /// <summary>
    /// Le tag de cet obstacle (souvent, environment)
    /// </summary>
    [SerializeField] AttackTag obstacleTag = AttackTag.Environment;
    public static bool playerHitObstacleOnce;

    private void Start()
    {
        damagesParameters = new DamagesParameters(1, 0.3f, RecoveringType.ConsidersRecover);
        knockbackParameters = new KnockbackParameters(20, 0.15f, 0.1f, true, 0.6f, 0.3f);
    }

    private void OnCollisionEnter(Collision collision)
    {
        ShipHitbox hitShipHitbox = collision.gameObject.GetComponent<ShipHitbox>();

        IDamageReceiver damageReceiver = collision.gameObject.GetComponent<IDamageReceiver>();
        if (damageReceiver != null)
        {
            if(hitShipHitbox != null)
            {
                if (!playerHitObstacleOnce)
                {
                    PlayerShip playerShip = hitShipHitbox.GetRelatedShip as PlayerShip;
                    if (playerShip != null)
                    {
                        if (ArenaManager.arenaManager != null)
                        {
                            ArenaManager.arenaManager.ScoreMng.SetPlayerHasHitObstacle();
                            playerHitObstacleOnce = true;
                        }
                    }
                }

                if (!hitShipHitbox.GetRelatedShip.BeingSkewered)
                {
                    bool enemyDontGetDamaged = false;
                    EnemyShip enemyShip = hitShipHitbox.GetRelatedShip as EnemyShip;
                    if (enemyShip != null)
                    {
                        if (enemyShip.GetCurrentLifeAmount <= damagesParameters.GetDamageAmount)
                            enemyDontGetDamaged = true;
                    }

                    if (!enemyDontGetDamaged)
                        DealDamage(damageReceiver, damagesParameters);
                }
                else
                {
                    hitShipHitbox.ReceiveDamage(hitShipHitbox, hitShipHitbox.GetRelatedShip.GetSkeweringParameters.GetDamagesOnImpact, null);
                    hitShipHitbox.GetRelatedShip.SetStun(hitShipHitbox.GetRelatedShip.GetSkeweringParameters.GetStunOnObstacleDuration);
                    hitShipHitbox.GetRelatedShip.EndSkewering();
                }
            }
            else
                DealDamage(damageReceiver, damagesParameters);
        }

        ICollisionReceiver collisionReceiver = collision.gameObject.GetComponent<ICollisionReceiver>();
        if (collisionReceiver != null)
        {
            Vector3 normal = new Vector3();
            float counter = 0;
            Vector3 pos = new Vector3();

            foreach (ContactPoint contact in collision.contacts)
            {
                normal -= contact.normal;
                counter++;
                pos += contact.point;
            }
            
            normal /= counter;
            pos /= counter;

            normal.y = 0;
            normal.Normalize();

            Vector3 redirection = Vector3.Reflect(collisionReceiver.GetMovementDirection(), normal).normalized;
            redirection.y = 0;
            redirection.Normalize();

            if (hitShipHitbox != null)
            {
                if (hitShipHitbox.GetRelatedShip.BeingSkewered)
                    redirection = Vector3.zero;
            }

            DealKnockback(collisionReceiver, knockbackParameters, normal, redirection);
        }
    }

    #region Damages
    /// <summary>
    /// Les dégâts que subiront les objets entrant en contact avec cet obstacle
    /// </summary>
    [Header("Damages")]
    /*[SerializeField] */DamagesParameters damagesParameters;

    /// <summary>
    /// Le tag de cet obstacle (souvent, environment)
    /// </summary>
    public AttackTag GetDamageTag
    {
        get
        {
            return obstacleTag;
        }
    }

    /// <summary>
    /// Inflige des dégâts à l'objet entrant en contact avec l'obstacle
    /// </summary>
    /// <param name="damageReceiver"></param>
    /// <param name="damagesParameters"></param>
    public void DealDamage(IDamageReceiver damageReceiver, DamagesParameters damagesParameters)
    {
        damageReceiver.ReceiveDamage(this, damagesParameters, null);
    }

    public Vector3 GetDamageSourcePosition { get { return transform.position; } }
    #endregion

    #region Collision
    /// <summary>
    /// Le knockback que subiront les objets entrant en contact avec cet obstacle
    /// </summary>
    [Header("Collision and Knockback")]
    /*[SerializeField]*/ KnockbackParameters knockbackParameters;

    /// <summary>
    /// Le tag de cet obstacle (souvent, environment)
    /// </summary>
    public AttackTag GetCollisionTag
    {
        get
        {
            return obstacleTag;
        }
    }

    /// <summary>
    /// Inflige un knockback à l'objet entrant en contact avec l'obstacle
    /// </summary>
    /// <param name="collisionReceiver"></param>
    /// <param name="knockbackParameters"></param>
    /// <param name="knockbackDirection"></param>
    /// <param name="redirection"></param>
    public void DealKnockback(ICollisionReceiver collisionReceiver, KnockbackParameters knockbackParameters, Vector3 knockbackDirection, Vector3 redirection)
    {
        collisionReceiver.ReceiveKnockback(this, knockbackParameters, knockbackDirection, redirection);
    }
    #endregion
}
