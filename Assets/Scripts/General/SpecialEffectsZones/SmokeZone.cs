using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SmokeZone : SpecialEffectZone
{
    SmokeZoneParameters currentSmokeZoneParameters;
    List<Ship> currentlyAffectedShips; 

    public override void SetUpZone(float duration, float size, object specialParameter)
    {
        base.SetUpZone(duration, size, specialParameter);
        currentlyAffectedShips = new List<Ship>();

        if((specialParameter as SmokeZoneParameters) != null)
            currentSmokeZoneParameters = specialParameter as SmokeZoneParameters;
    }

    #region Trigger
    public override void OnTriggerEnter(Collider other)
    {
        ShipHitbox hitShipHitbox = other.GetComponent<ShipHitbox>();
        if(hitShipHitbox != null)
        {
            currentlyAffectedShips.Add(hitShipHitbox.GetRelatedShip);
            hitShipHitbox.GetRelatedShip.Blind();
        }
    }

    public override void OnTriggerStay(Collider other)
    {

    }

    public override void OnTriggerExit(Collider other)
    {
        ShipHitbox hitShipHitbox = other.GetComponent<ShipHitbox>();
        if (hitShipHitbox != null)
        {
            if (currentSmokeZoneParameters != null)
            {
                if (currentSmokeZoneParameters.GetBlindingDuration != 0)
                    hitShipHitbox.GetRelatedShip.Blind(currentSmokeZoneParameters.GetBlindingDuration);
                else
                    hitShipHitbox.GetRelatedShip.Unblind();
            }
            else
                hitShipHitbox.GetRelatedShip.Unblind();

            currentlyAffectedShips.Remove(hitShipHitbox.GetRelatedShip);
        }
    }
    #endregion

    public override void SetIneffective()
    {
        base.SetIneffective();

        foreach(Ship ship in currentlyAffectedShips)
        {
            if (currentSmokeZoneParameters != null)
            {
                if (currentSmokeZoneParameters.GetBlindingDuration != 0)
                    ship.Blind(currentSmokeZoneParameters.GetBlindingDuration);
                else
                    ship.Unblind();
            }
            else
                ship.Unblind();
        }

        currentlyAffectedShips = new List<Ship>();
    }
}

[System.Serializable]
public class SmokeZoneParameters
{
    public bool IsNull { get { return zoneDuration == 0 || zoneSize == 0; } }

    public SmokeZoneParameters(SmokeZoneParameters parameters)
    {
        zoneDuration = parameters.zoneDuration;
        zoneSize = parameters.zoneSize;
        blindingDuration = parameters.blindingDuration;
    }

    [SerializeField] float zoneDuration;
    public float GetZoneDuration { get { return zoneDuration; } }

    [SerializeField] float zoneSize;
    public float GetZoneSize { get { return zoneSize; } }

    [SerializeField] float blindingDuration;
    public float GetBlindingDuration { get { return blindingDuration; } }
}
