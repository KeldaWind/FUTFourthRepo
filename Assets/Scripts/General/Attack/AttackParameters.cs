using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Paramètres de dégâts et de knockback infligés par un attaque
/// </summary>
[System.Serializable]
public class AttackParameters
{
    /// <summary>
    /// Dégâts infligés par cette attaque
    /// </summary>
    [SerializeField] DamagesParameters damagesParameters;
    public DamagesParameters GetDamagesParameters { get { return damagesParameters; } }

    /// <summary>
    /// Knockback infligé par cette attaque
    /// </summary>
    [SerializeField] KnockbackParameters knockbackParameters;
    public KnockbackParameters GetKnockbackParameters { get { return knockbackParameters; } }
}
