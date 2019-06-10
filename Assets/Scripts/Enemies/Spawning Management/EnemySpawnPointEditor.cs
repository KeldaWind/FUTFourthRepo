using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;

[CustomEditor(typeof(EnemySpawnPoint))]
[CanEditMultipleObjects]
public class EnemySpawnPointEditor : Editor
{
    public EnemySpawnPoint enemySpawnPoint { get { return target as EnemySpawnPoint; } }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
    }

    private void OnSceneGUI()
    {
        Handles.SphereHandleCap(0, enemySpawnPoint.transform.position, enemySpawnPoint.transform.rotation, 5f, EventType.Repaint);
        Handles.ArrowHandleCap(0, enemySpawnPoint.transform.position, enemySpawnPoint.transform.rotation, 15f, EventType.Repaint);
    }
}
#endif
