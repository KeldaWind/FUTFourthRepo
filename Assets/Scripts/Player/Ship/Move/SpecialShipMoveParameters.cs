using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct SpecialShipMoveParameters
{
    public bool IsNull { get { return duration == 0; } }

    #region Management
    public void StartParameter()
    {

    }

    public void UpdateParameter()
    {
        if (duration > 0)
            duration -= Time.deltaTime;
        else if (duration < 0)
            EndParameter();
    }

    public void EndParameter()
    {
        duration = 0;
        anchoredShip = false;
        speedBoost = 1;
        maniabilityBoost = 1;
    }
    #endregion

    [Header("Common")]
    [SerializeField] float duration;
    public float GetDuration { get { return duration; } }

    [Header("Anchor")]
    [SerializeField] bool anchoredShip;
    public bool ShipIsAnchored { get { return anchoredShip; } }

    [Header("Ship Boost")]
    [SerializeField] float speedBoost;
    public float GetSpeedBoost
    {
        get
        {
            return (speedBoost != 0) ? speedBoost : 1;
        }
    }

    [SerializeField] float maniabilityBoost;
    public float GetManiabilityBoost
    {
        get
        {
            return (maniabilityBoost != 0) ? maniabilityBoost : 1;
        }
    }

    public bool ShipBoost
    {
        get
        {
            return GetSpeedBoost != 1;
        }
    }

    public bool ManiabilityBoost
    {
        get
        {
            return GetManiabilityBoost != 1 && GetManiabilityBoost != 0;
        }
    }
}
