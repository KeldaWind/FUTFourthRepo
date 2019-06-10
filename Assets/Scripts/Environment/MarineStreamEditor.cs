using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(MarineStream))]
[CanEditMultipleObjects]
public class MarineStreamEditor : Editor
{
    public MarineStream marineStream { get { return target as MarineStream; } }
    float streamSpeedPerUnit = 1;
    float streamLifetimePerUnit = 1;

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (streamSpeedPerUnit != marineStream.GetStreamSpeedPerUnit)
        {
            streamSpeedPerUnit = marineStream.GetStreamSpeedPerUnit;
            marineStream.SetUpStreamRenderer();
        }

        if (streamLifetimePerUnit != marineStream.GetStreamLifetimePerUnit)
        {
            streamLifetimePerUnit = marineStream.GetStreamLifetimePerUnit;
            marineStream.SetUpStreamRenderer();
        }
    }
}
#endif