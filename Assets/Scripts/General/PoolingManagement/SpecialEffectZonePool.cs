using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SpecialEffectZonePool : Pool<SpecialEffectZone>
{
    [SerializeField] SpecialEffectZonePoolTag specialEffectZonePoolTag;
    public SpecialEffectZonePoolTag GetSpecialEffectZonePoolTag { get { return specialEffectZonePoolTag; } }
}

public enum SpecialEffectZonePoolTag
{
    Smoke, SpeedModifier
}
