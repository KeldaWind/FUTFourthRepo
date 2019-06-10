using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(MapArenaSpot))]
[CanEditMultipleObjects]
public class MapArenaSpotEditor : Editor
{
    public MapArenaSpot mapSpecialPlaceSpot { get { return target as MapArenaSpot; } }

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
