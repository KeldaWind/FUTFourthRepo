using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowingZone : SpecialEffectZone
{
    List<Ship> currentlyAffectedShips;
    SlowingZoneParameters currentSlowingZoneParameters;

    [Header("Slowing Zone Parameters")]
    [SerializeField] float zoneHeight;

    public override void SetUpZone(float duration, float size, object specialParameter)
    {
        base.SetUpZone(duration, size, specialParameter);

        transform.localScale = new Vector3(size, zoneHeight, size);
        currentlyAffectedShips = new List<Ship>();

        if ((specialParameter as SlowingZoneParameters) != null)
            currentSlowingZoneParameters = specialParameter as SlowingZoneParameters;

        Vector3 truePosition = transform.position;
        truePosition.y = GameManager.gameManager.GetSeaLevel;
        transform.position = truePosition;
    }

    #region Trigger
    public override void OnTriggerEnter(Collider other)
    {
        ShipHitbox hitShipHitbox = other.GetComponent<ShipHitbox>();
        if (hitShipHitbox != null)
        {
            currentlyAffectedShips.Add(hitShipHitbox.GetRelatedShip);

            if (currentSlowingZoneParameters != null)
                hitShipHitbox.GetRelatedShip.ShipMvt.StartNewSpeedModifier(currentSlowingZoneParameters.GetShipSpeedModifier, this);
        }
    }

    public override void OnTriggerExit(Collider other)
    {
        ShipHitbox hitShipHitbox = other.GetComponent<ShipHitbox>();
        if (hitShipHitbox != null)
        {
            hitShipHitbox.GetRelatedShip.ShipMvt.GetOutSlowingZone(this);

            currentlyAffectedShips.Remove(hitShipHitbox.GetRelatedShip);
        }
    }
    #endregion

    public override void SetIneffective()
    {
        base.SetIneffective();
        foreach(Ship ship in currentlyAffectedShips)
        {
            ship.ShipMvt.GetOutSlowingZone(this);
        }
    }
}

[System.Serializable]
public class SlowingZoneParameters
{
    public bool IsNull { get { return zoneDuration == 0 || zoneSize == 0; } }

    public SlowingZoneParameters(SlowingZoneParameters parameters)
    {
        zoneDuration = parameters.zoneDuration;
        zoneSize = parameters.zoneSize;
        shipSpeedModifier = new ShipSpeedModifier(parameters.GetShipSpeedModifier);
    }

    [SerializeField] float zoneDuration;
    public float GetZoneDuration { get { return zoneDuration; } }

    [SerializeField] float zoneSize;
    public float GetZoneSize { get { return zoneSize; } }

    [SerializeField] ShipSpeedModifier shipSpeedModifier;
    public ShipSpeedModifier GetShipSpeedModifier { get { return shipSpeedModifier; } }
}
