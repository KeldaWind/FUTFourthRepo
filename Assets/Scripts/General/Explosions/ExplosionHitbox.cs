using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionHitbox : MonoBehaviour, IDamageSource, ICollisionSource {

    [SerializeField] Explosion relatedExplosion;
    AttackTag attackTag;
    public void SetAttackTag(AttackTag atkTag)
    {
        attackTag = atkTag;
    }

    private void OnTriggerStay(Collider other)
    {
        IDamageReceiver hitDamageReceiver = other.GetComponent<IDamageReceiver>();
        if (hitDamageReceiver != null)
        {
            DealDamage(hitDamageReceiver, relatedExplosion.CurrentExplParameters.explosionAttackParameters.GetDamagesParameters);
        }

        ICollisionReceiver hitCollisionReceiver = other.GetComponent<ICollisionReceiver>();
        if (hitCollisionReceiver != null)
        {
            Vector3 knockbackDirection = new Vector3();
            knockbackDirection = other.transform.position - transform.position;
            knockbackDirection = new Vector3(knockbackDirection.x, 0, knockbackDirection.z).normalized;

            DealKnockback(hitCollisionReceiver, relatedExplosion.CurrentExplParameters.explosionAttackParameters.GetKnockbackParameters, knockbackDirection, knockbackDirection);
        }
    }

    public void DealDamage(IDamageReceiver damageReceiver, DamagesParameters damagesParameters)
    {
        if(damageReceiver.GetDamageTag != attackTag || damageReceiver.GetDamageTag == AttackTag.Enemies || damageReceiver.GetDamageTag == AttackTag.Environment)
            damageReceiver.ReceiveDamage(this, damagesParameters, null);
    }

    public void DealKnockback(ICollisionReceiver collisionReceiver, KnockbackParameters knockbackParameters, Vector3 knockbackDirection, Vector3 redirection)
    {
        collisionReceiver.ReceiveKnockback(this, knockbackParameters, knockbackDirection, redirection);
    }

    /// <summary>
    /// Returns the tag from this attackSource
    /// </summary>
    public AttackTag GetDamageTag { get { return attackTag; } }

    public AttackTag GetCollisionTag { get { return attackTag; } }

    public Vector3 GetDamageSourcePosition { get { return transform.position; } }
}
