using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowingZone : SpecialEffectZone
{
    List<Ship> currentlyAffectedShips;
    SlowingZoneParameters currentSlowingZoneParameters;

    [Header("Slowing Zone Parameters")]
    [SerializeField] float zoneHeight;

    [Header("Slowing Zone Rendering")]
    [SerializeField] SpriteRenderer slowingZoneSprite;
    [SerializeField] ParticleSystem slowingZonePS;
    float zoneSize;

    public override void SetUpZone(float duration, float size, object specialParameter)
    {
        base.SetUpZone(duration, size, specialParameter);

        transform.localScale = Vector3.zero;
        //transform.localScale = new Vector3(size, size, size);
        currentlyAffectedShips = new List<Ship>();

        if ((specialParameter as SlowingZoneParameters) != null)
            currentSlowingZoneParameters = specialParameter as SlowingZoneParameters;

        Vector3 truePosition = transform.position;
        truePosition.y = GameManager.gameManager.GetSeaLevel;
        transform.position = truePosition;

        slowingZonePS.Play();

        zoneSize = size;
    }

    public override void UpdateZone()
    {
        base.UpdateZone();

        if (zoneRemainingDuration > 0)
        {
            slowingZoneSprite.color = Color.Lerp(slowingZoneSprite.color, zoneColor, appearingCoeff);
            transform.localScale = Vector3.Lerp(transform.localScale , Vector3.one * zoneSize, appearingCoeff);
        }
        else if(zoneRemainingDuration == 0)
        {
            slowingZoneSprite.color = Color.Lerp(slowingZoneSprite.color, clearZoneColor, appearingCoeff);
            transform.localScale = Vector3.Lerp(transform.localScale, Vector3.zero, appearingCoeff);
            if (ReadyToBeTurnedOff)
                SetOff();
        }
    }

    public override bool ReadyToBeTurnedOff
    {
        get
        {
            return slowingZoneSprite.color.a < 0.01 && !slowingZonePS.isPlaying;
        }
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
        slowingZonePS.Stop();
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
