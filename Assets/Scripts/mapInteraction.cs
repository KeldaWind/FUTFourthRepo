using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapInteraction : MonoBehaviour
{
    [SerializeField] GameObject cinemachineCamPlayer;
    [SerializeField] GameObject cameraOftheLevel;
    [SerializeField] Animator interfaceInterLevel;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerShip>())
        {
            cinemachineCamPlayer.SetActive(false);
            cameraOftheLevel.SetActive(true);
            interfaceInterLevel.SetTrigger("Transition");
            //GameManager.
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<PlayerShip>())
        {
            cinemachineCamPlayer.SetActive(true);
            cameraOftheLevel.SetActive(false);
            interfaceInterLevel.SetTrigger("Transition");
        }
    }
}
