using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FleeingEnemyShipHitbox : ShipHitbox
{
    [Header("Fleeing Enemy Parameters")]
    [SerializeField] IntroControler[] dialoguesToLaunchOnFleeingEnemyHitByShot;

    public override void ReceiveDamage(IDamageSource damageSource, DamagesParameters damagesParameters, ProjectileSpecialParameters projSpecialParameters)
    {
        if (damageSource.GetDamageTag == AttackTag.Player)
        {
            if (!GameManager.gameManager.CinematicMng.CinematicProcessing)
            {
                IntroControler dialogue = dialoguesToLaunchOnFleeingEnemyHitByShot[Random.Range(0, dialoguesToLaunchOnFleeingEnemyHitByShot.Length)];
                dialogue.PlayCinematic();
            }
        }
    }
}
