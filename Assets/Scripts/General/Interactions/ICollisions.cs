using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Une souce de collision qui va faire réagir un objet capable de réagir aux collisions (knockbacks etc)
/// </summary>
public interface ICollisionSource
{
    /// <summary>
    /// Used to deal the knockback to the inputed damage receiver
    /// </summary>
    /// <param name="damageSource">The object touched by this damage Source</param>
    /// <param name="attackParameters">The parameters of the dealed damage</param>
    /// <param name="knockbackDirection">The direction in which the receiver needs to be pushed</param>
    /// <param name="redirection">The direction the receiver needs to follow as he receives this knockback</param>
    void DealKnockback(ICollisionReceiver collisionReceiver, KnockbackParameters knockbackParameters, Vector3 knockbackDirection, Vector3 redirection);

    /// <summary>
    /// Returns the tag from this collisionSource
    /// </summary>
    AttackTag GetCollisionTag { get; }
}

/// <summary>
/// Objet capable de réagir aux collisions (knockbacks etc)
/// </summary>
public interface ICollisionReceiver
{
    /// <summary>
    /// Used to receive the knockback from the inputed source
    /// </summary>
    /// <param name="damageSource">The damage source that touched this damage receiver</param>
    /// <param name="damagesParameters">The parameters of the received damage</param>
    /// /// <param name="knockbackDirection">The direction in which the receiver needs to be pushed</param>
    /// <param name="redirection">The direction the receiver needs to follow as he receives this knockback</param>
    void ReceiveKnockback(ICollisionSource collisionSource, KnockbackParameters knockbackParameters, Vector3 knockbackDirection, Vector3 redirection);

    /// <summary>
    /// Returns the tag from this collisionReceiver
    /// </summary>
    AttackTag GetCollisionTag { get; }

    Vector3 GetMovementDirection();

    bool Recovering();
}
