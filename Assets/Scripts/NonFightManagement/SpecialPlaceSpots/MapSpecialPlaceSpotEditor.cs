using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(MapSpecialPlaceSpot))]
[CanEditMultipleObjects]
public class MapSpecialPlaceSpotEditor : Editor
{
    public MapSpecialPlaceSpot mapSpecialPlaceSpot { get { return target as MapSpecialPlaceSpot; } }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
    }

    private void OnSceneGUI()
    {
        Transform playerTransformOnceStopped = mapSpecialPlaceSpot.GetPlayerTransformOnceStopped;
        if (playerTransformOnceStopped != null)
        {
            Handles.SphereHandleCap(0, playerTransformOnceStopped.position, playerTransformOnceStopped.rotation, 5f, EventType.Repaint);
            Handles.ArrowHandleCap(0, playerTransformOnceStopped.position, playerTransformOnceStopped.rotation, 15f, EventType.Repaint);
        }
    }
}
#endif