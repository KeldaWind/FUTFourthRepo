using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class MapSpecialPlaceSpot : MonoBehaviour
{
    [Header("References")]
    [SerializeField] protected CinemachineVirtualCamera cameraWhenOnSpot;
    [SerializeField] protected Transform playerTransformOnceStopped;
    public Transform GetPlayerTransformOnceStopped
    {
        get
        {
            if (playerTransformOnceStopped != null)
                return playerTransformOnceStopped;
            else
                return transform;
        }
    }

    bool dontActivateSincePlayerOut;
    public void SetDontActivateSincePlayerOut()
    {
        dontActivateSincePlayerOut = true;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (dontActivateSincePlayerOut)
            return;

        PlayerShip playerShip = other.GetComponent<PlayerShip>();
        if (playerShip != null)
            StartSpotInteraction(playerShip);
    }

    public void OnTriggerExit(Collider other)
    {
        if (dontActivateSincePlayerOut)
        {
            PlayerShip playerShip = other.GetComponent<PlayerShip>();
            if (playerShip != null)
                dontActivateSincePlayerOut = false;
        }
    }

    public virtual void StartSpotInteraction(PlayerShip player)
    {
        if (cameraWhenOnSpot != null)
            cameraWhenOnSpot.gameObject.SetActive(true);

        if (playerTransformOnceStopped != null)
            player.ShipMvt.SetTransformToTakeOnStop(playerTransformOnceStopped);
    }

    public virtual void EndSpotInteraction()
    {
        if (cameraWhenOnSpot != null)
            cameraWhenOnSpot.gameObject.SetActive(false);
    }

    [Header("Unlocking")]
    [SerializeField] GameObject[] objectsToActivateOnUnlock;
    [SerializeField] ParticleSystem[] particlesToPlayOnUnlock;
    public void UnlockSpot(bool playParticles)
    {
        gameObject.SetActive(true);

        foreach (GameObject obj in objectsToActivateOnUnlock)
            obj.SetActive(true);

        if (playParticles)
        {
            foreach (ParticleSystem particles in particlesToPlayOnUnlock)
            {
                particles.gameObject.SetActive(true);
                particles.Play();
            }
        }
    }

    public void JustUnlockSpot()
    {
        gameObject.SetActive(true);

        foreach (GameObject obj in objectsToActivateOnUnlock)
            obj.SetActive(true);

        foreach (ParticleSystem particles in particlesToPlayOnUnlock)
        {
            particles.gameObject.SetActive(true);
            particles.Play();
        }
    }
}
