using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(MapDockSpot))]
[CanEditMultipleObjects]
public class MapDockSpotEditor : Editor
{
    public MapDockSpot mapSpecialPlaceSpot { get { return target as MapDockSpot; } }

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
