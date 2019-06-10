using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionTest : MonoBehaviour
{
    [SerializeField] SpecialEffectZone specialEffectZone;
    [SerializeField] SlowingZoneParameters slowingZoneParameters;

    private void Update()
    {
        /*if(Input.touchCount > 3)
            specialEffectZone.SetUpZone(slowingZoneParameters.GetZoneDuration, slowingZoneParameters.GetZoneSize, slowingZoneParameters);*/
    }
}
