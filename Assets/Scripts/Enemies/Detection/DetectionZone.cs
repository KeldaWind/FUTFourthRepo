using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Une zone sphérique permettant de detecter le joueur et de récupérer sa référence.
/// </summary>
public class DetectionZone : MonoBehaviour
{
    /// <summary>
    /// Le joueur repéré dans cette zone
    /// </summary>
    PlayerShip targetShip;
    /// <summary>
    /// Le joueur repéré dans cette zone
    /// </summary>
    public PlayerShip GetTargetShip
    {
        get
        {
            return targetShip;
        }
    }

    /// <summary>
    /// Le collider capable de détecter le joueur
    /// </summary>
    [SerializeField] SphereCollider sphereCollider;

    /// <summary>
    /// Initialise la zone de détection.
    /// </summary>
    /// <param name="detectionRange">La distance à laquelle cette zone va pouvoir repérer le joueur</param>
    public void SetUp(float detectionRange)
    {
        sphereCollider.radius = detectionRange;
    }

    public void ResetDetectionZone()
    {
        targetShip = null;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (targetShip == null)
        {
            PlayerShipHitbox shipHitbox = other.GetComponent<PlayerShipHitbox>();
            if (shipHitbox != null)
            {
                if (shipHitbox.GetRelatedShip as PlayerShip != null)
                {
                    targetShip = shipHitbox.GetRelatedShip as PlayerShip;
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerShipHitbox shipHitbox = other.GetComponent<PlayerShipHitbox>();
        if (shipHitbox != null)
        {
            targetShip = null;
        }
    }

    public void SetTargetShip(PlayerShip ship)
    {
        targetShip = ship;
    }
}
