using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void OnPlayerEnter(TargetZone targetZone);

public class TargetZone : MonoBehaviour
{
    #region Validation
    bool validated;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerShipHitbox>() != null && !validated)
        {
            Validate();
            OnPlayerEnter(this);
        }
    }

    public void Validate()
    {
        validated = true;
        gameObject.SetActive(false);
    }

    public event OnPlayerEnter OnPlayerEnter;
    #endregion

    #region Apparition
    private void Start()
    {
        if(!started)
            gameObject.SetActive(false);
    }

    bool started;
    public void StartZone()
    {
        started = true;
        gameObject.SetActive(true);
    }
    #endregion
}
