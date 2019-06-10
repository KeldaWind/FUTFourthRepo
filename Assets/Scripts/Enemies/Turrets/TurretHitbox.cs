using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretHitbox : MonoBehaviour, IDamageReceiver
{
    Turret relatedTurret;

    public void SetUp(Turret turret)
    {
        relatedTurret = turret;
    }

    #region Damages Gesture
    public AttackTag GetDamageTag
    {
        get
        {
            return relatedTurret.HitboxTag;
        }
    }

    public bool IsDamageable
    {
        get
        {
            return !relatedTurret.LfManager.Recovering;
        }
    }

    public void Die()
    {
        OnDeath(this);
        relatedTurret.Die();
    }

    public void ReceiveDamage(IDamageSource damageSource, DamagesParameters damagesParameters, ProjectileSpecialParameters projSpecialParameters)
    {
        relatedTurret.LfManager.Damage(damagesParameters.GetDamageAmount, damagesParameters.GetRecoveringTime, damagesParameters.GetRecoveringType);
    }

    public void UpdateLifeBar(int lifeAmount)
    {
        
    }

    public event OnLifeEvent OnDeath;
    #endregion
}
