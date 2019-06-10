using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void OnLifeEvent(IDamageReceiver receiver);

/// <summary>
/// Source de dégâts pouvant en infliger aux objets capables de recevoir des dégâts
/// </summary>
public interface IDamageSource
{
    /// <summary>
    /// Used to deal the damage to the inputed damage receiver
    /// </summary>
    /// <param name="damageSource">The object touched by this damage Source</param>
    /// <param name="attackParameters">The parameters of the dealed damage</param>
    void DealDamage(IDamageReceiver damageReceiver, DamagesParameters damagesParameters);

    /// <summary>
    /// Returns the tag from this attackSource
    /// </summary>
    AttackTag GetDamageTag { get; }

    Vector3 GetDamageSourcePosition { get; }
}

/// <summary>
/// Objet capable de recevoir des dégâts
/// </summary>
public interface IDamageReceiver
{
    /// <summary>
    /// Used to receive the damage from the inputed source
    /// </summary>
    /// <param name="damageSource">The damage source that touched this damage receiver</param>
    /// <param name="damagesParameters">The parameters of the received damage</param>
    void ReceiveDamage(IDamageSource damageSource, DamagesParameters damagesParameters, ProjectileSpecialParameters projSpecialParameters);

    void UpdateLifeBar(int lifeAmount);

    /// <summary>
    /// Returns the tag from this damageReceiver
    /// </summary>
    AttackTag GetDamageTag { get; }

    /// <summary>
    /// Returns if this receiver is currently damageable or not
    /// </summary>
    bool IsDamageable { get; }

    /// <summary>
    /// Call this function as the life of this damage receiver reaches 0
    /// </summary>
    void Die();

    /// <summary>
    /// Appelle toutes les fonctions affectées
    /// </summary>
    /// <returns></returns>
    event OnLifeEvent OnDeath;
}

/// <summary>
/// Used to identify a attack source or receiver, and know what a attack source can hurt / what can hurt a attack receiver
/// </summary>
public enum AttackTag
{
    /// <summary>
    /// Will hurt everything except the player / Will take attacks from every source except from the player
    /// </summary>
    Player,
    /// <summary>
    /// Will hurt everything except the enemy ships / Will take attacks from every source except from the enemy ships
    /// </summary>
    Enemies,
    /// <summary>
    /// Will hurt everything except the Beast / Will take attacks from every source except from the beast
    /// </summary>
    TheBeast,
    /// <summary>
    /// Will hurt everything / Will take attacks from every source 
    /// </summary>
    Environment
}