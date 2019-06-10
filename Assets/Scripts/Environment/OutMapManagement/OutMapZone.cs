using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OutMapZone : MonoBehaviour
{
    OutMapManager outMapManager;
    private void Start()
    {
        outMapManager = GameManager.gameManager.OutMapMng;
    }

    private void OnTriggerEnter(Collider other)
    {
        PlayerShipHitbox playerHitbox = other.GetComponent<PlayerShipHitbox>();
        if (playerHitbox != null)
            outMapManager.EnterOutMapZone(this);
    }

    private void OnTriggerExit(Collider other)
    {
        PlayerShipHitbox playerHitbox = other.GetComponent<PlayerShipHitbox>();
        if (playerHitbox != null)
            outMapManager.ExitOutMapZone(this);
    }
}
