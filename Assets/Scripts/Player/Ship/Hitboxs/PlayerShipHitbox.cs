using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Objet de collision du joueu
/// </summary>
public class PlayerShipHitbox : ShipHitbox
{
    public override void ReceiveDamage(IDamageSource damageSource, DamagesParameters damagesParameter, ProjectileSpecialParameters projSpecialParameterss)
    {
        if (!GameManager.gameManager.Won && GameManager.gameManager.StartedFight)
            base.ReceiveDamage(damageSource, damagesParameter, projSpecialParameterss);
    }

    public override void UpdateLifeBar(int lifeAmount)
    {
        (relatedShip as PlayerShip).PlrInterface.UpdateLifeBar(lifeAmount);
    }
}
